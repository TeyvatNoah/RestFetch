using RestFetch;

namespace RestFetch.Examples;

public sealed partial class Foo {
	public partial class Bar {
		[BaseAddress("http://127.0.0.1")]
		public sealed partial class C3: IRestFetchApi<C3> {
			[Get("/user")]
			public partial Task<User> GetUserById([Query]int id);
		}

		[BaseAddress("http://127.0.0.1")]
		public partial struct S3: IRestFetchApi<S3> {
			[Get("/user")]
			public partial Task<User> GetUserById([Query]int id);
		}

		[BaseAddress("http://127.0.0.1")]
		public partial interface I3: IRestFetchApi<I3.Impl> {
			[Get("/user")]
			Task<User> GetUserById([Query]int id);
		}
	}
}
