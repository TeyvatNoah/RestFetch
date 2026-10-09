namespace RestFetch;

public interface IOnSuccess {
	static abstract Task OnSuccess<TRequestBodyContent>(TRequestBodyContent? body, ref readonly APIContext context, CancellationToken token);
}