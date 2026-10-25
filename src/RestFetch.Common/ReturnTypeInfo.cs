using System.Runtime.CompilerServices;

namespace RestFetch;

public readonly struct ReturnTypeInfo(
	Type returnType,
	bool isNullable,
	bool isValueType,
	bool skipNullableCheck,
	ReturnTypeKind returnTypeKind,
	ReturnTypeWrapperKind returnTypeWrapperKind
): IEquatable<ReturnTypeInfo> {
	public readonly Type ReturnType = returnType;

	public readonly bool Nullable = isNullable;

	public readonly bool ValueType = isValueType;

	public readonly bool SkipNullableCheck = skipNullableCheck;

	public readonly ReturnTypeWrapperKind ReturnTypeWrapperKind = returnTypeWrapperKind;

	public readonly ReturnTypeKind ReturnTypeKind = returnTypeKind;

	public readonly override string ToString() => ReturnType.FullName!;

	public readonly override bool Equals(object? obj) => obj is ReturnTypeInfo o && o == this;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(ReturnTypeInfo obj) => this == obj;

	public readonly override int GetHashCode() => HashCode.Combine(ReturnType, ReturnTypeKind, ReturnTypeWrapperKind);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ReturnTypeInfo left, ReturnTypeInfo right) => left.ReturnType == right.ReturnType
		&& left.ReturnTypeWrapperKind == right.ReturnTypeWrapperKind
		&& left.Nullable == right.Nullable
		&& left.ValueType == right.ValueType
		&& left.ReturnTypeKind == right.ReturnTypeKind;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ReturnTypeInfo left, ReturnTypeInfo right) => !(left == right);
}