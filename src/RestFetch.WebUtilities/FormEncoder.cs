using System.Runtime.CompilerServices;

namespace RestFetch.WebUtilities;

// public需要给用户作为泛型参数使用
public class FormEncoder: IURLEncoder {
	// 性能最佳的选择
	// 内部使用
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string EncodeURIComponent(string? str) => str is null ? "" : WebUtility.UrlEncode(str);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string EncodeURIComponent(ReadOnlySpan<char> str) => WebUtility.UrlEncode(str);

}