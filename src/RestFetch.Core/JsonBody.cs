using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RestFetch;

public sealed class JsonBody<T>(T data, int initialCapacity): IDisposable where T: notnull {
	private readonly Lock _lock = new();

	private byte[]? _buffer;

	private int _written;

	private bool _disposed;


	public JsonBody(T data): this(data, 102400) {}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static JsonWriterOptions CreateWriterOptions(JsonSerializerOptions options) => new() {
		Indented = options.WriteIndented,
		Encoder = options.Encoder,
		SkipValidation = false
	};

	public ReadOnlyMemory<byte> Create(JsonTypeInfo<T> jsonTypeInfo) {
		ObjectDisposedException.ThrowIf(_disposed, this);

		if (_buffer is not null) {
			return _buffer.AsMemory(0, _written);
		}

		lock (_lock) {
			ObjectDisposedException.ThrowIf(_disposed, this);

			if (_buffer is not null) {
				return _buffer.AsMemory(0, _written);
			}

			var buffer = ArrayPool<byte>.Shared.Rent(initialCapacity);

			var destination = new FixedArrayBufferWriter(buffer);


			try {
				using var utf8Writer = new Utf8JsonWriter(destination, CreateWriterOptions(jsonTypeInfo.Options));
				JsonSerializer.Serialize(utf8Writer, data, jsonTypeInfo);
				utf8Writer.Flush();
			} catch {
				ArrayPool<byte>.Shared.Return(buffer);
				_disposed = true;
				_buffer = null;
				_written = 0;
				throw;
			}

			_written = destination.WrittenCount;

			_buffer = buffer;
		}

		return _buffer.AsMemory(0, _written);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
	[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlyMemory<byte> Create(JsonSerializerOptions jsonSerializerOptions) {
		ObjectDisposedException.ThrowIf(_disposed, this);

		if (_buffer is not null) {
			return _buffer.AsMemory(0, _written);
		}

		lock (_lock) {
			ObjectDisposedException.ThrowIf(_disposed, this);

			if (_buffer is not null) {
				return _buffer.AsMemory(0, _written);
			}

			var buffer = ArrayPool<byte>.Shared.Rent(initialCapacity);

			var destination = new FixedArrayBufferWriter(buffer);


			try {
				using var utf8Writer = new Utf8JsonWriter(destination, CreateWriterOptions(jsonSerializerOptions));
				JsonSerializer.Serialize(utf8Writer, data, jsonSerializerOptions);
				utf8Writer.Flush();
			} catch {
				ArrayPool<byte>.Shared.Return(buffer);
				_disposed = true;
				_buffer = null;
				_written = 0;
				throw;
			}

			_written = destination.WrittenCount;

			_buffer = buffer;
		}

		return _buffer.AsMemory(0, _written);
	}

	public void Dispose() {
		lock (_lock) {
			if (_disposed) {
				return;
			}
			_disposed = true;

			var buffer = _buffer;
			_buffer = null;
			_written = 0;

			if (buffer is not null) {
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator JsonBody<T>(T obj) => new(obj);
}