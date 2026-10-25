namespace RestFetch;

public class DelegatingStream(Stream innerStream): Stream() {
	private int _isDisposed;
	private readonly Stream _innerStream = innerStream;

	public override bool CanRead => _innerStream.CanRead;

	public override bool CanSeek => _innerStream.CanSeek;

	public override bool CanTimeout => _innerStream.CanTimeout;

	public override bool CanWrite => _innerStream.CanWrite;

	public override long Length => _innerStream.Length;

	public override long Position { get => _innerStream.Position; set => _innerStream.Position = value; }

	public override int ReadTimeout { get => _innerStream.ReadTimeout; set => _innerStream.ReadTimeout = value; }

	public override int WriteTimeout { get => _innerStream.WriteTimeout; set => _innerStream.WriteTimeout = value; }


	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
		=> _innerStream.BeginRead(buffer, offset, count, callback, state);

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
		=> _innerStream.BeginWrite(buffer, offset, count, callback, state);


	public override void Close() => _innerStream.Close();

	public override void CopyTo(Stream destination, int bufferSize) => _innerStream.CopyTo(destination, bufferSize);

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
		=> _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);

	protected override void Dispose(bool disposing) {
		if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0) {
			if (disposing) {
				_innerStream.Dispose();
			}
		}
		base.Dispose(disposing);
	}

	public override async ValueTask DisposeAsync() {
		await DisposeAsyncCore().ConfigureAwait(false);
		Dispose(false);
		await base.DisposeAsync().ConfigureAwait(false);
		GC.SuppressFinalize(this);
	}

	protected virtual async ValueTask DisposeAsyncCore() {
		if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0) {
			await _innerStream.DisposeAsync();
		}
		Dispose(true);
	}

	public override int EndRead(IAsyncResult asyncResult) => _innerStream.EndRead(asyncResult);

	public override void EndWrite(IAsyncResult asyncResult) => _innerStream.EndWrite(asyncResult);

	public override void Flush() => _innerStream.Flush();

	public override Task FlushAsync(System.Threading.CancellationToken cancellationToken) => _innerStream.FlushAsync(cancellationToken);

	public override int Read(Span<byte> buffer) => _innerStream.Read(buffer);

	public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);

	public override ValueTask<int> ReadAsync(Memory<byte> buffer, System.Threading.CancellationToken cancellationToken = default)
		=> _innerStream.ReadAsync(buffer, cancellationToken);

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		=> _innerStream.ReadAsync(buffer, offset, count, cancellationToken);

	public override int ReadByte() => _innerStream.ReadByte();

	public override long Seek(long offset, System.IO.SeekOrigin origin) => _innerStream.Seek(offset, origin);

	public override void SetLength(long value) => _innerStream.SetLength(value);

	public override void Write(ReadOnlySpan<byte> buffer) => _innerStream.Write(buffer);

	public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);

	public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, System.Threading.CancellationToken cancellationToken = default)
		=> _innerStream.WriteAsync(buffer, cancellationToken);

	public override Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		=> _innerStream.WriteAsync(buffer, offset, count, cancellationToken);

	public override void WriteByte(byte value) => _innerStream.WriteByte(value);
}