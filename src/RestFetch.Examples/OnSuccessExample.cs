namespace RestFetch.Examples;

[OnSuccess<OnSuccessHk>(ConfigureAwait = true)]
public sealed partial class C17: IRestFetchApi<C17> {
	[Get("/user")]
	public partial Task GetUserById([Query]int id);
}

public sealed class OnSuccessHk: IOnSuccess {
	public static Task OnSuccess<TRequestBodyContent>(TRequestBodyContent? body, ApiContext context, CancellationToken token) {
		Console.WriteLine("Success");
		return Task.CompletedTask;
	}
}

public static class OnSuccessExample {
	public static async Task Main() {
		using var client = RestFetchService.For<C17>("http://127.0.0.1").Build();
		await client.GetUserById(114514);
		Console.WriteLine("completed.");
	}
}
