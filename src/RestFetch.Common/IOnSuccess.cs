namespace RestFetch;

public interface IOnSuccess {
	static abstract Task OnSuccess<TResponseBody>(TResponseBody? body, ref readonly ApiContext context, CancellationToken token);
}