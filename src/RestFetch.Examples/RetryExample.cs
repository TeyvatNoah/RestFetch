using Polly.Retry;

namespace RestFetch.Examples;

[Retry(4)]
[BaseAddress("http://127.0.0.1")]
public sealed partial class C20: IRestFetchApi<C20> {
	[Get("/user")]
	// [Retry(2)]
	public partial Task<User> GetUserById([Query]int id);

	[Get("/user")]
	[Retry(8, OnRetry = typeof(OnRetryTest), Predicate = typeof(PredicateTest))]
	public partial Task<User> GetUserByName([Query]int id);

	[Get("/user")]
	[Timeout(5)]
	public partial Task<User> GetUserById2([Query]int id);

	[Get("/user")]
	[Retry(10)]
	[Timeout(AttemptTimeout = 2, TotalTimeout = 10)]
	public partial Task<User> GetUserById3([Query]int id);
}

public sealed class OnRetryTest: IOnRetry {
	public static ValueTask OnRetry(OnRetryArguments<HttpResponseMessage> pollyOnRetryArgs) {
		if (pollyOnRetryArgs.Context.Properties.TryGetValue(C20.ApiContextKey, out var context)) {
			Console.WriteLine(context.MethodName);
		}
		return ValueTask.CompletedTask;
	}
}

public sealed class PredicateTest: IRetryPredicate {
	public static Func<RetryPredicateArguments<HttpResponseMessage>, ValueTask<bool>> CreateShouldHandle<TRequestBody>(TRequestBody? body, RetryOptions<HttpResponseMessage>? retryOptions, HttpMethod httpMethod) {
		Console.WriteLine("should handle");
		return static args => ValueTask.FromResult(true);
	}
	public static void ValidateRequestBody<TRequestBody>(TRequestBody? body, RetryOptions<HttpResponseMessage>? retryOptions, ApiContext context) {
		Console.WriteLine("validate body");
	}
}


public static class RetryExample {
	public static async Task Main() {
		// using var client = RestFetchService.For<C20>().AddRetryFor(nameof(C20.GetUserByName), new RetryOptions(
		// 	Ignore: true
		// )).Build();

		using var client = RestFetchService.For<C20>();
		client.AddRetryFor(nameof(C20.GetUserByName), new RetryOptions<HttpResponseMessage>(
			Ignore: true
		))
		.AddRetry(new RetryOptions<HttpResponseMessage>(
			MaxRetryAttempts: 6
		))
		.AddRetryFor(nameof(C20.GetUserByName), new RetryOptions<HttpResponseMessage>(
			MaxRetryAttempts: 2,
			CreateShouldHandle: static (body, options, httpMethod) => {
				Console.WriteLine(httpMethod);
				return args => {
					if (args.Context.Properties.TryGetValue(C20.ApiContextKey, out var ctx)) {
						Console.WriteLine(ctx.MethodName);
					}
					return ValueTask.FromResult(true);
				};
			},
			OnRetry: static args => {
				Console.WriteLine("onretry");
				return ValueTask.CompletedTask;
			},
			ValidateRequestBody: static (body, options, ctx) => {
				Console.WriteLine("validate");
			}
		));
		client.Build();
		// var user = await client.GetUserById(114514);
		var user = await client.GetUserById3(114514);
		Console.WriteLine(user.Name);
	}
}