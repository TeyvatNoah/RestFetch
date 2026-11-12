using RestFetch;
namespace RestFetch.Examples;

public sealed partial class C8: IRestFetchApi<C8> {
	[Metadata("Foo", "Bar")]
	[Metadata("Foo", "bbb")]
	[Metadata("Fooo", "Baar")]
	[Get("/user")]
	public partial Task<User> GetUserById([Query]int id, [Metadata("Foooo")]int? value);

}

public static class MetadataExample {
	public static async Task Main() {
		// var client = RestFetchService.For<C8>();
		using var client = RestFetchService.For<C8>("http://127.0.0.1").Build();
		var user = await client.GetUserById(114514, 123);
		Console.WriteLine(user.Name);
	}
}