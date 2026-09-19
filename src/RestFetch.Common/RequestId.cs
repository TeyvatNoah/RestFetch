using System.Globalization;
using System.Runtime.CompilerServices;

namespace RestFetch;

public readonly struct RequestId: IEquatable<RequestId> {
	private readonly long _numericId;
	private readonly string? _stringId;

	public bool IsNumeric => _stringId is null;
	public RequestId(long value) {
		_numericId = value;
		_stringId = null;
	}

	public RequestId(string value) {
		_numericId = 0;
		_stringId = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator RequestId(long value) => new(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator RequestId(string value) => new(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => _stringId is null ? _numericId.ToString(CultureInfo.InvariantCulture) : _stringId;

	public override readonly bool Equals(object? obj) => obj is not null && this == (RequestId)obj;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(RequestId obj) => this == obj;

	public override readonly int GetHashCode() => IsNumeric ? _numericId.GetHashCode() : string.GetHashCode(_stringId, StringComparison.Ordinal);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(RequestId left, RequestId right) => left.IsNumeric && right.IsNumeric ? left._numericId == right._numericId : !left.IsNumeric && !right.IsNumeric && left._stringId == right._stringId;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(RequestId left, RequestId right) => !(left == right);
}