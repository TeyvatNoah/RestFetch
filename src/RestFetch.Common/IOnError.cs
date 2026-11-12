namespace RestFetch;

public interface IOnError {
	static abstract Task OnError(Exception? e, ApiContext context, CancellationToken token);
}