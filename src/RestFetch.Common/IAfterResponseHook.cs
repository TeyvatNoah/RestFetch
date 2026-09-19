namespace RestFetch;

public interface IAfterResponseHook<TURL, TDisposer, TResponse, TProxy>
	where TURL: IURL
	where TDisposer: IDisposer<TURL>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TURL, TDisposer, TResponse, TProxy> {
	static abstract Task AfterResponse(TResponse? resp, ref readonly APIInfo<TURL, TDisposer, TResponse, TProxy> context, CancellationToken token);
}

public interface IAfterResponseHook: IAfterResponseHook<URLProxy, URLDisposer, HttpResponseMessage, HttpClientProxy>;