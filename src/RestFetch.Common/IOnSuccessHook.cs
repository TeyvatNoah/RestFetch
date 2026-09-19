namespace RestFetch;

public interface IOnSuccessHook<TURL, TDisposer, TResponse, TProxy>
	where TURL: IURL
	where TDisposer: IDisposer<TURL>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TURL, TDisposer, TResponse, TProxy> {
	static abstract Task OnSuccess<TRequestBodyContent>(TRequestBodyContent? body, ref readonly APIInfo<TURL, TDisposer, TResponse, TProxy> context, CancellationToken token);
}

public interface IOnSuccessHook: IOnSuccessHook<URLProxy, URLDisposer, HttpResponseMessage, HttpClientProxy>;