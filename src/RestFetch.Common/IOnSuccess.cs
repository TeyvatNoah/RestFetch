namespace RestFetch;

public interface IOnSuccess<TURL, TDisposer, TResponse, TProxy>
	where TURL: IURL
	where TDisposer: IDisposer<TURL>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TURL, TDisposer, TResponse, TProxy> {
	static abstract Task OnSuccess<TRequestBodyContent>(TRequestBodyContent? body, ref readonly APIContext<TURL, TDisposer, TResponse, TProxy> context, CancellationToken token);
}

public interface IOnSuccess: IOnSuccess<URLProxy, URLDisposer, HttpResponseMessage, HttpClientProxy>;