using RestFetch;
using RestFetch.Examples;

// 无namespace

#pragma warning disable CA1050 // Declare types in namespaces
[BaseAddress("http://127.0.0.1")]
public sealed partial class C2: IRestFetchApi<C2> {
		[Get("/user")]
	public partial Task<User> GetUserById([Query]int id);
}

[BaseAddress("http://127.0.0.1")]
public partial struct S2: IRestFetchApi<S2> {
	[Get("/user")]
	public partial Task<User> GetUserById([Query]int id);
}

[BaseAddress("http://127.0.0.1")]
public partial interface I2: IRestFetchApi<I2.Impl> {
	[Get("/user")]
	Task<User> GetUserById([Query]int id);
}

#pragma warning restore CA1050 // Declare types in namespaces