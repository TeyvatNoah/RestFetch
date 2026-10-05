namespace RestFetch;

public interface IAfterResponseHook<TResponse, TProxy>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TResponse, TProxy> {
	static abstract Task AfterResponse(TResponse resp, ref readonly APIContext<TResponse, TProxy> context, CancellationToken token);
}

public interface IAfterResponseHook: IAfterResponseHook<HttpResponseMessage, HttpClientProxy>;