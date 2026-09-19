using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace RestFetch;

// public interface IHttpClientProxy {
// 	TResponse Request<TResponse>(int param);
// }
public sealed class HttpClientProxy: IHttpClientProxy {
	private readonly HttpClient _Client;

	public required Uri BaseAddress { get => _Client.BaseAddress!; set => _Client.BaseAddress = value; }

	public static readonly SocketsHttpHandler DefaultHandler = new() {
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

	public HttpClientProxy() => _Client = new(DefaultHandler);

	public HttpClientProxy(HttpMessageHandler handler) => _Client = new(handler);

	public Task<HttpResponseMessage> SendAsync<TRequestBodyContent>(ref readonly APIInfo<URLProxy, URLDisposer, HttpResponseMessage, HttpClientProxy> context, Dictionary<string, string>? headers, TRequestBodyContent? body, CancellationToken token) {
		using var msg = new HttpRequestMessage(context.HttpMethod, context.URLProxy.Uri is null ? new(context.URLProxy.URL) : context.URLProxy.Uri);
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
	public void Dispose() => _Client.Dispose();
}

public interface IRestFetchOptions {
	
}

public sealed class RestFetchOptions {
	
}


public static class RestFetch<TOptions> where TOptions: IRestFetchOptions {
	public static TAPI For<TAPI>(string baseAddress) {
		throw new NotImplementedException();
	}

	public static TAPI For<TAPI>(TOptions options) {
		throw new NotImplementedException();
	}
}


public static class RestFetch {
	public static TAPI For<TAPI>(string baseAddress) {
		throw new NotImplementedException();
	}

	public static TAPI For<TAPI>(RestFetchOptions options) {
		throw new NotImplementedException();
	}
}