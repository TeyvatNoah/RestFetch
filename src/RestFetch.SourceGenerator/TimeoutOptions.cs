namespace RestFetch.SourceGenerator;

public sealed record TimeoutOptions(
	bool Ignore = false,
	int? AttemptTimeout = null,
	int? TotalTimeout = null
);