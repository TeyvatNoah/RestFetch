namespace RestFetch;

public interface IHttpClientProxy<TURL, TDisposer, TResponse, TProxy>: IDisposable
	where TURL: IURL
	where TDisposer: IDisposer<TURL>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TURL, TDisposer, TResponse, TProxy> {
	string BaseAddress { get; set; }

	Task<TResponse> SendAsync<TRequestBodyContent>(ref readonly APIContext<TURL, TDisposer, TResponse, TProxy> context, Dictionary<string, string>? headers, TRequestBodyContent? body, CancellationToken token);

	static abstract TProxy Create(string baseAddress);
}

public interface IHttpClientProxy: IHttpClientProxy<URLProxy, URLDisposer, HttpResponseMessage, HttpClientProxy>;