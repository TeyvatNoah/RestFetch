using System.Runtime.CompilerServices;

using Cysharp.Text;

namespace RestFetch.WebUtilities;

#pragma warning disable CA1010,CS0659,CA2231 // Generic interface should also be implemented
public partial struct FastQueryString: IQueryStringSerializer<FastQueryString>, IEquatable<FastQueryString>, ISpanFormattable, IDisposable {
#pragma warning restore CA1010,CS0659,CA2231 // Generic interface should also be implemented
	public const string TRUE = "1";

	public const string FALSE = "0";

	public string TrueValue = TRUE;

	public string FalseValue = FALSE;

	private Utf16ValueStringBuilder _builder = ZString.CreateStringBuilder();

	private bool _isFirst = true;


	public FastQueryString() {}

	public FastQueryString(string str) {
		// 是否需要escape? 不要,由用户自己负责
		_builder.Append(str);
		_isFirst = false;
	}

	public void Add(string key, string? value) {
		if (string.IsNullOrEmpty(value)) return;

		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(URLHelper.EncodeURIComponent(value));
	}

	public void Add(KeyValuePair<string, string> kv) {
		var (key, value) = kv;

		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(URLHelper.EncodeURIComponent(value));
	}

	public void Add(IEnumerable<KeyValuePair<string, string>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(URLHelper.EncodeURIComponent(value));
		}
		_isFirst = false;
	}

	public void Add(string key, string[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(URLHelper.EncodeURIComponent(values[i]));
		}
		_isFirst = false;
	}

	public void Add(string key, IEnumerable<string> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(URLHelper.EncodeURIComponent(value));
		}
		_isFirst = false;
	}

	public void Add(string key, bool value) {
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value ? TrueValue : FalseValue);
	}

	public void Add(KeyValuePair<string, bool> kv) {
		var (key, value) = kv;

		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value ? TrueValue : FalseValue);
	}

	public void Add(IEnumerable<KeyValuePair<string, bool>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value ? TrueValue : FalseValue);
		}
		_isFirst = false;
	}

	public void Add(string key, bool[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i] ? TrueValue : FalseValue);
		}
		_isFirst = false;

	}

	public void Add(string key, IEnumerable<bool> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value ? TrueValue : FalseValue);
		}
		_isFirst = false;
	}

	public void Add(string key, bool? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((bool)value ? TrueValue : FalseValue);
	}

	public void Add(KeyValuePair<string, bool?> kv) {
		var (key, value) = kv;

		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((bool)value ? TrueValue : FalseValue);
	}

	public void Add(IEnumerable<KeyValuePair<string, bool?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((bool)value ? TrueValue : FalseValue);
		}
		_isFirst = false;
	}

	public void Add(string key, bool?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((bool)values[i]! ? TrueValue : FalseValue);
		}
		_isFirst = false;

	}

	public void Add(string key, IEnumerable<bool?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((bool)value ? TrueValue : FalseValue);
		}
		_isFirst = false;
	}

	public void Add(string key, sbyte value) {
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}

	public void Add(KeyValuePair<string, sbyte> kv) {
		var (key, value) = kv;

		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}

	public void Add(IEnumerable<KeyValuePair<string, sbyte>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, sbyte? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((sbyte)value);
	}

	public void Add(KeyValuePair<string, sbyte?> kv) {
		var (key, value) = kv;

		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((sbyte)value);
	}

	public void Add(IEnumerable<KeyValuePair<string, sbyte?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((sbyte)value);
		}
		_isFirst = false;
	}


	public void Add(string key, byte value) {
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}

	public void Add(KeyValuePair<string, byte> kv) {
		var (key, value) = kv;

		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}

	public void Add(IEnumerable<KeyValuePair<string, byte>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, byte? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((byte)value);
	}

	public void Add(KeyValuePair<string, byte?> kv) {
		var (key, value) = kv;

		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((byte)value);
	}

	public void Add(IEnumerable<KeyValuePair<string, byte?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((byte)value);
		}
		_isFirst = false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
		=> _builder.TryCopyTo(destination, out charsWritten);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? format, IFormatProvider? formatProvider) => _builder.ToString();

	// 不要readonly,避免防御性拷贝
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => _builder.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToUriComponent() => _builder.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() => _builder.Dispose();

	// public override int GetHashCode() => throw new NotImplementedException();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(FastQueryString qs) => qs.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator string(FastQueryString qs) => qs.ToString();

	public override int GetHashCode() => ToString().GetHashCode();

	public override readonly bool Equals(object? obj) => false;


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(FastQueryString other) => this == other;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(FastQueryString left, FastQueryString right) => left.ToString() != right.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(FastQueryString left, FastQueryString right) => left.ToString() == right.ToString();
}

#pragma warning disable CA1010,CS0659,CA2231 // Generic interface should also be implemented
public partial struct FastQueryString<TEncoder>: IQueryStringSerializer<FastQueryString>, IEquatable<FastQueryString<TEncoder>>, ISpanFormattable, IDisposable
	where TEncoder: IURLEncoder {
#pragma warning restore CA1010,CS0659,CA2231 // Generic interface should also be implemented
	public string TrueValue { get; init; } = "true";

	public string FalseValue { get; init; } = "false";

	private Utf16ValueStringBuilder _builder = ZString.CreateStringBuilder();

	private bool _isFirst = true;


	public FastQueryString() {}

	public FastQueryString(string str) {
		// 是否需要escape?不要,由用户自己负责
		_builder.Append(str);
		_isFirst = false;
	}

	public void Add(string key, string? value) {
		if (string.IsNullOrEmpty(value)) return;

		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(TEncoder.EncodeURIComponent(value));
	}

	public void Add(KeyValuePair<string, string> kv) {
		var (key, value) = kv;

		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(TEncoder.EncodeURIComponent(value));
	}

	public void Add(IEnumerable<KeyValuePair<string, string>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(TEncoder.EncodeURIComponent(value));
		}
		_isFirst = false;
	}

	public void Add(string key, string[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(TEncoder.EncodeURIComponent(values[i]));
		}
		_isFirst = false;
	}

	public void Add(string key, IEnumerable<string> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(TEncoder.EncodeURIComponent(value));
		}
		_isFirst = false;
	}

	public void Add(string key, bool value) {
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value ? TrueValue : FalseValue);
	}

	public void Add(KeyValuePair<string, bool> kv) {
		var (key, value) = kv;

		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value ? TrueValue : FalseValue);
	}

	public void Add(IEnumerable<KeyValuePair<string, bool>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value ? TrueValue : FalseValue);
		}
		_isFirst = false;
	}

	public void Add(string key, bool[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i] ? TrueValue : FalseValue);
		}
		_isFirst = false;

	}

	public void Add(string key, IEnumerable<bool> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value ? TrueValue : FalseValue);
		}
		_isFirst = false;
	}

	public void Add(string key, bool? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((bool)value ? TrueValue : FalseValue);
	}

	public void Add(KeyValuePair<string, bool?> kv) {
		var (key, value) = kv;

		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((bool)value ? TrueValue : FalseValue);
	}

	public void Add(IEnumerable<KeyValuePair<string, bool?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((bool)value ? TrueValue : FalseValue);
		}
		_isFirst = false;
	}

	public void Add(string key, bool?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((bool)values[i]! ? TrueValue : FalseValue);
		}
		_isFirst = false;

	}

	public void Add(string key, IEnumerable<bool?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((bool)value ? TrueValue : FalseValue);
		}
		_isFirst = false;
	}

	public void Add(string key, sbyte value) {
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}

	public void Add(KeyValuePair<string, sbyte> kv) {
		var (key, value) = kv;

		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}

	public void Add(IEnumerable<KeyValuePair<string, sbyte>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, sbyte? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((sbyte)value);
	}

	public void Add(KeyValuePair<string, sbyte?> kv) {
		var (key, value) = kv;

		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((sbyte)value);
	}

	public void Add(IEnumerable<KeyValuePair<string, sbyte?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((sbyte)value);
		}
		_isFirst = false;
	}


	public void Add(string key, byte value) {
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}

	public void Add(KeyValuePair<string, byte> kv) {
		var (key, value) = kv;

		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}

	public void Add(IEnumerable<KeyValuePair<string, byte>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, byte? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((byte)value);
	}

	public void Add(KeyValuePair<string, byte?> kv) {
		var (key, value) = kv;

		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');
		} else {
			_isFirst = false;
		}

		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((byte)value);
	}

	public void Add(IEnumerable<KeyValuePair<string, byte?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((byte)value);
		}
		_isFirst = false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
		=> _builder.TryCopyTo(destination, out charsWritten);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? format, IFormatProvider? formatProvider) => _builder.ToString();

	// 不要readonly,避免防御性拷贝
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => _builder.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToUriComponent() => _builder.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() => _builder.Dispose();

	// public override int GetHashCode() => throw new NotImplementedException();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(FastQueryString qs) => qs.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator string(FastQueryString<TEncoder> qs) => qs.ToString();

	public override int GetHashCode() => ToString().GetHashCode();

	public readonly override bool Equals(object? obj) => false;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(FastQueryString<TEncoder> other) => this == other;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(FastQueryString<TEncoder> left, FastQueryString<TEncoder> right) => left.ToString() != right.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(FastQueryString<TEncoder> left, FastQueryString<TEncoder> right) => left.ToString() == right.ToString();
}
