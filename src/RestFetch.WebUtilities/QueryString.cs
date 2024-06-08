using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Cysharp.Text;

namespace RestFetch.WebUtilities;

using InternalDictionary = Dictionary<string, List<string>>;

// 假装有默认泛型参数
public sealed class QueryString:
	QueryString<URLHelper, URLHelper>,
	IQueryStringConverter<QueryString>,
	IAliasQueryStringSerializer<QueryString> {
	
	public QueryString() {}

	public QueryString(string str): base(str) {}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(QueryString value) => QueryString<URLHelper, URLHelper>.Serialize(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, QueryString? value) => QueryString<URLHelper, URLHelper>.Serialize(aliasKey, value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(string? str, [NotNullWhen(true)]out QueryString? retval) {
		var result = TryParse(str, out var r);
		retval = r;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new static QueryString Parse(string? str) => Unsafe.As<QueryString>(QueryString<URLHelper, URLHelper>.Parse(str));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new QueryString Set(string key, string value) {
		_dict[key] = new(1) {
			value
		};
		_shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;
		return this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new QueryString Set(string key, IEnumerable<string> list) {
		_dict[key] = list is List<string> l ? l : [..list];
		_shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;
		return this;
	}
}

public partial class QueryString<TEncoder, TDecoder>:
	IQueryStringConverter<QueryString<TEncoder, TDecoder>>,
	// for internal
	IAliasQueryStringSerializer<QueryString<TEncoder, TDecoder>>
	where TEncoder: IURLEncoder
	where TDecoder: IURLDecoder
	{
	public const string Separator = "&";
	public const string Eq = "=";
	// 此字符串不执行URL编码,务必提前URL编码
	public static string TrueStr = "1";
	public static string FalseStr = "0";

	protected readonly InternalDictionary _dict = [];

	protected string _lastToStringResult = "";

	protected string _lastToURIComponentResult = "";

	protected ShouldUpdateStatus _shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;

	public bool HasValue => _dict.Count > 0;

	public string Value => "?" + ToUriComponent();

	public int Count => _dict.Count;

	public ICollection<string> Keys => _dict.Keys;

	public QueryString() {}

	public QueryString(string str) {
		Parse(str, _dict);
	}

	public static QueryString<TEncoder, TDecoder> Parse(string? str) {
		if (string.IsNullOrWhiteSpace(str)) {
			return [];
		}
		var qs = new QueryString<TEncoder, TDecoder>();
		Parse(str, qs._dict);
		return qs;
	}

	protected static void Parse(string? input, InternalDictionary dict) {
		if (string.IsNullOrWhiteSpace(input)) return;

		ReadOnlySpan<char> qs = input;
		var len = qs.Length;
		int start = 0, end = 0;
		for (int i = 0; i < len; ++i) {
			if (qs[i] == '&') {
				end = i;
				AddKeyValuePair(qs[start..end], dict);
				start = end + 1;
			}
		}
		
		// start == end == 0
		if (start == end) {
			AddKeyValuePair(qs, dict);
		}
	}
	
	private static void AddKeyValuePair(ReadOnlySpan<char> kvpair, InternalDictionary dict) {
		int start = 0, end = 0;
		for (int i = 0, len = kvpair.Length; i < len; ++i) {
			if (kvpair[i] == '=') {
				end = i;
				var key = TDecoder.DecodeURIComponent(kvpair[start..end]);
				var value = TDecoder.DecodeURIComponent(kvpair[(end + 1)..len]);
				AddKeyValue(key, value, dict);
			}
		}
		
		if (start == end) {
			AddKeyValue(new(kvpair), string.Empty, dict);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void AddKeyValue(string k, string v, InternalDictionary dict) {
			if (dict.TryGetValue(k, out var list)) {
				list.Add(v);
			} else {
				dict[k] = new(1) { v };
			}
		}
	}

	public static bool TryParse(string? str, [NotNullWhen(true)]out QueryString<TEncoder, TDecoder>? retval) {
		try {
			var qs = Parse(str);
			retval = qs;
			return true;
		} catch {
			retval = null;
			return false;
		}
	}

	// QueryString
	public static string Serialize(QueryString<TEncoder, TDecoder>? qs) {
		if (qs is null) {
			return "";
		} else {
			using var sb = ZString.CreateStringBuilder();

			var iter = qs.GetEnumerator();

			if (iter.MoveNext()) {
				// iter.Current.Value必不为null,所有入口处确保
				var (key, value) = iter.Current;
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, TEncoder.EncodeURIComponent(value)));
				sb.Append(TEncoder.EncodeURIComponent(key));
				sb.Append(Eq);
				sb.Append(TEncoder.EncodeURIComponent(value));
			}

			while (iter.MoveNext()) {
				var (key, value) = iter.Current;
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(key), Eq, TEncoder.EncodeURIComponent(value)));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(key));
				sb.Append(Eq);
				sb.Append(TEncoder.EncodeURIComponent(value));
			}

			return sb.ToString();
		}
	}

	// QueryString
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, QueryString<TEncoder, TDecoder>? value) => Serialize(value);

	// string
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string? value) => TEncoder.EncodeURIComponent(value);

	// IEnumerable<KeyValuePair<string, string?>>/IEnumerable<KeyValuePair<string, string>>
	public static string Serialize(IEnumerable<KeyValuePair<string, string?>> collection) {
		using var sb = ZString.CreateStringBuilder();
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext() && !string.IsNullOrEmpty(value)) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, TEncoder.EncodeURIComponent(value)));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(TEncoder.EncodeURIComponent(value));
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			if (string.IsNullOrEmpty(innerValue)) continue;
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, TEncoder.EncodeURIComponent(innerValue)));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(TEncoder.EncodeURIComponent(innerValue));
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, string?>>/IEnumerable<KeyValuePair<string, string>>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, string?>> collection) => Serialize(collection);

	// string
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, string? value) => string.IsNullOrEmpty(value) ? "" : ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, TEncoder.EncodeURIComponent(value));

	// IEnumerable<string?>/IEnumerable<string>
	public static string Serialize(string aliasKey, IEnumerable<string?> collection) {
		using var sb = ZString.CreateStringBuilder();
		using var iterator = collection.GetEnumerator();

		var current = iterator.Current;
		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && !string.IsNullOrEmpty(current)) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, TEncoder.EncodeURIComponent(current)));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(TEncoder.EncodeURIComponent(current));
		}

		while (iterator.MoveNext()) {
			var innerCurrent = iterator.Current;
			if (string.IsNullOrEmpty(innerCurrent)) continue;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, TEncoder.EncodeURIComponent(innerCurrent)));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(TEncoder.EncodeURIComponent(innerCurrent));
		}

		return sb.ToString();
	}

	// string[]/string?[]
	public static string Serialize(string aliasKey, string?[] array) {
		using var sb = ZString.CreateStringBuilder();

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			if (!string.IsNullOrEmpty(array[0])) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, TEncoder.EncodeURIComponent(v)));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(TEncoder.EncodeURIComponent(array[0]));
			}
			for (var i = 1; i < array.Length; ++i) {
				if (string.IsNullOrEmpty(array[i])) continue;
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, TEncoder.EncodeURIComponent(vv)));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(TEncoder.EncodeURIComponent(array[i]));
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// bool
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(bool value) => value ? TrueStr : FalseStr;

	// bool
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, bool value) => ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value));

	// bool?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(bool? value) => value is bool v ? v ? TrueStr : FalseStr : "";

	// bool?
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string aliasKey, bool? value) => value is bool ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(value)) : "";

	// bool[]
	public static string Serialize(string aliasKey, bool[] array) {
		using var sb = ZString.CreateStringBuilder();

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(array[0])));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(Serialize(array[0]));
			for (var i = 1; i < array.Length; ++i) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(array[i])));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(Serialize(array[i]));
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// bool?[]
	public static string Serialize(string aliasKey, bool?[] array) {
		using var sb = ZString.CreateStringBuilder();

		if (array.Length > 0) {
			var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

			if (array[0] is bool v) {
				// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(v)));
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(Serialize(v));
			}

			for (var i = 1; i < array.Length; ++i) {
				if (array[i] is bool vv) {
					// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(vv)));
					sb.Append(Separator);
					sb.Append(encodedKey);
					sb.Append(Eq);
					sb.Append(Serialize(vv));
				}
			}
		} else {
			return "";
		}

		return sb.ToString();
	}

	// IEnumerable<bool>
	public static string Serialize(string aliasKey, IEnumerable<bool> collection) {
		using var sb = ZString.CreateStringBuilder();
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(iterator.Current)));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(Serialize(iterator.Current));
		}

		while (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(iterator.Current)));
			sb.Append(Separator);
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(Serialize(iterator.Current));
		}

		return sb.ToString();
	}

	// IEnumerable<bool?>
	public static string Serialize(string aliasKey, IEnumerable<bool?> collection) {
		using var sb = ZString.CreateStringBuilder();
		using var iterator = collection.GetEnumerator();

		var encodedKey = TEncoder.EncodeURIComponent(aliasKey);

		if (iterator.MoveNext() && iterator.Current is bool v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(v)));
			sb.Append(encodedKey);
			sb.Append(Eq);
			sb.Append(Serialize(v));
		}

		while (iterator.MoveNext()) {
			if (iterator.Current is bool vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(aliasKey), Eq, Serialize(vv)));
				sb.Append(Separator);
				sb.Append(encodedKey);
				sb.Append(Eq);
				sb.Append(Serialize(vv));
			}
		}

		return sb.ToString();
	}

	// IEnumerable<KeyValuePair<string, bool>>
	public static string Serialize(IEnumerable<KeyValuePair<string, bool>> collection) {
		using var sb = ZString.CreateStringBuilder();
		using var iterator = collection.GetEnumerator();

		var (key, value) = iterator.Current;
		if (iterator.MoveNext()) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(key), Eq, Serialize(value)));
			sb.Append(TEncoder.EncodeURIComponent(key));
			sb.Append(Eq);
			sb.Append(Serialize(value));
		}

		while (iterator.MoveNext()) {
			var (innerKey, innerValue) = iterator.Current;
			// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(innerKey), Eq, Serialize(innerValue)));
			sb.Append(Separator);
			sb.Append(TEncoder.EncodeURIComponent(innerKey));
			sb.Append(Eq);
			sb.Append(Serialize(innerValue));
		}

		return sb.ToString();
	}

	public static string Serialize(string _, IEnumerable<KeyValuePair<string, bool>> collection) => Serialize(collection);

	// IEnumerable<KeyValuePair<string, bool?>>
	public static string Serialize(IEnumerable<KeyValuePair<string, bool?>> collection) {
		using var sb = ZString.CreateStringBuilder();
		using var iterator = collection.GetEnumerator();

		if (iterator.MoveNext() && iterator.Current.Value is bool v) {
			// sb.Append(ZString.Concat(TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, Serialize(v)));
			sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
			sb.Append(Eq);
			sb.Append(Serialize(v));
		}

		while (iterator.MoveNext()) {
			if (iterator.Current.Value is bool vv) {
				// sb.Append(ZString.Concat(Separator, TEncoder.EncodeURIComponent(iterator.Current.Key), Eq, Serialize(vv)));
				sb.Append(Separator);
				sb.Append(TEncoder.EncodeURIComponent(iterator.Current.Key));
				sb.Append(Eq);
				sb.Append(Serialize(vv));
			}
		}

		return sb.ToString();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Serialize(string _, IEnumerable<KeyValuePair<string, bool?>> collection) => Serialize(collection);

	// IDictionary
	public List<string>? this[string k] {
		get => _dict[k];
		[param: DisallowNull]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set {
			if (value is not null) Set(k, value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QueryString<TEncoder, TDecoder> Set(string key, string value) {
		_dict[key] = new(1) {
			value
		};
		_shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;
		return this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QueryString<TEncoder, TDecoder> Set(string key, IEnumerable<string> list) {
		_dict[key] = list is List<string> l ? l : [..list];
		_shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;
		return this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QueryString<TEncoder, TDecoder> Add(string key, string value) {
		if (_dict.TryGetValue(key, out var values)) {
			values.Add(value);
		} else {
			values = new(1) {
				value
			};
			_dict.Add(key, values);
		}
		_shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;
		return this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QueryString<TEncoder, TDecoder> Add(KeyValuePair<string, string> item) => Add(item.Key, item.Value);

	public QueryString<TEncoder, TDecoder> Add(string key, IEnumerable<string> collection) {
		if (_dict.TryGetValue(key, out var values)) {
			values.AddRange(collection);
		} else {
			values = [..collection];
			_dict.Add(key, values);
		}
		_shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;
		return this;
	}

	public QueryString<TEncoder, TDecoder> Add(string key, string[] array) {
		if (_dict.TryGetValue(key, out var values)) {
			for (var i = 0; i < array.Length; ++i){
				values.Add(array[i]);
			}
		} else {
			values = new(array.Length);
			for (var i = 0; i < array.Length; ++i){
				values.Add(array[i]);
			}
			_dict.Add(key, values);
		}
		_shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;
		return this;
	}

	public QueryString<TEncoder, TDecoder> Add(IEnumerable<KeyValuePair<string, string>> collection) {
		foreach (var item in collection) {
			Add(item.Key, item.Value);
		}
		_shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;
		return this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ContainsKey(string key) => _dict.ContainsKey(key);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() {
		_dict.Clear();
		_shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;
	}

	public void Remove(string key, out bool result) {
		result = _dict.Remove(key);
		_shouldUpdate = ShouldUpdateStatus.Raw | ShouldUpdateStatus.Escaped;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public QueryString<TEncoder, TDecoder> Remove(string key) {
		Remove(key, out _);
		return this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetValue(string key, [NotNullWhen(true)]out List<string>? value) => _dict.TryGetValue(key, out value);

	// ICollection
	void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item) => Add(item.Key, item.Value);

	bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item) {
		Remove(item.Key, out var result);
		return result;
	}

	bool ICollection<KeyValuePair<string, string>>.IsReadOnly => false;

	bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item) => ((ICollection<KeyValuePair<string, string>>)_dict).Contains(item);

	void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int index) => ((ICollection<KeyValuePair<string, string>>)_dict).CopyTo(array, index);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);

	IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() {
		foreach (var (key, list) in _dict) {
			foreach (var item in list) {
				yield return new(key, item);
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		foreach (var (key, list) in _dict) {
			foreach (var item in list) {
				yield return new KeyValuePair<string, string>(key, item);
			}
		}
	}

	// 无法实现约束
	// [MethodImpl(MethodImplOptions.AggressiveInlining)]
	// private static string SerializeNumeric<N>(string? aliasKey, N v) where N: struct => v is N ? ZString.Concat(TEncoder.EncodeURIComponent(aliasKey), Eq, v.ToString(CultureInfo.InvariantCulture)) : "";

	protected static string UnescapeSerialize(QueryString<TEncoder, TDecoder> qs) {
		using var sb = ZString.CreateStringBuilder();

		var iter = qs.GetEnumerator();

		if (iter.MoveNext()) {
			// iter.Current.Value必不为null,所有入口处确保
			var (key, value) = iter.Current;
			// sb.Append(ZString.Concat(key, Eq, value));
			sb.Append(key);
			sb.Append(Eq);
			sb.Append(value);
		}

		while (iter.MoveNext()) {
			var (key, value) = iter.Current;
			// sb.Append(ZString.Concat(Separator, key, Eq, value));
			sb.Append(Separator);
			sb.Append(key);
			sb.Append(Eq);
			sb.Append(value);
		}

		return sb.ToString();
	}

	// 不做URL escape
	// sealed 应该可以内联
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() {
		if ((_shouldUpdate & ShouldUpdateStatus.Raw) == ShouldUpdateStatus.Raw) {
			_lastToStringResult = UnescapeSerialize(this);
			_shouldUpdate &= ~ShouldUpdateStatus.Raw;
		}
		return _lastToStringResult;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToUriComponent() {
		if ((_shouldUpdate & ShouldUpdateStatus.Escaped) == ShouldUpdateStatus.Escaped) {
			_lastToURIComponentResult = Serialize(this);
			_shouldUpdate &= ~ShouldUpdateStatus.Escaped;
		}
		return _lastToURIComponentResult;
	}

	[Flags]
	protected enum ShouldUpdateStatus: byte {
		False = 0,
		Raw = 1,
		Escaped = 2
	}

	// [MethodImpl(MethodImplOptions.AggressiveInlining)]
	// public override ReadOnlySpan<byte> ToUTF8String() => SerializeUTF8String(this);

	public struct Enumerator(QueryString<TEncoder, TDecoder> qs) {
		public KeyValuePair<string, string> Current { get; private set; }

		private InternalDictionary.Enumerator _dictEnumerator = qs._dict.GetEnumerator();

		private IEnumerator<string>? _listEnumerator;

		public bool MoveNext() {
			if (_listEnumerator is not null && _listEnumerator.MoveNext()) {
				Current = new(_dictEnumerator.Current.Key, _listEnumerator.Current);
				return true;
			} else if (_dictEnumerator.MoveNext()) {
				_listEnumerator = _dictEnumerator.Current.Value.GetEnumerator();
				if (_listEnumerator.MoveNext()) {
					Current = new(_dictEnumerator.Current.Key, _listEnumerator.Current);
					return true;
				}
			}
			Current = default;
			return false;
		}

		public void Dispose() {
			_listEnumerator?.Dispose();
			_dictEnumerator.Dispose();
		}

		public readonly override string ToString() => $"{Current.Key}={Current.Value}";

		public readonly override bool Equals(object? obj) => false;

		public override readonly int GetHashCode() => Guid.NewGuid().GetHashCode();

		public static bool operator ==(Enumerator left, Enumerator right) => false;

		public static bool operator !=(Enumerator left, Enumerator right) => true;
	}
}