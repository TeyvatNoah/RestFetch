namespace RestFetch;

public interface IBeforeSendHook<in TRequestBodyContent> {
	static abstract Task BeforeSend(TRequestBodyContent? body, ApiContext context, ResourceManager resourceManager, CancellationToken token);
}

public interface IBeforeSendHook: IBeforeSendHook<HttpContent>;