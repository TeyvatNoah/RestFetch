namespace RestFetch;

public interface IOnError<TResponse, TProxy>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TResponse, TProxy> {
	static abstract Task OnError(Exception? e, ref readonly APIContext<TResponse, TProxy> context, CancellationToken token);
}

public interface IOnError: IOnError<HttpResponseMessage, HttpClientProxy>;