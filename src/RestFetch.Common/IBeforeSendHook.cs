namespace RestFetch;

public interface IBeforeSendHook<TRequestBodyContent> {
	static abstract Task BeforeSend(TRequestBodyContent? body, ref readonly APIContext context, CancellationToken token);
}

public interface IBeforeSendHook: IBeforeSendHook<HttpContent>;