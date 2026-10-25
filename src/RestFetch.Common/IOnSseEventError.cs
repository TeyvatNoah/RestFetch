namespace RestFetch;

public interface IOnSseEventError {
	static abstract Task<bool> OnSseEventError(Exception exp, ApiMinimalContext context);
}