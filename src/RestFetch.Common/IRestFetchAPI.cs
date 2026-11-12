using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace RestFetch;

public interface IRestFetchApi<out TSelf, THttpClient, TResponse>
	where TSelf: notnull
	where THttpClient: IDisposable
	where TResponse: IDisposable {
	static abstract TSelf Create();
	static abstract TSelf Create(string baseAddress);
	static abstract TSelf Create(IHttpClientProxy<THttpClient, TResponse> client);
	static abstract TSelf Create(bool sharedClient, IHttpClientProxy<THttpClient, TResponse> client);

	TSelf AddRetry(RetryOptions<TResponse>? options);

	TSelf AddRetry(RetryStrategyOptions<TResponse>? options);

	TSelf AddRetryFor(string methodName, RetryOptions<TResponse>? options);

	TSelf AddRetryFor(string methodName, RetryStrategyOptions<TResponse>? options);

	TSelf AddTimeout(TimeoutOptions? options);

	TSelf AddTimeout(TimeoutStrategyOptions? options);

	TSelf AddTimeoutFor(string methodName, TimeoutOptions? options);

	TSelf AddTimeoutFor(string methodName, TimeoutStrategyOptions? options);

	// TSelf AddResiliencePipeline(ResiliencePipeline pipeline);
	TSelf AddResiliencePipeline(ResiliencePipeline<TResponse> pipeline);

	// TSelf AddResiliencePipelineFor(string methodName, ResiliencePipeline pipeline);

	TSelf AddResiliencePipelineFor(string methodName, ResiliencePipeline<TResponse> pipeline);

	TSelf Build();
}

public interface IRestFetchApi<TSelf>: IRestFetchApi<TSelf, HttpClient, HttpResponseMessage> where TSelf: notnull;