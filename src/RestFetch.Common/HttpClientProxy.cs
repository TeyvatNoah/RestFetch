using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace RestFetch;

public sealed class HttpClientProxy(HttpClientProxyOptions options): IHttpClientProxy<HttpClient, HttpResponseMessage> {
	private readonly bool _sharedClient = options switch {
		HttpClientProxyOptions.HttpClientProxyClientOptions o => o.SharedClient,
		HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions => false
	};

	private readonly HttpClient _client = options switch {
		HttpClientProxyOptions.HttpClientProxyClientOptions o => o.Client,
		HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions o => new(o.Handler, !o.SharedHandler)
	};

	public object? Options { get; init; } = options;

	public string BaseAddress { get => _client.BaseAddress?.ToString() ?? ""; set => _client.BaseAddress = new(value, new UriCreationOptions() {
		DangerousDisablePathAndQueryCanonicalization = true
	}); }


	public HttpClientProxy(): this(new HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions(
		Handler: new SocketsHttpHandler() {
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
			}},
			SharedHandler: false)) {}

	public HttpClientProxy(HttpMessageHandler handler): this(new HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions(
		Handler: handler,
		SharedHandler: false)) => _client = new(handler);

	public HttpClientProxy(HttpClient client, bool sharedClient = true): this(new HttpClientProxyOptions.HttpClientProxyClientOptions(
		Client: client,
		SharedClient: sharedClient)) {}

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

		return _client.SendAsync(msg, context.ReturnsAsyncEnumerable ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, token);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IHttpClientProxy<HttpClient, HttpResponseMessage> Create(string baseAddress)
		=> new HttpClientProxy() {
			BaseAddress = baseAddress
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IHttpClientProxy<HttpClient, HttpResponseMessage> Create(HttpClient client)
		=> new HttpClientProxy(client);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() {
		if (!_sharedClient) {
			_client.Dispose();
		}
	}
}

