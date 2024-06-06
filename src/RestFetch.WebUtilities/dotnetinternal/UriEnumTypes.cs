// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace RestFetch.WebUtilities;

[Flags]
internal enum UnescapeMode
{
	CopyOnly = 0x0,                          // used for V1.0 ToString() compatibility mode only
	Escape = 0x1,                            // Only used by ImplicitFile, the string is already fully unescaped
	Unescape = 0x2,                          // Only used as V1.0 UserEscaped compatibility mode
	EscapeUnescape = Unescape | Escape,      // does both escaping control+reserved and unescaping of safe characters
	V1ToStringFlag = 0x4,                    // Only used as V1.0 ToString() compatibility mode, assumes DontEscape level also
	UnescapeAll = 0x8,                       // just unescape everything, leave bad escaped sequences as is
	UnescapeAllOrThrow = 0x10 | UnescapeAll, // just unescape everything plus throw on bad escaped sequences
}
