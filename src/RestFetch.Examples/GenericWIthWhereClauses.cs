using RestFetch;

namespace RestFetch.Examples;
#pragma warning disable CA1715 // Identifiers should have correct prefix
public sealed partial class Foo {
	public partial class Bar {
		[BaseAddress("http://127.0.0.1")]
		public sealed partial class C5<T, U>: IRestFetchApi<C5<T, U>> where T: IEnumerable<U> {
			[Get("/user")]
			public partial Task<T> GetUserById([Query]int id);
		}

		[BaseAddress("http://127.0.0.1")]
		public partial struct S5<T, U>: IRestFetchApi<S5<T, U>> where T: IEnumerable<U> {
			[Get("/user")]
			public partial Task<T> GetUserById([Query]int id);
		}

		[BaseAddress("http://127.0.0.1")]
		public partial interface I5<T, U>: IRestFetchApi<I5<T, U>.Impl> where T: IEnumerable<U> {
			[Get("/user")]
			Task<T> GetUserById([Query]int id);
		}
	}
}

#pragma warning restore CA1715 // Identifiers should have correct prefix