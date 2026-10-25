namespace RestFetch;

public interface IHttpClientProxy<TResponse, THttpClientProxy>: IDisposable
	where TResponse: IDisposable
	where THttpClientProxy: IHttpClientProxy<TResponse, THttpClientProxy> {
	string BaseAddress { get; set; }

	// 会被放入ApiContext中传递
	object? Options { get; init; }

	Task<TResponse> SendAsync<TRequestBodyContent>(ref readonly ApiContext context, Dictionary<string, string>? headers, TRequestBodyContent? body, CancellationToken token);

	static abstract THttpClientProxy Create(string baseAddress);
}

// public interface IHttpClientProxy: IHttpClientProxy<HttpResponseMessage, HttpClientProxy>;