using System.Collections;
using System.Runtime.CompilerServices;

using Cysharp.Text;

namespace RestFetch.WebUtilities;

#pragma warning disable CA1010,CS0659,CA2231 // Generic interface should also be implemented
public partial struct FastQueryString: IQueryStringSerializer<FastQueryString>, IEnumerable, IDisposable {
#pragma warning restore CA1010,CS0659,CA2231 // Generic interface should also be implemented
	public string TrueValue { get; init; } = "true";

	public string FalseValue { get; init; } = "false";
	
	private Utf16ValueStringBuilder _builder = ZString.CreateStringBuilder();

	private bool _isFirst = true;


	public FastQueryString() {}

	public FastQueryString(string str) {
		// TODO 是否需要escape?
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

	// 不要readonly,避免防御性拷贝
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => _builder.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToUriComponent() => _builder.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() => _builder.Dispose();

	// public readonly override bool Equals(object? obj) => false;

	// public static bool operator !=(FastQueryString left, FastQueryString right) => false;

	// public override int GetHashCode() => throw new NotImplementedException();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(FastQueryString qs) => qs.ToString();

	readonly IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}

#pragma warning disable CA1010,CS0659,CA2231 // Generic interface should also be implemented
public partial struct FastQueryString<TEncoder>: IQueryStringSerializer<FastQueryString>, IEnumerable, IDisposable
	where TEncoder: IURLEncoder {
#pragma warning restore CA1010,CS0659,CA2231 // Generic interface should also be implemented
	public string TrueValue { get; init; } = "true";

	public string FalseValue { get; init; } = "false";
	
	private Utf16ValueStringBuilder _builder = ZString.CreateStringBuilder();

	private bool _isFirst = true;


	public FastQueryString() {}

	public FastQueryString(string str) {
		// TODO 是否需要escape?
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
	

	// 不要readonly,避免防御性拷贝
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => _builder.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToUriComponent() => _builder.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() => _builder.Dispose();

	// public readonly override bool Equals(object? obj) => false;

	// public static bool operator !=(FastQueryString left, FastQueryString right) => false;

	// public override int GetHashCode() => throw new NotImplementedException();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(FastQueryString qs) => qs.ToString();

	readonly IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}
