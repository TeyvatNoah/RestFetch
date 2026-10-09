namespace RestFetch;

public interface IAfterResponseHook<TResponse> where TResponse: IDisposable {
	static abstract Task AfterResponse(TResponse resp, ref readonly APIContext context, CancellationToken token);
}

public interface IAfterResponseHook: IAfterResponseHook<HttpResponseMessage>;