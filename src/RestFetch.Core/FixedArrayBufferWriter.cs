using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RestFetch;

public sealed class FixedArrayBufferWriter(byte[] buffer): IBufferWriter<byte> {
	private readonly byte[] _buffer = buffer;

	private int _written;

	public int WrittenCount => _written;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EnsureCapacity(int sizeHint) {
		ArgumentOutOfRangeException.ThrowIfNegative(sizeHint, nameof(sizeHint));

		sizeHint = Math.Max(sizeHint, 1);

		if (sizeHint > _buffer.Length - _written) {
			ThrowInvalidOperation();
		}
	}

	[DoesNotReturn]
	private static void ThrowInvalidOperation() {
		throw new InvalidOperationException("The buffer is too small.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Advance(int count) {
		ArgumentOutOfRangeException.ThrowIfGreaterThan(count, _buffer.Length - _written);
		_written += count;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Memory<byte> GetMemory(int sizeHint = 0) {
		EnsureCapacity(sizeHint);
		return _buffer.AsMemory(_written);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<byte> GetSpan(int sizeHint = 0) {
		EnsureCapacity(sizeHint);
		return _buffer.AsSpan(_written);
	}
}