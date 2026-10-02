namespace RestFetch;

public interface IOnError<TURL, TDisposer, TResponse, TProxy>
	where TURL: IURL
	where TDisposer: IDisposer<TURL>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TURL, TDisposer, TResponse, TProxy> {
	static abstract Task OnError(Exception? e, ref readonly APIContext<TURL, TDisposer, TResponse, TProxy> context, CancellationToken token);
}

public interface IOnError: IOnError<URLProxy, URLDisposer, HttpResponseMessage, HttpClientProxy>;