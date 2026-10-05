using System.Diagnostics.CodeAnalysis;

namespace RestFetch;
public sealed class ResponseContentNullException(string msg): Exception(msg) {

	[DoesNotReturn]
	public static void Throw() => throw new ResponseContentNullException("Response content is null while return type is not nullable.");
}