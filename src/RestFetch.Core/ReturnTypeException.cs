using System.Diagnostics.CodeAnalysis;

namespace RestFetch;
public sealed class ReturnTypeException(string msg): Exception(msg) {

	[DoesNotReturn]
	public static void Throw() => throw new ReturnTypeException("The return type is expected to be non-nullable, but the deserializer can only return nullable types. Please use Task<T?>/ApiResponse<T?> instead of Task<T>/ApiResponse<T>");
}