namespace RestFetch;

public interface IHttpClientProxy<TResponse, TProxy>: IDisposable
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TResponse, TProxy> {
	string BaseAddress { get; set; }

	Task<TResponse> SendAsync<TRequestBodyContent>(ref readonly APIContext<TResponse, TProxy> context, Dictionary<string, string>? headers, TRequestBodyContent? body, CancellationToken token);

	static abstract TProxy Create(string baseAddress);
}

public interface IHttpClientProxy: IHttpClientProxy<HttpResponseMessage, HttpClientProxy>;