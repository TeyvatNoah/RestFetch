using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace RestFetch;

public static class ThrowHelper {
		[DoesNotReturn]
		public static void Throw(Exception e) => ExceptionDispatchInfo.Capture(e).Throw();
}