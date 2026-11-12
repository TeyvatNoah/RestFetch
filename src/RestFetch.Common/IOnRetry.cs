using Polly.Retry;

namespace RestFetch;

public interface IOnRetry<TResponse> where TResponse: IDisposable {
	// [MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual ValueTask OnRetry(OnRetryArguments<TResponse> pollyOnRetryArgs) => ValueTask.CompletedTask;
}

public interface IOnRetry: IOnRetry<HttpResponseMessage>;