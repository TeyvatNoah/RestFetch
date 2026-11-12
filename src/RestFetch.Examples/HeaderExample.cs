using RestFetch;
namespace RestFetch.Examples;

[Header("Foo", "Bar")]
[Header("Fooo", "Baar")]
public sealed partial class C7: IRestFetchApi<C7> {
	[Header("Foo", "bbb")]
	[Header("Fooo", null)]
	[Header("Foooo", "Baaar")]
	[Get("/user")]
	public partial Task<User> GetUserById([Query]int id);

	[Get("/user")]
	public partial Task<User> GetUserByName([Query]string name, [Header]IEnumerable<KeyValuePair<string, string>>? headers);

	[Get("/user")]
	public partial Task<User> GetUserByAge([Query]int age, [Header("Content-Length")]string length);
}

public static class HeaderExample {
	public static async Task Main() {
		// var client = RestFetchService.For<C7>();
		using var client = RestFetchService.For<C7>("http://127.0.0.1").Build();
		var user = await client.GetUserById(114514);
		Console.WriteLine(user.Name);
	}
}