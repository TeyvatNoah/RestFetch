using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;

namespace RestFetch.WebUtilities;

// public需要给用户作为泛型参数使用
public class URLHelper: IURLEncoder, IURLDecoder {
	// 性能最佳的选择
	// 内部使用
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string EncodeURIComponent(string? str) => str is null ? "" : UrlEncoder.Default.Encode(str);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string EncodeURIComponent(ReadOnlySpan<char> str) {
		char[] destination = ArrayPool<char>.Shared.Rent(str.Length * 3);
		Span<char> destSpan = destination;
		var result = UrlEncoder.Default.Encode(str, destSpan, out _, out int len) switch {
			OperationStatus.Done => destSpan[..len].ToString(),
			// 理论上来讲不可能发生
			OperationStatus.DestinationTooSmall => throw new InternalBufferOverflowException("Destination buffer too small."),
			OperationStatus.InvalidData => throw new InvalidDataException($"Invalid URI component: {str.ToString()}"),
			OperationStatus.NeedMoreData => throw new InvalidDataException($"Invalid URI component: {str.ToString()}"),
			_ => throw new NotImplementedException(),
		};
		ArrayPool<char>.Shared.Return(destination);
		return result;
	}

	// 性能最佳的选择
	// 内部使用
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string DecodeURIComponent(string? str) => str is null ? "" : UriFromBCL.UnescapeDataString(str);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string DecodeURIComponent(ReadOnlySpan<char> str) => UriFromBCL.UnescapeDataString(str);

	// [MethodImpl(MethodImplOptions.AggressiveInlining)]
	// public static string DecodeURIComponentUri(string? str) => str is null ? "" : UriFromBCL.UnescapeDataString(str);

	// // TODO 如果有天Uri内置了ReadOnlySpan<char>重载则改回来
	// [MethodImpl(MethodImplOptions.AggressiveInlining)]
	// public static string DecodeURIComponentUri(ReadOnlySpan<char> str) => UriFromBCL.UnescapeDataString(str);
}