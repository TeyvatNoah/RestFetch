using RestFetch;
namespace RestFetch.Examples;

public sealed partial class C9: IRestFetchApi<C9> {
	[Get("/user/{id}/{id}")]
	public partial Task<User> GetUserById0([Param]int id);

	// [Get("/user/{id}")]
	// public partial Task<User> GetUserById0([Param]int uid); // error

	[Get("/user/{uid}")]
	public partial Task<User> GetUserById1([Param("uid")]int id);

	[Get("/user/{user.Id}")]
	public partial Task<User> GetUserById2([Param]User user);

	[Get("/user/{id}")]
	public partial Task<User> GetUserById3([Param(UrlEncode = false)]int id);

	[Get("/user/{id}")]
	public partial Task<User> GetUserById4([Param<ParamSerializer>]int id);

	[Get("/user/{id}")]
	public partial Task<User> GetUserById5([Param<ParamSerializer>("id")]int uid);
}

public static class ParamExample {
	public static async Task Main() {
		// var client = RestFetchService.For<C9>();
		using var client = RestFetchService.For<C9>("http://127.0.0.1").Build();
		var user = await client.GetUserById2(new(
			Id: 114514,
			Age: 24,
			Name: "senpai"
		));
		Console.WriteLine(user.Name);
	}
}