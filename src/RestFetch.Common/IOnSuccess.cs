namespace RestFetch;

public interface IOnSuccess<TResponse, TProxy>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TResponse, TProxy> {
	static abstract Task OnSuccess<TRequestBodyContent>(TRequestBodyContent? body, ref readonly APIContext<TResponse, TProxy> context, CancellationToken token);
}

public interface IOnSuccess: IOnSuccess<HttpResponseMessage, HttpClientProxy>;