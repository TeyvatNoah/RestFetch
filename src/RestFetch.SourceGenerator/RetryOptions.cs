
namespace RestFetch.SourceGenerator;

public sealed record RetryOptions(
	bool Ignore = false,
	bool AllowUnsafeHttpMethod = false,
	bool AllowUnsafeRequestBody = false,
	int? MaxRetryAttempts = null,
	DelayBackoffType? BackoffType = null,
	int? Delay = 2,
	int? MaxDelay = null,
	bool? UseJitter = null,
	string Predicate = "global::RestFetch.DefaultRetryPredicate",
	string OnRetry = "global::RestFetch.DefaultOnRetry",
	EquatableArray<DiagnosticInfo>? Diagnostics = null
);