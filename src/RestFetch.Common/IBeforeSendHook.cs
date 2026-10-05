namespace RestFetch;

public interface IBeforeSendHook<TRequestBodyContent, TResponse, TProxy>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TResponse, TProxy> {
	static abstract Task BeforeSend(TRequestBodyContent? body, ref readonly APIContext<TResponse, TProxy> context, CancellationToken token);
}

public interface IBeforeSendHook: IBeforeSendHook<HttpContent, HttpResponseMessage, HttpClientProxy>;