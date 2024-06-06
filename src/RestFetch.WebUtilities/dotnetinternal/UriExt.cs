// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace RestFetch.WebUtilities;
public static class UriFromBCL
{
	internal const int StackallocThreshold = 512;
	internal const char c_DummyChar = (char)0xFFFF;     //An Invalid Unicode character used as a dummy char passed into the parameter

	public static string UnescapeDataString(string stringToUnescape)
	{
		if (stringToUnescape.Length == 0)
			return string.Empty;

		int position = stringToUnescape.IndexOf('%');
		if (position == -1)
			return stringToUnescape;

		var vsb = new ValueStringBuilder(stackalloc char[StackallocThreshold]);
		vsb.EnsureCapacity(stringToUnescape.Length);

		vsb.Append(stringToUnescape.AsSpan(0, position));
		UriHelper.UnescapeString(
				stringToUnescape, position, stringToUnescape.Length, ref vsb,
				c_DummyChar, c_DummyChar, c_DummyChar,
				UnescapeMode.Unescape | UnescapeMode.UnescapeAll);

		return vsb.ToString();
	}

	public static string UnescapeDataString(ReadOnlySpan<char> stringToUnescape)
	{

		if (stringToUnescape.Length == 0)
			return string.Empty;

		int position = stringToUnescape.IndexOf('%');
		if (position == -1)
			return new string(stringToUnescape);

		var vsb = new ValueStringBuilder(stackalloc char[StackallocThreshold]);
		vsb.EnsureCapacity(stringToUnescape.Length);

		vsb.Append(stringToUnescape[0..position]);
		UriHelper.UnescapeString(
				stringToUnescape, position, stringToUnescape.Length, ref vsb,
				c_DummyChar, c_DummyChar, c_DummyChar,
				UnescapeMode.Unescape | UnescapeMode.UnescapeAll);

		return vsb.ToString();
	}

}
