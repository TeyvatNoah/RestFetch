using RestFetch;

namespace RestFetch.Examples;

#pragma warning disable CA1715 // Identifiers should have correct prefix
public sealed partial class Foo {
	public partial class Bar {
		[BaseAddress("http://127.0.0.1")]
		public sealed partial class C4<T>: IRestFetchApi<C4<T>> {
			[Get("/user")]
			public partial Task<T> GetUserById([Query]int id);

			[Get("/user")]
			public partial Task<U> GetUserByName<U>([Query]string name) where U: IEnumerable<string>;
		}

		[BaseAddress("http://127.0.0.1")]
		public partial struct S4<T>: IRestFetchApi<S4<T>> {
			[Get("/user")]
			public partial Task<T> GetUserById([Query]int id);
		}

		[BaseAddress("http://127.0.0.1")]
		public partial interface I4<T>: IRestFetchApi<I4<T>.Impl> {
			[Get("/user")]
			Task<T> GetUserById([Query]int id);

			[Get("/user")]
			Task<U> GetUserByName<U>([Query]string name) where U: IEnumerable<string>;
		}
	}
}

#pragma warning restore CA1715 // Identifiers should have correct prefix