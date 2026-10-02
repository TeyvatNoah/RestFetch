namespace RestFetch;

public interface IExceptionFactory<TURL, TDisposer, TResponse, TProxy>
	where TURL: IURL
	where TDisposer: IDisposer<TURL>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TURL, TDisposer, TResponse, TProxy> {
	static abstract ValueTask<Exception?> CreateException(TResponse resp, ref readonly APIContext<TURL, TDisposer, TResponse, TProxy> context, CancellationToken token);
}

public interface IExceptionFactory: IExceptionFactory<URLProxy, URLDisposer, HttpResponseMessage, HttpClientProxy>;