using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace RestFetch;

public sealed class HttpClientProxy(HttpClientProxyOptions options): IHttpClientProxy<HttpClientProxy, HttpResponseMessage> {
	private readonly Func<HttpClient>? _factory = options is HttpClientProxyOptions.HttpClientProxyFactoryOptions o ? o.Factory : null;

	private readonly HttpClient? _client = options is HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions o ? new(o.Handler, !o.SharedHandler) : null;

	public object? Options { get; init; } = options;

	public required string? BaseAddress {
		get => _client?.BaseAddress?.ToString() ?? field;
		set {
			if (_client is null) {
				field = value;
			} else {
				_client.BaseAddress = value is null ? null : new(value, new UriCreationOptions {
					DangerousDisablePathAndQueryCanonicalization = true
				});
			}
		}
	}


	public HttpClientProxy(): this(new HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions(
		new SocketsHttpHandler() {
			// Proxy = new WebProxy("http://127.0.0.1:2333", false),
			AllowAutoRedirect = true,
			AutomaticDecompression = DecompressionMethods.All,
			PooledConnectionLifetime = TimeSpan.FromMinutes(2),
			MaxConnectionsPerServer = 100,
			// 注意会共享CookieContainer
			UseCookies = true,
			SslOptions = new() {
				EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
				CertificateRevocationCheckMode = X509RevocationMode.Online
			}}) {
				SharedHandler = false
			}) {}

	public HttpClientProxy(HttpMessageHandler handler): this(new HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions(handler) {
		SharedHandler = false
	}) {}

	// public HttpClientProxy(HttpClient client, bool sharedClient = true): this(new HttpClientProxyOptions.HttpClientProxyClientOptions(
	// 	Client: client,
	// 	SharedClient: sharedClient)) {}

	[DoesNotReturn]
	private static void ThrowHeaderException(string key, string value) => throw new InvalidOperationException($"Cannot add HTTP header '{key}': '{value}'.");

	[DoesNotReturn]
	private static void ThrowContentException() => throw new ArgumentException("Argument TRequestBodyContent body is not HttpContent.");

	public Task<HttpResponseMessage> SendAsync<TRequestBodyContent>(
		TRequestBodyContent? body,
		ApiContext context,
		ResourceManager resourceManager,
		CancellationToken token) {
		// 不using,由外部请求方法释放
		var msg = resourceManager.TakeOwnership(new HttpRequestMessage(context.HttpMethod, context.Url));
		// var msg = new HttpRequestMessage(context.HttpMethod, context.Url);

		// if (context.ReturnTypeInfo.ReturnTypeKind != ReturnTypeKind.RawResponse) {
		// 	resourceManager.TakeOwnership(msg);
		// }

		if (body is HttpContent content) {
			msg.Content = content;
		} else if (body is not null) {
			ThrowContentException();
		}

		if (context.Headers is not null) {
			foreach (var (key, value) in context.Headers) {
				if (msg.Headers.TryAddWithoutValidation(key, value)) {
					continue;
				} else if (msg.Content?.Headers.TryAddWithoutValidation(key, value) ?? true) {
					continue;
				} else {
					ThrowHeaderException(key, value);
				}
			}
		}

		if (context.Metadata is {} metadata) {
			foreach (var (key, value) in metadata) {
				msg.Options.Set(new(key), value);
			}
		}

		var client = _factory?.Invoke() is HttpClient c ? resourceManager.TakeOwnership(c) : _client;

		if (client is null) {
			ThrowUnreachableException(nameof(client));
		}

		return client.SendAsync(msg, context.ReturnsAsyncEnumerable ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, token);
	}

	[DoesNotReturn]
	private static void ThrowUnreachableException(string name) {
		throw new UnreachableException($"The {name} cannot be null.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HttpClientProxy Create(string? baseAddress)
		=> new HttpClientProxy() {
			BaseAddress = baseAddress
		};

	// [MethodImpl(MethodImplOptions.AggressiveInlining)]
	// public static IHttpClientProxyService<HttpResponseMessage> Create(HttpClient client)
	// 	=> new HttpClientProxy(client);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() => _client?.Dispose();
}

