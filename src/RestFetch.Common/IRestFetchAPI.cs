namespace RestFetch;

public interface IRestFetchAPI<TSelf, TResponse, THttpClientProxy>
	where TResponse: IDisposable
	where THttpClientProxy: IHttpClientProxy<TResponse, THttpClientProxy> {
	static abstract TSelf Create(string baseAddress);
	static abstract TSelf Create(IHttpClientProxy<TResponse, THttpClientProxy> client);
	static abstract TSelf Create(bool sharedClient, IHttpClientProxy<TResponse, THttpClientProxy> client);
}

public interface IRestFetchAPI<TSelf>: IRestFetchAPI<TSelf, HttpResponseMessage, HttpClientProxy>;