using System.Runtime.CompilerServices;

namespace RestFetch;

public interface IOnSseEventError {
	// NOTE: 不用ValueTask,因为Task<bool>有缓存优化不会有堆分配
	// 受限于API,这里不使用ApiContext,仅传递最小必要信息,请求ID和URL
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static virtual Task<bool> OnSseEventError(Exception exp, ApiContext context, CancellationToken cancellationToken) => Task.FromResult(true);
}