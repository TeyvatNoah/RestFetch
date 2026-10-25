namespace RestFetch;

public readonly record struct ApiMinimalContext(
	RequestId Id,
	string Url
);