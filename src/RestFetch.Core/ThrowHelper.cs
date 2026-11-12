using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RestFetch;

public static class ThrowHelper {
	[DoesNotReturn]
	public static void ThrowMethodNameNotFound(string typeName, string methodName, string argName) => throw new ArgumentException($"The method {methodName} cannot be found in type {typeName}.", argName);

	[Conditional("DEBUG")]
	public static void ThrowBaseAddressIsNullOrEmpty(string? baseAddress) {
		if (string.IsNullOrEmpty(baseAddress)) {
			throw new ArgumentException("The BaseAddress is null or empty.");
		}
	}
}