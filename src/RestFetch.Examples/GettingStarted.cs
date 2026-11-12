namespace RestFetch.Examples;

[BaseAddress("http://127.0.0.1")]
public sealed partial class C1: IRestFetchApi<C1> {
	[Get("/user")]
	public partial Task<User> GetUserById([Query]int id);
}

[BaseAddress("http://127.0.0.1")]
public partial struct S1: IRestFetchApi<S1> {
	[Get("/user")]
	public partial Task<User> GetUserById([Query]int id);
}

[BaseAddress("http://127.0.0.1")]
public partial interface I1: IRestFetchApi<I1.Impl> {
	[Get("/user")]
	Task<User> GetUserById([Query]int id);
}

public static class GettingStarted {
	public static async Task Main() {
		using var client = RestFetchService.For<I1.Impl>().Build();
		var user = await client.GetUserById(114514);
		Console.WriteLine(user.Name);
	}
}