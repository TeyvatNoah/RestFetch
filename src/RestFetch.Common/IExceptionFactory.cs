namespace RestFetch;

public interface IExceptionFactory<TResponse, TProxy>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TResponse, TProxy> {
	static abstract ValueTask<Exception?> CreateException(TResponse resp, ref readonly APIContext<TResponse, TProxy> context, CancellationToken token);
}

public interface IExceptionFactory: IExceptionFactory<HttpResponseMessage, HttpClientProxy>;