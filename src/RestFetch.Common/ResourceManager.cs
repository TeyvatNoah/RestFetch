using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RestFetch;

public sealed class ResourceManager: IDisposable, IAsyncDisposable {
	private bool _isDisposed;

	private readonly List<IDisposable> _resources = new(4);

	private readonly List<IAsyncDisposable> _asyncResources = new(2);

	private readonly Lock _lock = new();

	public T TakeOwnership<T>(T resource) where T: IDisposable {
		if (resource is null) {
			return resource!;
		}

		var disposeImmediately = false;

		lock (_lock) {
			if (_isDisposed) {
				disposeImmediately = true;
			} else {
				_resources.Add(resource);
			}
		}

		if (disposeImmediately) {
			resource.Dispose();
		}

		return resource;
	}

	// NOTE: 这里不给out参数了,也没什么太大的用
	public bool TryTakeOwnership<T>(T resource) where T: IDisposable {
		if (resource is null) {
			return false;
		}


		lock (_lock) {
			if (_isDisposed) {
				return false;
			} else {
				_resources.Add(resource);
			}
		}

		return true;
	}

	public async Task TakeOwnershipAsync<T>(T resource) where T: IAsyncDisposable {
		if (resource is null) {
			return;
		}

		var disposeImmediately = false;

		lock (_lock) {
			if (_isDisposed) {
				disposeImmediately = true;
			} else {
				_asyncResources.Add(resource);
			}
		}

		if (disposeImmediately) {
			await resource.DisposeAsync().ConfigureAwait(false);
		}
	}

	public async Task<bool> TryTakeOwnershipAsync<T>(T resource) where T: IAsyncDisposable {
		if (resource is null) {
			return false;
		}


		lock (_lock) {
			if (_isDisposed) {
				return false;
			} else {
				_asyncResources.Add(resource);
			}
		}

		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Absolve<T>(T resource) where T: IDisposable {
		lock(_lock) {
			return _resources.Remove(resource);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool AbsolveAsync<T>(T resource) where T: IAsyncDisposable {
		lock(_lock) {
			return _asyncResources.Remove(resource);
		}
	}

	public void Dispose() {
		IDisposable[]? resourcesSnapshot = default;

		lock (_lock) {
			if (_isDisposed) {
				return;
			}
			_isDisposed = true;
			if (_resources.Count > 0) {
				resourcesSnapshot = new IDisposable[_resources.Count];
				var resourcesSpan = CollectionsMarshal.AsSpan(_resources);
				for (int i = 0, len = resourcesSpan.Length - 1, count = resourcesSpan.Length; i < count; ++i) {
					resourcesSnapshot[i] = resourcesSpan[len - i];
				}
				_resources.Clear();
			}
		}

		if (resourcesSnapshot is null) {
			return;
		}

		List<Exception>? exceptions = null;

		foreach (var res in resourcesSnapshot) {
			try {
				res?.Dispose();
			} catch(Exception e) {
				exceptions ??= [];
				exceptions.Add(e);
			}
		}

		if (exceptions is not null) {
			ThrowAggregateException(exceptions);
		}
	}

	public async ValueTask DisposeAsync() {
		List<Exception>? exceptions = null;
		try {
			Dispose();
		} catch(AggregateException ex) {
			exceptions ??= [];
			if (ex is AggregateException aggEx) {
				exceptions.AddRange(aggEx.InnerExceptions);
			} else {
				exceptions.Add(ex);
			}
		}

		IAsyncDisposable[]? resourcesSnapshot = default;

		lock (_lock) {
			if (_asyncResources.Count > 0) {
				resourcesSnapshot = new IAsyncDisposable[_asyncResources.Count];
				var resourcesSpan = CollectionsMarshal.AsSpan(_asyncResources);
				for (int i = 0, len = resourcesSpan.Length - 1, count = resourcesSpan.Length; i < count; ++i) {
					resourcesSnapshot[i] = resourcesSpan[len - i];
				}
				_asyncResources.Clear();
			}
		}

		if (resourcesSnapshot is not null) {
			foreach (var res in resourcesSnapshot) {
				try {
					// NOTE: 不能保证res非null只能尽力
					if (res is not null) {
						await res.DisposeAsync().ConfigureAwait(false);
					}
				} catch(Exception e) {
					exceptions ??= [];
					exceptions.Add(e);
				}
			}
		}

		if (exceptions is not null && exceptions.Count > 0) {
			ThrowAggregateException(exceptions);
		}
	}

	[DoesNotReturn]
	private static void ThrowAggregateException(IEnumerable<Exception> exps) {
		throw new AggregateException("One or more resources failed to dispose during request cleanup.", exps);
	}
}