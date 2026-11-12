namespace RestFetch.Examples;

[AfterResponse<AfterResponseHk>(ConfigureAwait = true)]
public sealed partial class C15: IRestFetchApi<C15> {
	[Get("/user")]
	public partial Task GetUserById([Query]int id);
}

public sealed class AfterResponseHk : IAfterResponseHook {
	public static Task AfterResponse(HttpResponseMessage resp, ApiContext context, ResourceManager resourceManager, CancellationToken token) {
		Console.WriteLine("After response");
		return Task.CompletedTask;
	}
}

public static class AfterResponseExample {
	public static async Task Main() {
		using var client = RestFetchService.For<C15>("http://127.0.0.1").Build();
		await client.GetUserById(114514);
		Console.WriteLine("completed.");
	}
}
