namespace RestFetch.Examples;

[OnError<OnErrorHk>]
public sealed partial class C18: IRestFetchApi<C18> {
	[Get("/user")]
	public partial Task GetUserById([Query]int id);
}

public sealed class OnErrorHk: IOnError {
	public static Task OnError(Exception? e, ApiContext context, CancellationToken token) {
		Console.WriteLine("Error");
		return Task.CompletedTask;
	}
}

public static class OnErrorExample {
	public static async Task Main() {
		using var client = RestFetchService.For<C18>("http://127.0.0.1").Build();
		await client.GetUserById(114514);
		Console.WriteLine("completed.");
	}
}
