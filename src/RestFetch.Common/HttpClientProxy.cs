using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace RestFetch;

public sealed class HttpClientProxy(HttpClientProxyOptions options): IHttpClientProxy<HttpResponseMessage, HttpClientProxy> {
	private readonly bool _ownedHandler = !options.SharedHandler;

	private readonly HttpClient _client = new(options.Handler);

	public object? Options { get; init; } = options;

	public required string BaseAddress { get => _client.BaseAddress?.ToString() ?? ""; set => _client.BaseAddress = new(value, new UriCreationOptions() {
		DangerousDisablePathAndQueryCanonicalization = true
	}); }

	public static readonly SocketsHttpHandler SharedDefaultHandler = new() {
		AllowAutoRedirect = true,
		AutomaticDecompression = DecompressionMethods.All,
		PooledConnectionLifetime = TimeSpan.FromMinutes(2),
		MaxConnectionsPerServer = 100,
		// 注意会共享CookieContainer
		UseCookies = true,
		SslOptions = new() {
			EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
			CertificateRevocationCheckMode = X509RevocationMode.Online
		}
	};

	public HttpClientProxy(): this(new HttpClientProxyOptions(
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
			SharedHandler: false,
			JsonSerializerOptions: null)) {}

	public HttpClientProxy(HttpMessageHandler handler): this(new HttpClientProxyOptions(
		Handler: handler,
		SharedHandler: false,
		JsonSerializerOptions: null
	)) => _client = new(handler);

	public Task<HttpResponseMessage> SendAsync<TRequestBodyContent>(ref readonly ApiContext context, Dictionary<string, string>? headers, TRequestBodyContent? body, CancellationToken token) {
		// 不using,当HttpResponseMessage被Dispose的时候它会被释放
		var msg = new HttpRequestMessage(context.HttpMethod, context.Url);
		if (headers is not null) {
			foreach (var (key, value) in headers) {
				msg.Headers.TryAddWithoutValidation(key, value);
			}
		}
		if (body is HttpContent content) {
			msg.Content = content;
		} else {
			// TODO
		}
		return _client.SendAsync(msg, context.ReturnsAsyncEnumerable ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, token);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HttpClientProxy Create(string baseAddress)
		=> new() {
			BaseAddress = baseAddress
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() {
		if (_ownedHandler) {
			_client.Dispose();
		}
	}
}

