namespace RestFetch;

public interface IHttpClientProxy<in THttpClient, TResponse>: IDisposable
	where THttpClient: IDisposable
	where TResponse: IDisposable {
	string BaseAddress { get; set; }

	// 会被放入ApiContext中传递
	object? Options { get; init; }

	Task<TResponse> SendAsync<TRequestBodyContent>(TRequestBodyContent? body, ApiContext context, ResourceManager resourceManager, CancellationToken token);

	static abstract IHttpClientProxy<THttpClient, TResponse> Create(string baseAddress);

	static abstract IHttpClientProxy<THttpClient, TResponse> Create(THttpClient client);
}

// public interface IHttpClientProxy: IHttpClientProxy<HttpResponseMessage, HttpClientProxy>;