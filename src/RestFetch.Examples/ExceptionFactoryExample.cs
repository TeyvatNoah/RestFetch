namespace RestFetch.Examples;

[ExceptionFactory<ExceptionHk>(ConfigureAwait = true)]
public sealed partial class C16: IRestFetchApi<C16> {
	[Get("/user")]
	public partial Task GetUserById([Query]int id);

	[Get("/user")]
	public partial Task GetUserByName([Query]string name);
}

public sealed class ExceptionHk: IExceptionFactory {
	public static ValueTask<Exception?> CreateException(HttpResponseMessage resp, ApiContext context, CancellationToken token) {
		Console.WriteLine("Exception hook");
		return CreateExceptionInternal(context.MethodName, context.Id, resp, token);

		static async ValueTask<Exception?> CreateExceptionInternal(string methodName, RequestId id, HttpResponseMessage resp, CancellationToken token) {
			if (resp.IsSuccessStatusCode) {
				return null;
			} else {
				var body = await resp.Content.ReadAsStringAsync(token).ConfigureAwait(false);
				return new RestFetchApiException(methodName, id, resp.Version, resp.ReasonPhrase) {
					ResponseBody = body
				};
			}
		}
	}
}

public static class ExceptionFactoryExample {
	public static async Task Main() {
		using var client = RestFetchService.For<C16>("http://127.0.0.1").Build();
		await client.GetUserById(114514);
		Console.WriteLine("completed.");
	}
}
