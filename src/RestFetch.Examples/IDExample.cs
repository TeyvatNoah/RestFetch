namespace RestFetch.Examples;

[IdGenerator<IDGen>]
public sealed partial class C13: IRestFetchApi<C13> {
	[Get("/user")]
	public partial Task GetUserById([Query]int id);

	[IdGenerator<RequestIdGenerator>]
	[Get("/user")]
	public partial Task GetUserByName([Query]string name);
}

public sealed class IDGen: IRequestIdGenerator {
	public static RequestId NextId() => Guid.CreateVersion7().ToString();
}

public static class IDExample {
	public static async Task Main() {
		using var client = RestFetchService.For<C13>("http://127.0.0.1").Build();
		await client.GetUserById(114514);
		Console.WriteLine("completed.");
	}
}
