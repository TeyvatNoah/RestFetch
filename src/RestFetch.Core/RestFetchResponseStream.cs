namespace RestFetch;

public sealed class RestFetchResponseStream(HttpResponseMessage response, Stream stream): DelegatingStream(stream) {
	private int _isDisposed;

	private readonly HttpResponseMessage _response = response;

	protected override void Dispose(bool disposing) {
		if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0) {
			if (disposing) {
				_response.Dispose();
			}
		}

		base.Dispose(disposing);
	}

	protected override ValueTask DisposeAsyncCore() {
		if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0) {
			_response.Dispose();
		}

		return base.DisposeAsyncCore();
	}
}