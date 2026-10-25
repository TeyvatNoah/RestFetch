namespace RestFetch;

public interface IOnError {
	static abstract Task OnError(Exception? e, ref readonly ApiContext context, CancellationToken token);
}