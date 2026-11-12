namespace RestFetch;

public sealed class UnsafeRetryHTTPMethodException(HttpMethod httpMethod, string msg): Exception(msg) {
	public readonly HttpMethod HttpMethod = httpMethod;
}