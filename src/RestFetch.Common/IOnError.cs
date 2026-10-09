namespace RestFetch;

public interface IOnError {
	static abstract Task OnError(Exception? e, ref readonly APIContext context, CancellationToken token);
}