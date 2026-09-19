using System.Diagnostics.CodeAnalysis;

namespace RestFetch;

public static class ThrowHelper {
		[DoesNotReturn]
		public static void Throw(Exception e) => throw e;
}