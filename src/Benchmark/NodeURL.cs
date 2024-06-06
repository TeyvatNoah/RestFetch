using System.Buffers;

using Cysharp.Text;

public static class NodeURL {
	public static readonly int[] IsHexTable = [
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0 - 15
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 16 - 31
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 32 - 47
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, // 48 - 63
		0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 64 - 79
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 80 - 95
		0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 96 - 111
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 112 - 127
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 128 ...
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  // ... 256
	];
	
	public static int[] NoEscapeTable = [
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0 - 15
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 16 - 31
		0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, // 32 - 47
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, // 48 - 63
		0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 64 - 79
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, // 80 - 95
		0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 96 - 111
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0,  // 112 - 127
	];
	
	public static string[] HexTable = new string[256];
	
	public static char[] HexNumberTable = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'];
	
	static NodeURL() {
		for (var i = 0; i < 256; ++i) {
			HexTable[i] = i < 16 ? ZString.Concat("%0", HexNumberTable[i]) : ZString.Concat('%', HexNumberTable[(i & 240) >> 4], HexNumberTable[i & 15]);
		}
	}

	public static string EncodeURIComponent(string rawStr) {
		var len = rawStr.Length;
		var lastPos = 0;
		var i = 0;
		var str = rawStr.AsSpan();

		using var result = ZString.CreateStringBuilder(true);
		
		for (; i < len; ++i) {
			var c = str[i];
			
			while (c < 0x80) {
				if (NoEscapeTable[c] != 1) {
					if (lastPos < i) result.Append(str.Slice(lastPos, i - lastPos));
					lastPos = i + 1;
					result.Append(HexTable[c]);
				}
				
				if (++i == len) {
					goto outer;
				}
				
				c = str[i];
			}
			
			if (lastPos < i) {
				result.Append(str.Slice(lastPos, i - lastPos));
			}

			if (c < 0x800) {
				lastPos = i + 1;
				result.Append(HexTable[0xC0 | (c >> 6)] + HexTable[0x80 | (c & 0x3F)]);
				continue;
			}
			
			if (c < 0xD800 || c >= 0xE000) {
				lastPos = i + 1;
				result.Append(HexTable[0xE0 | (c >> 12)]);
				result.Append(HexTable[0x80 | (c >> 6) & 0x3F]);
				result.Append(HexTable[0x80 | (c & 0x3F)]);
				continue;
			}
			++i;
			
			if (i >= len) {
				throw new UriFormatException("Invalid URI.");
			}
			
			var c2 = str[i] & 0x3FF;
			lastPos = i + 1;
			c = (char)(0x10000 + (((c & 0x3FF) << 10) | c2));
			
			result.Append(HexTable[0xF0 | (c >> 18)]);
			result.Append(HexTable[0x80 | ((c >> 12) & 0x3F)]);
			result.Append(HexTable[0x80 | ((c >> 6) & 0x3F)]);
			result.Append(HexTable[0x80 | (c & 0x3F)]);
		}
		
		outer:
		if (lastPos == 0) {
			return rawStr;
		}
		
		if (lastPos < len) {
			result.Append(str.Slice(lastPos));
		}

		return result.ToString();
	}
}