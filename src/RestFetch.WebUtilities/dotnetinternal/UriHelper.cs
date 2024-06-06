// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace RestFetch.WebUtilities;
internal static class UriHelper
{

	//
	// This method will assume that any good Escaped Sequence will be unescaped in the output
	// - Assumes Dest.Length - detPosition >= end-start
	// - UnescapeLevel controls various modes of operation
	// - Any "bad" escape sequence will remain as is or '%' will be escaped.
	// - destPosition tells the starting index in dest for placing the result.
	//   On return destPosition tells the last character + 1 position in the "dest" array.
	// - The control chars and chars passed in rsdvX parameters may be re-escaped depending on UnescapeLevel
	// - It is a RARE case when Unescape actually needs escaping some characters mentioned above.
	//   For this reason it returns a char[] that is usually the same ref as the input "dest" value.
	//
	internal static unsafe void UnescapeString(string input, int start, int end, ref ValueStringBuilder dest,
			char rsvd1, char rsvd2, char rsvd3, UnescapeMode unescapeMode)
	{
		fixed (char* pStr = input)
		{
			UnescapeString(pStr, start, end, ref dest, rsvd1, rsvd2, rsvd3, unescapeMode);
		}
	}

	internal static unsafe void UnescapeString(ReadOnlySpan<char> input, int start, int end, ref ValueStringBuilder dest,
			char rsvd1, char rsvd2, char rsvd3, UnescapeMode unescapeMode)
	{
		fixed (char* pStr = input)
		{
			UnescapeString(pStr, start, end, ref dest, rsvd1, rsvd2, rsvd3, unescapeMode);
		}
	}

	internal static unsafe void UnescapeString(char* pStr, int start, int end, ref ValueStringBuilder dest,
			char rsvd1, char rsvd2, char rsvd3, UnescapeMode unescapeMode)
	{
		if ((unescapeMode & UnescapeMode.EscapeUnescape) == UnescapeMode.CopyOnly)
		{
			dest.Append(pStr + start, end - start);
			return;
		}

		bool escapeReserved = false;
		bool iriParsing = ((unescapeMode & UnescapeMode.EscapeUnescape) == UnescapeMode.EscapeUnescape);

		for (int next = start; next < end;)
		{
			char ch = (char)0;

			for (; next < end; ++next)
			{
				if ((ch = pStr[next]) == '%')
				{
					if ((unescapeMode & UnescapeMode.Unescape) == 0)
					{
						// re-escape, don't check anything else
						escapeReserved = true;
					}
					else if (next + 2 < end)
					{
						ch = DecodeHexChars(pStr[next + 1], pStr[next + 2]);
						// Unescape a good sequence if full unescape is requested
						if (unescapeMode >= UnescapeMode.UnescapeAll)
						{
							if (ch == UriFromBCL.c_DummyChar)
							{
								if (unescapeMode >= UnescapeMode.UnescapeAllOrThrow)
								{
									// Should be a rare case where the app tries to feed an invalid escaped sequence
									throw new UriFormatException("Invalid URI: There is an invalid sequence in the string.");
								}
								continue;
							}
						}
						// re-escape % from an invalid sequence
						else if (ch == UriFromBCL.c_DummyChar)
						{
							if ((unescapeMode & UnescapeMode.Escape) != 0)
								escapeReserved = true;
							else
								continue;   // we should throw instead but since v1.0 would just print '%'
						}
						// Do not unescape '%' itself unless full unescape is requested
						else if (ch == '%')
						{
							next += 2;
							continue;
						}
						// Do not unescape a reserved char unless full unescape is requested
						else if (ch == rsvd1 || ch == rsvd2 || ch == rsvd3)
						{
							next += 2;
							continue;
						}
						// Do not unescape a dangerous char unless it's V1ToStringFlags mode
						else if ((unescapeMode & UnescapeMode.V1ToStringFlag) == 0 && IsNotSafeForUnescape(ch))
						{
							next += 2;
							continue;
						}
						else if (iriParsing && ((ch <= '\x9F' && IsNotSafeForUnescape(ch)) ||
																		(ch > '\x9F' && !IriHelper.CheckIriUnicodeRange(ch))))
						{
							// check if unenscaping gives a char outside iri range
							// if it does then keep it escaped
							next += 2;
							continue;
						}
						// unescape escaped char or escape %
						break;
					}
					else if (unescapeMode >= UnescapeMode.UnescapeAll)
					{
						if (unescapeMode >= UnescapeMode.UnescapeAllOrThrow)
						{
							// Should be a rare case where the app tries to feed an invalid escaped sequence
							throw new UriFormatException("Invalid URI: There is an invalid sequence in the string.");
						}
						// keep a '%' as part of a bogus sequence
						continue;
					}
					else
					{
						escapeReserved = true;
					}
					// escape (escapeReserved==true) or otherwise unescape the sequence
					break;
				}
				else if ((unescapeMode & (UnescapeMode.Unescape | UnescapeMode.UnescapeAll))
						== (UnescapeMode.Unescape | UnescapeMode.UnescapeAll))
				{
					continue;
				}
				else if ((unescapeMode & UnescapeMode.Escape) != 0)
				{
					// Could actually escape some of the characters
					if (ch == rsvd1 || ch == rsvd2 || ch == rsvd3)
					{
						// found an unescaped reserved character -> escape it
						escapeReserved = true;
						break;
					}
					else if ((unescapeMode & UnescapeMode.V1ToStringFlag) == 0
							&& (ch <= '\x1F' || (ch >= '\x7F' && ch <= '\x9F')))
					{
						// found an unescaped reserved character -> escape it
						escapeReserved = true;
						break;
					}
				}
			}

			//copy off previous characters from input
			while (start < next)
				dest.Append(pStr[start++]);

			if (next != end)
			{
				if (escapeReserved)
				{
					PercentEncodeByte((byte)pStr[next], ref dest);
					escapeReserved = false;
					next++;
				}
				else if (ch <= 127)
				{
					dest.Append(ch);
					next += 3;
				}
				else
				{
					// Unicode
					int charactersRead = PercentEncodingHelper.UnescapePercentEncodedUTF8Sequence(
							pStr + next,
							end - next,
							ref dest,
							iriParsing);

					Debug.Assert(charactersRead > 0);
					next += charactersRead;
				}

				start = next;
			}
		}
	}

	internal static void PercentEncodeByte(byte b, ref ValueStringBuilder to)
	{
		to.Append('%');
		HexConverter.ToCharsBuffer(b, to.AppendSpan(2), 0, HexConverter.Casing.Upper);
	}

	/// <summary>
	/// Converts 2 hex chars to a byte (returned in a char), e.g, "0a" becomes (char)0x0A.
	/// <para>If either char is not hex, returns <see cref="Uri.c_DummyChar"/>.</para>
	/// </summary>
	internal static char DecodeHexChars(int first, int second)
	{
		int a = HexConverter.FromChar(first);
		int b = HexConverter.FromChar(second);

		if ((a | b) == 0xFF)
		{
			// either a or b is 0xFF (invalid)
			return UriFromBCL.c_DummyChar;
		}

		return (char)((a << 4) | b);
	}

	internal const string RFC3986ReservedMarks = @";/?:@&=+$,#[]!'()*";
	private const string AdditionalUnsafeToUnescape = @"%\#"; // While not specified as reserved, these are still unsafe to unescape.

	// When unescaping in safe mode, do not unescape the RFC 3986 reserved set:
	// gen-delims  = ":" / "/" / "?" / "#" / "[" / "]" / "@"
	// sub-delims  = "!" / "$" / "&" / "'" / "(" / ")"
	//             / "*" / "+" / "," / ";" / "="
	//
	// In addition, do not unescape the following unsafe characters:
	// excluded    = "%" / "\"
	//
	// This implementation used to use the following variant of the RFC 2396 reserved set.
	// That behavior is now disabled by default, and is controlled by a UriSyntax property.
	// reserved    = ";" | "/" | "?" | "@" | "&" | "=" | "+" | "$" | ","
	// excluded    = control | "#" | "%" | "\"
	internal static bool IsNotSafeForUnescape(char ch)
	{
		if (ch <= '\x1F' || (ch >= '\x7F' && ch <= '\x9F'))
		{
			return true;
		}

		const string NotSafeForUnescape = RFC3986ReservedMarks + AdditionalUnsafeToUnescape;

		return NotSafeForUnescape.Contains(ch);
	}


	//
	// Is this a gen delim char from RFC 3986
	//
	internal static bool IsGenDelim(char ch)
	{
		return (ch == ':' || ch == '/' || ch == '?' || ch == '#' || ch == '[' || ch == ']' || ch == '@');
	}


	// Is this a Bidirectional control char.. These get stripped
	internal static bool IsBidiControlCharacter(char ch) =>
			char.IsBetween(ch, '\u200E', '\u202E') && !char.IsBetween(ch, '\u2010', '\u2029');

	// Strip Bidirectional control characters from this string
	internal static unsafe string StripBidiControlCharacters(ReadOnlySpan<char> strToClean, string? backingString = null)
	{
		Debug.Assert(backingString is null || strToClean.Length == backingString.Length);

		int charsToRemove = 0;

		int indexOfPossibleCharToRemove = strToClean.IndexOfAnyInRange('\u200E', '\u202E');
		if (indexOfPossibleCharToRemove >= 0)
		{
			// Slow path: Contains chars that fall in the [u200E, u202E] range (so likely Bidi)
			foreach (char c in strToClean.Slice(indexOfPossibleCharToRemove))
			{
				if (IsBidiControlCharacter(c))
				{
					charsToRemove++;
				}
			}
		}

		if (charsToRemove == 0)
		{
			// Hot path
			return backingString ?? new string(strToClean);
		}

#pragma warning disable CS8500 // takes address of managed type
		ReadOnlySpan<char> tmpStrToClean = strToClean; // avoid address exposing the span and impacting the other code in the method that uses it
		return string.Create(tmpStrToClean.Length - charsToRemove, (IntPtr)(&tmpStrToClean), static (buffer, strToCleanPtr) =>
		{
			int destIndex = 0;
			foreach (char c in *(ReadOnlySpan<char>*)strToCleanPtr)
			{
				if (!IsBidiControlCharacter(c))
				{
					buffer[destIndex++] = c;
				}
			}
			Debug.Assert(buffer.Length == destIndex);
		});
#pragma warning restore CS8500
	}
}
