using System.Runtime.CompilerServices;

namespace RestFetch;

[Union]
public readonly struct SseEventResult<T>: IUnion, IEquatable<SseEventResult<T>> {
	private readonly Ok _okValue;

	private readonly Err _errValue;

	private readonly ResultTag _tag;

	public object? Value => _tag switch {
		ResultTag.OK => _okValue,
		ResultTag.ERROR => _errValue,
		_ => null
	};
	public bool HasValue => _tag == ResultTag.OK;

	public bool TryGetValue(out Ok value) {
		value = _okValue;
		return _tag == ResultTag.OK;
	}

	public bool TryGetValue(out Err value) {
		value = _errValue;
		return _tag == ResultTag.ERROR;
	}

	public SseEventResult(Ok okValue) {
		_okValue = okValue;
		_tag = ResultTag.OK;
	}

	public SseEventResult(Err errValue) {
		_errValue = errValue;
		_tag = ResultTag.ERROR;
	}

	public readonly override string ToString() => HasValue ? _okValue.ToString() : _errValue.ToString();
	public readonly override bool Equals(object? obj) => obj is SseEventResult<T> o && o == this;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(SseEventResult<T> obj) => this == obj;
	public readonly override int GetHashCode() => _okValue.GetHashCode();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(SseEventResult<T> left, SseEventResult<T> right) => left.HasValue && right.HasValue && left._okValue == right._okValue;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(SseEventResult<T> left, SseEventResult<T> right) => !(left == right);

	public readonly record struct Ok(T Value);
	public readonly record struct Err(Exception Error, bool Skip);

	private enum ResultTag: byte {
		OK,
		ERROR,
	}
}


// public readonly record struct SseEventResult<T>(
// 	T Data,
// 	Exception? Error = null,
// 	bool Skip = false
// );