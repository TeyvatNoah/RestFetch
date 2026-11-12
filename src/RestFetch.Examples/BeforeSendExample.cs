namespace RestFetch.Examples;

[BeforeSend<BeforeSendHk>(ConfigureAwait = false)]
public sealed partial class C14: IRestFetchApi<C14> {
	[Get("/user")]
	public partial Task GetUserById([Query]int id);
}

public sealed class BeforeSendHk: IBeforeSendHook {
	public static Task BeforeSend(HttpContent? body, ApiContext context, ResourceManager resourceManager, CancellationToken token) {
		Console.WriteLine("Before send");
		return Task.CompletedTask;
	}
}

public static class BeforeSendExample {
	public static async Task Main() {
		using var client = RestFetchService.For<C14>("http://127.0.0.1").Build();
		await client.GetUserById(114514);
		Console.WriteLine("completed.");
	}
}
