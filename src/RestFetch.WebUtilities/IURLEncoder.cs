using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;

namespace RestFetch.WebUtilities;

public interface IURLEncoder {
	// 性能最佳的选择
	// 内部使用
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual string EncodeURIComponent(string? str) => str is null ? "" : UrlEncoder.Default.Encode(str);

	static virtual string EncodeURIComponent(ReadOnlySpan<char> str) {
		char[] destination = ArrayPool<char>.Shared.Rent(str.Length * 3);
		Span<char> destSpan = destination;
		try {
			var status = UrlEncoder.Default.Encode(str, destSpan, out _, out int len);
			switch (status) {
				case OperationStatus.Done:
					return destSpan[..len].ToString();
				// 理论上来讲不可能发生
				case OperationStatus.DestinationTooSmall:
					ThrowInternalBufferOverflowException();
					// 编译器过于弱智推导不出来这里是never
					return string.Empty;
				case OperationStatus.InvalidData:
					ThrowInvalidDataException(str);
					return string.Empty;
				case OperationStatus.NeedMoreData:
					ThrowInvalidDataException(str);
					return string.Empty;
				default:
					ThrowNotImplementedException();
					return string.Empty;
			}
		} finally {
			ArrayPool<char>.Shared.Return(destination);
		}

		[DoesNotReturn]
		static void ThrowInvalidDataException(ReadOnlySpan<char> str) => throw new InvalidDataException($"Invalid URI component: {str}");
		[DoesNotReturn]
		static void ThrowNotImplementedException() => throw new NotImplementedException();
		[DoesNotReturn]
		static void ThrowInternalBufferOverflowException() => throw new InternalBufferOverflowException("Destination buffer too small.");
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual string DecodeURIComponent(string? str) => str is null ? "" : Uri.UnescapeDataString(str);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual string DecodeURIComponent(ReadOnlySpan<char> str) => Uri.UnescapeDataString(str);
}