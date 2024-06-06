// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace RestFetch.WebUtilities;
internal static class IriHelper
{
	//
	// Checks if provided non surrogate char lies in iri range
	// This method implements the ABNF checks per https://tools.ietf.org/html/rfc3987#section-2.2
	//
	internal static bool CheckIriUnicodeRange(char unicode)
	{
		return IsInInclusiveRange(unicode, '\u00A0', '\uD7FF')
				|| IsInInclusiveRange(unicode, '\uF900', '\uFDCF')
				|| IsInInclusiveRange(unicode, '\uFDF0', '\uFFEF');
	}


	internal static bool CheckIriUnicodeRange(uint value)
	{
		if (value <= 0xFFFF)
		{
			return IsInInclusiveRange(value, '\u00A0', '\uD7FF')
					|| IsInInclusiveRange(value, '\uF900', '\uFDCF')
					|| IsInInclusiveRange(value, '\uFDF0', '\uFFEF');
		}
		else
		{
			return ((value & 0xFFFF) < 0xFFFE)
					&& !IsInInclusiveRange(value, 0xE0000, 0xE0FFF)
					&& value < 0xF0000;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInInclusiveRange(uint value, uint min, uint max)
			=> (value - min) <= (max - min);

}
