namespace RestFetch.WebUtilities;

public partial struct FastQueryString {
	public void Add(string key, short value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, short> kv) {
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
	
	public void Add(IEnumerable<KeyValuePair<string, short>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, short[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<short> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, short? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((short)value);
	}
	
	public void Add(KeyValuePair<string, short?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((short)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, short?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((short)value);
		}
		_isFirst = false;
	}

	public void Add(string key, short?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((short)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<short?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((short)value);
		}
		_isFirst = false;
	}

	public void Add(string key, int value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, int> kv) {
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
	
	public void Add(IEnumerable<KeyValuePair<string, int>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, int[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<int> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, int? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((int)value);
	}
	
	public void Add(KeyValuePair<string, int?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((int)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, int?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((int)value);
		}
		_isFirst = false;
	}

	public void Add(string key, int?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((int)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<int?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((int)value);
		}
		_isFirst = false;
	}

	public void Add(string key, long value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, long> kv) {
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
	
	public void Add(IEnumerable<KeyValuePair<string, long>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, long[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<long> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, long? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((long)value);
	}
	
	public void Add(KeyValuePair<string, long?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((long)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, long?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((long)value);
		}
		_isFirst = false;
	}

	public void Add(string key, long?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((long)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<long?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((long)value);
		}
		_isFirst = false;
	}

	public void Add(string key, ushort value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, ushort> kv) {
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
	
	public void Add(IEnumerable<KeyValuePair<string, ushort>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, ushort[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<ushort> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, ushort? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((ushort)value);
	}
	
	public void Add(KeyValuePair<string, ushort?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((ushort)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, ushort?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ushort)value);
		}
		_isFirst = false;
	}

	public void Add(string key, ushort?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ushort)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<ushort?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ushort)value);
		}
		_isFirst = false;
	}

	public void Add(string key, uint value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, uint> kv) {
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
	
	public void Add(IEnumerable<KeyValuePair<string, uint>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, uint[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<uint> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, uint? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((uint)value);
	}
	
	public void Add(KeyValuePair<string, uint?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((uint)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, uint?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((uint)value);
		}
		_isFirst = false;
	}

	public void Add(string key, uint?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((uint)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<uint?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((uint)value);
		}
		_isFirst = false;
	}

	public void Add(string key, ulong value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, ulong> kv) {
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
	
	public void Add(IEnumerable<KeyValuePair<string, ulong>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, ulong[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<ulong> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, ulong? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((ulong)value);
	}
	
	public void Add(KeyValuePair<string, ulong?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((ulong)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, ulong?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ulong)value);
		}
		_isFirst = false;
	}

	public void Add(string key, ulong?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ulong)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<ulong?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ulong)value);
		}
		_isFirst = false;
	}

	public void Add(string key, float value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, float> kv) {
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
	
	public void Add(IEnumerable<KeyValuePair<string, float>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, float[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<float> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, float? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((float)value);
	}
	
	public void Add(KeyValuePair<string, float?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((float)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, float?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((float)value);
		}
		_isFirst = false;
	}

	public void Add(string key, float?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((float)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<float?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((float)value);
		}
		_isFirst = false;
	}

	public void Add(string key, double value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, double> kv) {
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
	
	public void Add(IEnumerable<KeyValuePair<string, double>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, double[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<double> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, double? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((double)value);
	}
	
	public void Add(KeyValuePair<string, double?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((double)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, double?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((double)value);
		}
		_isFirst = false;
	}

	public void Add(string key, double?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((double)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<double?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((double)value);
		}
		_isFirst = false;
	}

	public void Add(string key, decimal value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, decimal> kv) {
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
	
	public void Add(IEnumerable<KeyValuePair<string, decimal>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, decimal[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<decimal> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, decimal? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((decimal)value);
	}
	
	public void Add(KeyValuePair<string, decimal?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(URLHelper.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((decimal)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, decimal?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((decimal)value);
		}
		_isFirst = false;
	}

	public void Add(string key, decimal?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((decimal)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<decimal?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(URLHelper.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((decimal)value);
		}
		_isFirst = false;
	}

}

public partial struct FastQueryString<TEncoder> {
	public void Add(string key, short value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, short> kv) {
		var (key, value) = kv;
		
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, short>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, short[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<short> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, short? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((short)value);
	}
	
	public void Add(KeyValuePair<string, short?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((short)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, short?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((short)value);
		}
		_isFirst = false;
	}

	public void Add(string key, short?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((short)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<short?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((short)value);
		}
		_isFirst = false;
	}

	public void Add(string key, int value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, int> kv) {
		var (key, value) = kv;
		
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, int>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, int[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<int> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, int? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((int)value);
	}
	
	public void Add(KeyValuePair<string, int?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((int)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, int?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((int)value);
		}
		_isFirst = false;
	}

	public void Add(string key, int?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((int)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<int?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((int)value);
		}
		_isFirst = false;
	}

	public void Add(string key, long value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, long> kv) {
		var (key, value) = kv;
		
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, long>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, long[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<long> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, long? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((long)value);
	}
	
	public void Add(KeyValuePair<string, long?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((long)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, long?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((long)value);
		}
		_isFirst = false;
	}

	public void Add(string key, long?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((long)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<long?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((long)value);
		}
		_isFirst = false;
	}

	public void Add(string key, ushort value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, ushort> kv) {
		var (key, value) = kv;
		
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, ushort>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, ushort[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<ushort> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, ushort? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((ushort)value);
	}
	
	public void Add(KeyValuePair<string, ushort?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((ushort)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, ushort?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ushort)value);
		}
		_isFirst = false;
	}

	public void Add(string key, ushort?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ushort)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<ushort?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ushort)value);
		}
		_isFirst = false;
	}

	public void Add(string key, uint value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, uint> kv) {
		var (key, value) = kv;
		
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, uint>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, uint[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<uint> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, uint? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((uint)value);
	}
	
	public void Add(KeyValuePair<string, uint?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((uint)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, uint?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((uint)value);
		}
		_isFirst = false;
	}

	public void Add(string key, uint?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((uint)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<uint?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((uint)value);
		}
		_isFirst = false;
	}

	public void Add(string key, ulong value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, ulong> kv) {
		var (key, value) = kv;
		
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, ulong>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, ulong[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<ulong> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, ulong? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((ulong)value);
	}
	
	public void Add(KeyValuePair<string, ulong?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((ulong)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, ulong?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ulong)value);
		}
		_isFirst = false;
	}

	public void Add(string key, ulong?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ulong)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<ulong?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((ulong)value);
		}
		_isFirst = false;
	}

	public void Add(string key, float value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, float> kv) {
		var (key, value) = kv;
		
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, float>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, float[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<float> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, float? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((float)value);
	}
	
	public void Add(KeyValuePair<string, float?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((float)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, float?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((float)value);
		}
		_isFirst = false;
	}

	public void Add(string key, float?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((float)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<float?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((float)value);
		}
		_isFirst = false;
	}

	public void Add(string key, double value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, double> kv) {
		var (key, value) = kv;
		
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, double>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, double[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<double> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, double? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((double)value);
	}
	
	public void Add(KeyValuePair<string, double?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((double)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, double?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((double)value);
		}
		_isFirst = false;
	}

	public void Add(string key, double?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((double)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<double?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((double)value);
		}
		_isFirst = false;
	}

	public void Add(string key, decimal value) {
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(KeyValuePair<string, decimal> kv) {
		var (key, value) = kv;
		
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append(value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, decimal>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}

	public void Add(string key, decimal[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(values[i]);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<decimal> values) {
		foreach (var value in values) {
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append(value);
		}
		_isFirst = false;
	}
	
	public void Add(string key, decimal? value) {
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((decimal)value);
	}
	
	public void Add(KeyValuePair<string, decimal?> kv) {
		var (key, value) = kv;
		
		if (value is null) return;
		if (!_isFirst) {
	 		_builder.Append('&');		
		} else {
			_isFirst = false;
		}
		
		_builder.Append(TEncoder.EncodeURIComponent(key));
		_builder.Append('=');
		_builder.Append((decimal)value);
	}
	
	public void Add(IEnumerable<KeyValuePair<string, decimal?>> kvPairs) {
		foreach (var (key, value) in kvPairs) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((decimal)value);
		}
		_isFirst = false;
	}

	public void Add(string key, decimal?[] values) {
		for (var i = 0; i < values.Length; ++i) {
			if (values[i] is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((decimal)values[i]!);
		}
		_isFirst = false;
		
	}

	public void Add(string key, IEnumerable<decimal?> values) {
		foreach (var value in values) {
			if (value is null) continue;
			if (!_isFirst) _builder.Append('&');
			_builder.Append(TEncoder.EncodeURIComponent(key));
			_builder.Append('=');
			_builder.Append((decimal)value);
		}
		_isFirst = false;
	}

}
