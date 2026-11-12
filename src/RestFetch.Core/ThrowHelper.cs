using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace RestFetch;

public static class ThrowHelper {
	[DoesNotReturn]
	public static void ThrowMethodNameNotFound(string typeName, string methodName, string argName) => throw new ArgumentException($"The method {methodName} cannot be found in type {typeName}.", argName);
}