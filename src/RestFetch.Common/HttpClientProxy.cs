using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace RestFetch;

// public interface IHttpClientProxy {
// 	TResponse Request<TResponse>(int param);
// }
public sealed class HttpClientProxy(bool sharedHandler, HttpMessageHandler handler): IHttpClientProxy {
	private readonly bool _OwnedHandler = !sharedHandler;

	private readonly HttpClient _Client = new(handler);

	public required string BaseAddress { get => _Client.BaseAddress?.ToString() ?? ""; set => _Client.BaseAddress = new(value, new UriCreationOptions() {
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

	public HttpClientProxy(): this(false, new SocketsHttpHandler() {
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
	}) {}

	public HttpClientProxy(HttpMessageHandler handler): this(true, handler) => _Client = new(handler);

	public Task<HttpResponseMessage> SendAsync<TRequestBodyContent>(ref readonly APIContext context, Dictionary<string, string>? headers, TRequestBodyContent? body, CancellationToken token) {
		using var msg = new HttpRequestMessage(context.HttpMethod, context.URL);
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
		return _Client.SendAsync(msg, token);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HttpClientProxy Create(string baseAddress)
		=> new() {
			BaseAddress = baseAddress
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() {
		if (_OwnedHandler) {
			_Client.Dispose();
		}
	}
}

public interface IRestFetchOptions {
	
}

public sealed class RestFetchOptions {
	
}


public static class RestFetchService<THttpClientProxy> where THttpClientProxy: IHttpClientProxy {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(string baseAddress) where TAPI: IRestFetchAPI<TAPI>
		=> TAPI.Create(baseAddress);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(THttpClientProxy httpClientProxy) where TAPI: IRestFetchAPI<TAPI>
		=> TAPI.Create(httpClientProxy);
}

public static class RestFetchService<TResponse, THttpClientProxy>
	where TResponse: IDisposable
	where THttpClientProxy: IHttpClientProxy<TResponse, THttpClientProxy> {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(string baseAddress) where TAPI: IRestFetchAPI<TAPI, TResponse, THttpClientProxy>
		=> TAPI.Create(baseAddress);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(THttpClientProxy httpClientProxy) where TAPI: IRestFetchAPI<TAPI, TResponse, THttpClientProxy>
		=> TAPI.Create(httpClientProxy);
}


public static class RestFetchService {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(string baseAddress) where TAPI: IRestFetchAPI<TAPI>
		=> TAPI.Create(baseAddress);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(HttpClientProxy httpClientProxy) where TAPI: IRestFetchAPI<TAPI>
		=> TAPI.Create(httpClientProxy);
}