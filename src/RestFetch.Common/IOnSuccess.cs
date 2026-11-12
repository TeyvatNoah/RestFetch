namespace RestFetch;

public interface IOnSuccess {
	static abstract Task OnSuccess<TResponseBody>(TResponseBody? body, ApiContext context, CancellationToken token);
}