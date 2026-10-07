using System.Runtime.CompilerServices;

namespace RestFetch;

public readonly struct ReturnTypeInfo(
	string? responseTypeName,
	string minimallyQualifiedName,
	ReturnTypeKind returnTypeKind,
	ReturnTypeWrapperKind returnTypeWrapperKind
): IEquatable<ReturnTypeInfo> {
	public readonly string? ResponseTypeName = responseTypeName;
	public readonly string MinimallyQualifiedName = minimallyQualifiedName;

	public readonly ReturnTypeWrapperKind ReturnTypeWrapperKind = returnTypeWrapperKind;

	public readonly ReturnTypeKind ReturnTypeKind = returnTypeKind;

	public readonly override string ToString() => MinimallyQualifiedName;

	public readonly override bool Equals(object? obj) => obj is ReturnTypeInfo o && o == this;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(ReturnTypeInfo obj) => this == obj;

	public readonly override int GetHashCode() => HashCode.Combine(ResponseTypeName, MinimallyQualifiedName, ReturnTypeKind, ReturnTypeWrapperKind);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ReturnTypeInfo left, ReturnTypeInfo right) => left.ResponseTypeName == right.ResponseTypeName
		&& left.MinimallyQualifiedName == right.MinimallyQualifiedName
		&& left.ReturnTypeWrapperKind == right.ReturnTypeWrapperKind
		&& left.ReturnTypeKind == right.ReturnTypeKind;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ReturnTypeInfo left, ReturnTypeInfo right) => !(left == right);
}