using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace RestFetch;

public interface IRestFetchApiService<out TSelf, TResponse>
	where TSelf: class, IRestFetchApiService<TSelf, TResponse>
	where TResponse: IDisposable {
	TSelf AddRetry(RetryOptions<TResponse>? options);

	TSelf AddRetry(RetryStrategyOptions<TResponse>? options);

	TSelf AddRetryFor(string methodName, RetryOptions<TResponse>? options);

	TSelf AddRetryFor(string methodName, RetryStrategyOptions<TResponse>? options);

	TSelf AddTimeout(TimeoutOptions? options);

	TSelf AddTimeout(TimeoutStrategyOptions? options);

	TSelf AddTimeoutFor(string methodName, TimeoutOptions? options);

	TSelf AddTimeoutFor(string methodName, TimeoutStrategyOptions? options);

	TSelf AddResiliencePipeline(ResiliencePipeline<TResponse> pipeline);

	TSelf AddResiliencePipelineFor(string methodName, ResiliencePipeline<TResponse> pipeline);

	TSelf Build();
}

public interface IRestFetchApiService<TSelf>: IRestFetchApiService<TSelf, HttpResponseMessage> where TSelf: class, IRestFetchApiService<TSelf>;

public interface IRestFetchApi<out TSelf, TResponse>: IRestFetchApiService<TSelf, TResponse>
	where TSelf: class, IRestFetchApi<TSelf, TResponse>
	where TResponse: IDisposable {

	static abstract TSelf Create();

	static abstract TSelf Create(string baseAddress);

	static abstract TSelf Create(IHttpClientProxyService<TResponse> client);

	static abstract TSelf Create(bool sharedClient, IHttpClientProxyService<TResponse> client);
}

public interface IRestFetchApi<TSelf>: IRestFetchApi<TSelf, HttpResponseMessage> where TSelf: class, IRestFetchApi<TSelf>;