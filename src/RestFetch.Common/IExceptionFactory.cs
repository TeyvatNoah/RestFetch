namespace RestFetch;

public interface IExceptionFactory<in TResponse> where TResponse: IDisposable {
	static abstract ValueTask<Exception?> CreateException(TResponse resp, ref readonly ApiContext context, CancellationToken token);
}

public interface IExceptionFactory: IExceptionFactory<HttpResponseMessage>;