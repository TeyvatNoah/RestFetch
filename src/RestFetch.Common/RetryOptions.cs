using Polly;
using Polly.Retry;

namespace RestFetch;

public sealed record RetryOptions<TResponse>(
	bool Ignore = false,
	bool AllowUnsafeHttpMethod = false,
	bool AllowUnsafeRequestBody = false,
	int? MaxRetryAttempts = null,
	DelayBackoffType BackoffType = DelayBackoffType.Constant,
	int? Delay = 2,
	int? MaxDelay = null,
	bool? UseJitter = null,
	// NOTE: 签名和IRetryPredicate.CreateShouldHandle保持一致
	Func<object?, RetryOptions<TResponse>?, HttpMethod, Func<RetryPredicateArguments<TResponse>, ValueTask<bool>>>? CreateShouldHandle = null,
	// NOTE: 签名和IRetryPredicate.ValidateRequestBody保持一致
	Action<object?, RetryOptions<TResponse>?, ApiContext>? ValidateRequestBody = null,
	Func<OnRetryArguments<TResponse>, ValueTask>? OnRetry = null
) where TResponse: IDisposable;