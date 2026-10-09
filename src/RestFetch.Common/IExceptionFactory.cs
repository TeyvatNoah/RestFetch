namespace RestFetch;

public interface IExceptionFactory<TResponse> where TResponse: IDisposable {
	static abstract ValueTask<Exception?> CreateException(TResponse resp, ref readonly APIContext context, CancellationToken token);
}

public interface IExceptionFactory: IExceptionFactory<HttpResponseMessage>;