using RestFetch;
namespace RestFetch.Examples;

[BaseAddress("https://github.com")]
public sealed partial class C6: IRestFetchApi<C6> {
	// [BaseAddress("http://127.0.0.1")]
	[Get("/user")]
	public partial Task<User> GetUserById([Query]int id);
}

// [BaseAddress("https://github.com")]
// public partial struct S6: IRestFetchApi<S6> {
// 	[BaseAddress("http://127.0.0.1")]
// 	[Get("/user")]
// 	public partial Task<User> GetUserById([Query]int id);
// }

[BaseAddress("https://github.com")]
public partial interface I6: IRestFetchApi<I6.Impl> {
	[BaseAddress("http://127.0.0.1")]
	[Get("/user")]
	Task<User> GetUserById([Query]int id);
}

public static class BaseAddressExample {
	public static async Task Main() {
		using var client = RestFetchService.For<C6>().Build();
		var user = await client.GetUserById(114514);
		// var client = RestFetchService.For<I6.Impl>("https://google.com");
		Console.WriteLine(user.Name);
	}
}