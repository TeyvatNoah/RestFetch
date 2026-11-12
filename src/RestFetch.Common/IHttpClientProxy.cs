namespace RestFetch;

// NOTE: 仅仅为了解决带static abstract方法的interface无法作为泛型参数而存在
// 和IHttpClientProxy保持一致
public interface IHttpClientProxyService<TResponse>: IDisposable
	where TResponse: IDisposable {
	string? BaseAddress { get; set; }

	// 会被放入ApiContext中传递
	object? Options { get; init; }

	Task<TResponse> SendAsync<TRequestBodyContent>(TRequestBodyContent? body, ApiContext context, ResourceManager resourceManager, CancellationToken token);
}

public interface IHttpClientProxy<TSelf, TResponse>: IHttpClientProxyService<TResponse>
	where TResponse: IDisposable
	where TSelf: IHttpClientProxyService<TResponse> {

	static abstract TSelf Create(string? baseAddress);

	// static abstract IHttpClientProxyService<TResponse> Create(THttpClient client);
}

// public interface IHttpClientProxy: IHttpClientProxy<HttpResponseMessage, HttpClientProxy>;