namespace RestFetch;

public interface IHttpClientProxy<TResponse, THttpClientProxy>: IDisposable
	where TResponse: IDisposable
	where THttpClientProxy: IHttpClientProxy<TResponse, THttpClientProxy> {
	string BaseAddress { get; set; }

	Task<TResponse> SendAsync<TRequestBodyContent>(ref readonly APIContext context, Dictionary<string, string>? headers, TRequestBodyContent? body, CancellationToken token);

	static abstract THttpClientProxy Create(string baseAddress);
}

public interface IHttpClientProxy: IHttpClientProxy<HttpResponseMessage, HttpClientProxy>;