using System.Runtime.CompilerServices;

namespace RestFetch;

public static class IAsyncEnumerableTExtensions {
	public static async IAsyncEnumerable<T> Catch<T>(this IAsyncEnumerable<T> self, bool configureAwait, Func<Exception, Task> onError, [EnumeratorCancellation]CancellationToken token) {
		IAsyncEnumerator<T>? enumerator = null;

		try {
			// GetAsyncEnumerator 本身也可能抛出异常。
			try {
				enumerator = self.GetAsyncEnumerator(token);
			} catch (Exception e) {
				await onError(e).ConfigureAwait(configureAwait);
				throw;
			}

			while (true) {
				T item;

				// 这里只捕获 MoveNextAsync 和 Current 访问过程中的异常。
				try {
					if (!await enumerator.MoveNextAsync().ConfigureAwait(configureAwait)) {
						break;
					}

					item = enumerator.Current;
				} catch (Exception e) {
					await onError(e).ConfigureAwait(configureAwait);
					throw;
				}

				yield return item;
			}
		} finally {
			// 即使调用者提前停止枚举，也释放底层枚举器。
			if (enumerator is not null) {
				try {
					await enumerator.DisposeAsync().ConfigureAwait(configureAwait);
				} catch (Exception e) {
					await onError(e).ConfigureAwait(configureAwait);
				}
			}
#pragma warning disable CA2219 // Do not raise exceptions in finally clauses
			// throw;
#pragma warning restore CA2219 // Do not raise exceptions in finally clauses
		}
	}
}