namespace RestFetch;

public interface IBeforeSendHook<in TRequestBodyContent> {
	static abstract Task BeforeSend(TRequestBodyContent? body, ref readonly ApiContext context, CancellationToken token);
}

public interface IBeforeSendHook: IBeforeSendHook<HttpContent>;