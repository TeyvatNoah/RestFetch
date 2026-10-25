namespace RestFetch;

public interface IRestFetchApi<TSelf, TResponse, THttpClientProxy>
	where TSelf: allows ref struct
	where TResponse: IDisposable
	where THttpClientProxy: IHttpClientProxy<TResponse, THttpClientProxy> {
	static abstract TSelf Create();
	static abstract TSelf Create(string baseAddress);
	static abstract TSelf Create(IHttpClientProxy<TResponse, THttpClientProxy> client);
	static abstract TSelf Create(bool sharedClient, IHttpClientProxy<TResponse, THttpClientProxy> client);
}

public interface IRestFetchApi<TSelf>: IRestFetchApi<TSelf, HttpResponseMessage, HttpClientProxy> where TSelf: allows ref struct;