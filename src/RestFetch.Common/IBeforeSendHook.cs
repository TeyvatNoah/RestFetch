namespace RestFetch;

public interface IBeforeSendHook<TURL, TDisposer, TRequestBodyContent, TResponse, TProxy>
	where TURL: IURL
	where TDisposer: IDisposer<TURL>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TURL, TDisposer, TResponse, TProxy> {
	static abstract Task BeforeSend(TRequestBodyContent? body, ref readonly APIContext<TURL, TDisposer, TResponse, TProxy> context, CancellationToken token);
}

public interface IBeforeSendHook: IBeforeSendHook<URLProxy, URLDisposer, HttpContent, HttpResponseMessage, HttpClientProxy>;