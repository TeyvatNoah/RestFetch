namespace RestFetch;

public interface IAfterResponseHook<in TResponse> where TResponse: IDisposable {
	static abstract Task AfterResponse(TResponse resp, ref readonly ApiContext context, CancellationToken token);
}

public interface IAfterResponseHook: IAfterResponseHook<HttpResponseMessage>;