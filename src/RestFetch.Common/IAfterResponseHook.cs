namespace RestFetch;

public interface IAfterResponseHook<in TResponse> where TResponse: IDisposable {
	static abstract Task AfterResponse(TResponse resp, ApiContext context, ResourceManager resourceManager, CancellationToken token);
}

public interface IAfterResponseHook: IAfterResponseHook<HttpResponseMessage>;