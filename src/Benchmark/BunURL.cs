using System.Buffers;
using System.Text;

using Cysharp.Text;

public static class BunURL
{
	private const int UTF8_ACCEPT = 12;
	private const int UTF8_REJECT = 0;

	public static readonly ushort[] UTF8_DATA = [
		// The first part of the table maps bytes to character to a transition.
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
		3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
		4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
		5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
		6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 7, 7,
		10, 9, 9, 9, 11, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,

		// The second part of the table maps a state to a new state when adding a
		// transition.
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		12, 0, 0, 0, 0, 24, 36, 48, 60, 72, 84, 96,
		0, 12, 12, 12, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 24, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 24, 24, 24, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 24, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 48, 48, 48, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 48, 48, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 48, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,

		// The third part maps the current transition to a mask that needs to apply
		// to the byte.
		0x7F, 0x3F, 0x3F, 0x3F, 0x00, 0x1F, 0x0F, 0x0F, 0x0F, 0x07, 0x07, 0x07
	];

	public static readonly int[] NoEscape = [
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
		1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0,
	];

	public static readonly int[] UnHexTable = [
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -1,
		-1, -1, -1, -1, -1, 10, 11, 12, 13, 14, 15, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, 10, 11, 12, 13, 14, 15, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
		-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
	];

	public static string[] HexTable = new string[256];

	public static char[] HexNumberTable = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'];

	public static readonly Dictionary<char, int> HEX = new() {
		{ '0', 0 },
		{ '1', 1 },
		{ '2', 2 },
		{ '3', 3 },
		{ '4', 4 },
		{ '5', 5 },
		{ '6', 6 },
		{ '7', 7 },
		{ '8', 8 },
		{ '9', 9 },
		{ 'a', 10 },
		{ 'A', 10 },
		{ 'b', 11 },
		{ 'B', 11 },
		{ 'c', 12 },
		{ 'C', 12 },
		{ 'd', 13 },
		{ 'D', 13 },
		{ 'e', 14 },
		{ 'E', 14 },
		{ 'f', 15 },
		{ 'F', 15 },
	};


	static BunURL()
	{
		for (var i = 0; i < 256; ++i)
		{
			HexTable[i] = i < 16 ? ZString.Concat("%0", HexNumberTable[i]) : ZString.Concat('%', HexNumberTable[(i & 240) >> 4], HexNumberTable[i & 15]);
		}
	}

	public static string EncodeURIComponent(string rawStr)
	{
		var lastPos = 0;
		var str = rawStr.AsSpan();

		using var result = ZString.CreateStringBuilder(true);

		for (var i = 0; i < str.Length; ++i)
		{
			var c = str[i];

			if (c < 128)
			{
				if (NoEscape[c] == 1) continue;
				if (lastPos < i) result.Append(str.Slice(lastPos, i - lastPos));

				lastPos = i + 1;
				result.Append(HexTable[c]);
				continue;
			}

			if (lastPos < i) result.Append(str.Slice(lastPos, i - lastPos));

			if (c < 2048)
			{
				lastPos = i + 1;
				result.Append(HexTable[192 | (c >> 6)] + HexTable[128 | (c & 63)]);
				continue;
			}

			if (c < 55296 || c >= 57344)
			{
				lastPos = i + 1;
				result.Append(HexTable[224 | (c >> 12)] + HexTable[128 | ((c >> 6) & 63)] + HexTable[128 | (c & 63)]);
				continue;
			}

			++i;

			var c2 = 0;
			if (i < str.Length)
			{
				c2 = str[i] & 1023;
			}
			else
			{
				throw new UriFormatException("URI malformed");
			}
			lastPos = i + 1;
			c = (char)(65536 + (((c & 1023) << 10) | c2));
			result.Append(HexTable[240 | (c >> 18)]);
			result.Append(HexTable[128 | ((c >> 12) & 63)]);
			result.Append(HexTable[128 | ((c >> 6) & 63)]);
			result.Append(HexTable[128 | (c & 63)]);
		}

		if (lastPos == 0) return rawStr;

		if (lastPos < str.Length)
		{
			result.Append(str.Slice(lastPos));
		}

		return result.ToString();
	}

	public static string DecodeURIComponent(string qs, bool decodeSpaces)
	{
		var result = ArrayPool<byte>.Shared.Rent(qs.Length);
		var state = 0;
		int n = 0, m = 0, outIndex = 0;
		char c, hexChar = (char)0;

		for (int inIndex = 0; ; ++inIndex)
		{
			if (inIndex < qs.Length)
			{
				c = qs[inIndex];
			}
			else
			{
				if (state > 0)
				{
					result[outIndex++] = 37;
					if (state == 2) result[outIndex++] = (byte)hexChar;
				}
				break;
			}
			switch (state)
			{
				case 0:
					switch (c)
					{
						case (char)37:
							n = 0;
							m = 0;
							state = 1;
							break;
						case (char)43:
							if (decodeSpaces) c = (char)32;
							goto default;
						default:
							result[outIndex++] = (byte)c;
							break;
					}
					break;
				case 1:
					hexChar = c;
					n = UnHexTable[c];
					if (n < 0)
					{
						result[outIndex++] = 37;
						result[outIndex++] = (byte)c;
						state = 0;
						break;
					}
					state = 2;
					break;
				case 2:
					state = 0;
					m = UnHexTable[c];
					if (m < 0)
					{
						result[outIndex++] = 37;
						result[outIndex++] = (byte)hexChar;
						result[outIndex++] = (byte)c;
						break;
					}
					result[outIndex++] = (byte)((16 * n) + m);
					break;
			}
		}
		var r = Encoding.Default.GetString(result[..outIndex]);
		ArrayPool<byte>.Shared.Return(result);
		return r;
	}

	private static int HexCodeToInt(char c, int shift) => HEX.TryGetValue(c, out int i) ? i << shift : 255;

	public static string DecodeURIComponent(string qs)
	{
		var percentPosition = qs.IndexOf('%');
		if (percentPosition == -1)
		{
			return qs;
		}

		var offset = 0;
		var qss = qs.AsSpan();
		var decodedOwner = ArrayPool<char>.Shared.Rent(qs.Length);
		var decoded = decodedOwner.AsSpan();
		var length = qs.Length;
		var last = 0;
		char codePoint = default;
		var startOfOctets = percentPosition;
		var state = UTF8_ACCEPT;

		while (percentPosition > -1 && percentPosition < length)
		{
			var high = HexCodeToInt(qss[percentPosition + 1], 4);
			var low = HexCodeToInt(qss[percentPosition + 2], 0);
			var b = high | low;
			var type = UTF8_DATA[b];
			state = UTF8_DATA[256 + state + type];
			codePoint = (char)((codePoint << 6) | (b & UTF8_DATA[364 + type]));

			if (state == UTF8_ACCEPT)
			{
				var sliceLen = startOfOctets - last;
				qss.Slice(last, sliceLen).CopyTo(decoded.Slice(offset, sliceLen));
				offset += sliceLen;

				if (codePoint <= 0xFFFF)
				{
					decoded[offset++] = codePoint;
				}
				else
				{
					decoded[offset++] = (char)(0xD7C0 + (codePoint >> 10));
					decoded[offset++] = (char)(0xDC00 + (codePoint & 0x3FF));
				}

				codePoint = (char)0;
				last = percentPosition + 3;
				percentPosition = startOfOctets = qs.IndexOf('%', last);
			}
			else if (state == UTF8_REJECT)
			{
				return "";
			}
			else
			{
				percentPosition += 3;
				if (percentPosition >= length || qs[percentPosition] != 37)
				{
					return "";
				}
			}
		}
		if (last < length)
		{
			var sliceLen = length - last;
			qss[last..].CopyTo(decoded.Slice(offset, sliceLen));
			offset += sliceLen;
		}
		var result = decoded[0..offset].ToString();
		ArrayPool<char>.Shared.Return(decodedOwner);
		return result;
	}
}