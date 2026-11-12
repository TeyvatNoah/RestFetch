using Polly.Retry;

namespace RestFetch;

public interface IRetryPredicate<TResponse> where TResponse: IDisposable {
	// NOTE: 此方法只会在pipeline构建时执行一次,之后请求方法调用则不会执行
	// RetryOptions, pipeline都是和请求方法一一对应的方法级配置,所以多次执行请求方法也是共享相同的配置
	// 即一次性配置(可能是运行时也可能是编译时),所以作为实现者只应当在里面校验一次性配置的内容比如HTTP Method,
	// 这里提供了body参数但只应当用来校验body的声明类型而不要校验如body字段等每次执行可能不一样的动态内容
	// 返回的lambda建议static,避免捕获第一次执行时的内容,更不要捕获body参数
	// TODO pipeline和RetryOptions是按方法名缓存的,这导致请求方法重载之间和泛型请求方法的多个单态化之间会共享同一份配置
	// 并且泛型请求方法会导致CreateShouldHandle只在第一个单态化的第一次调用的时候执行一次,此后其他的单态化不会执行
	// 解决方法只能是将封闭泛型完全限定名作为缓存key而不是简单使用方法名,但是这样用户体验就很差了
	static abstract Func<RetryPredicateArguments<TResponse>, ValueTask<bool>> CreateShouldHandle<TRequestBody>(TRequestBody? body, RetryOptions<TResponse>? retryOptions, HttpMethod httpMethod);

	// NOTE: 此方法会在每次请求方法执行时都执行,用于检查如body这样的动态内容是否可以retry
	static abstract void ValidateRequestBody<TRequestBody>(TRequestBody? body, RetryOptions<TResponse>? retryOptions, ApiContext context);
}

public interface IRetryPredicate: IRetryPredicate<HttpResponseMessage>;