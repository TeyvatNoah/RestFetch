namespace RestFetch.WebUtilities;

using InternalDictionary = Dictionary<string, List<string>>;


public partial class QueryString<TEncoder, TDecoder> {
	public static QueryString<TEncoder, TDecoder> ParseSimple(string? str) {
		if (string.IsNullOrWhiteSpace(str)) {
			return [];
		}
		var qs = new QueryString<TEncoder, TDecoder>();
		ParseSimple(str, qs._dict);
		return qs;
	}

	public static QueryString<TEncoder, TDecoder> ParseFromFastQS(string? str) {
		if (string.IsNullOrWhiteSpace(str)) {
			return [];
		}
		var qs = new QueryString<TEncoder, TDecoder>();
		ParseFromFastQS(str, qs._dict);
		return qs;
	}

	private static void AddKeyValue(string k, string v, InternalDictionary dict) {
		if (k == string.Empty) (k, v) = (v, k);
		if (dict.TryGetValue(k, out var list)) {
			list.Add(v);
		} else {
			dict[k] = new(1) { v };
		}
	}

	protected static void ParseSimple(string? input, InternalDictionary dict) {
		if (string.IsNullOrWhiteSpace(input)) return;

		ReadOnlySpan<char> qs = input;
		var len = qs.Length;
		int ampIdx = -1, eqIdx = -1;
		var maybeShouldDecode = false;
		var keyStr = string.Empty;
		var valueStr = string.Empty;

		for (int i = 0; i < len + 1; ++i) {
			var ch = i == len ? '&' : qs[i];
			if (ch == '&') {
				ampIdx = i;
				ReadOnlySpan<char> valueSpan = qs[(eqIdx + 1)..ampIdx];
				valueStr = maybeShouldDecode ? TDecoder.DecodeURIComponent(valueSpan) : new string(valueSpan);

				AddKeyValue(keyStr, valueStr, dict);

				keyStr = string.Empty;
				// valueStr = string.Empty;
				maybeShouldDecode = false;
				// eqIdx不小于上一个ampidx
				eqIdx = i;
			} else if (ch == '=') {
				eqIdx = i;
				ReadOnlySpan<char> keySpan = qs[(ampIdx + 1)..eqIdx];
				keyStr = maybeShouldDecode ? TDecoder.DecodeURIComponent(qs[(ampIdx + 1)..eqIdx]) : new string(keySpan);
			} else if (ch == '+' || ch == '%') {
				maybeShouldDecode = true;
			}
		}
		
	}

	protected static void ParseFromFastQS(string? input, InternalDictionary dict) {
		if (string.IsNullOrWhiteSpace(input)) return;
		var qs = input.AsSpan();
		var inputLength = input.Length;
		var keyStr = string.Empty;
		var valueStr = string.Empty;
		var startingIndex = -1;
		var equalityIndex = -1;
		var shouldDecodeKey = false;
		var shouldDecodeValue = false;
		char c = (char)0;
		
		for (var i = 0; i < inputLength + 1; ++i) {
			c = (char)(i != inputLength ? qs[i] : 38);
			
			if (c == 38) {
				var hasBothKeyValuePair = equalityIndex > startingIndex;

				if (!hasBothKeyValuePair) {
					equalityIndex = i;
				}

				ReadOnlySpan<char> key = qs[(startingIndex + 1)..equalityIndex];

				if (hasBothKeyValuePair || key.Length > 0) {
					keyStr = shouldDecodeKey ? TDecoder.DecodeURIComponent(key) : new string(key);

					if (hasBothKeyValuePair) {
						ReadOnlySpan<char> value = qs[(equalityIndex + 1)..i];

						valueStr = shouldDecodeValue ? TDecoder.DecodeURIComponent(value) : new string(value);
					}
					
					if (dict.TryGetValue(keyStr, out var list)) {
						list.Add(valueStr);
					} else {
						dict[keyStr] = new(1) { valueStr };
					}
				}
				
				startingIndex = i;
				equalityIndex = i;
				shouldDecodeKey = false;
				shouldDecodeValue = false;
			} else if (c == 61) {
				if (equalityIndex <= startingIndex) {
					equalityIndex = i;
				} else {
					shouldDecodeValue = true;
				}
			} else if (c == 43) {
				if (equalityIndex > startingIndex) {
					shouldDecodeValue = true;
				} else {
					shouldDecodeKey = true;
				}
			} else if (c == 37) {
				if (equalityIndex > startingIndex) {
					shouldDecodeValue = true;
				} else {
					shouldDecodeKey = true;
				}
			}
		}
		
	}
}