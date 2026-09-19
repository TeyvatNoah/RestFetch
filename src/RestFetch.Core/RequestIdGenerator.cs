using System.Runtime.CompilerServices;

namespace RestFetch;

public class RequestIdGenerator: IRequestIdGenerator {
	private static long _CurrentId = DateTime.UtcNow.Ticks;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RequestId NextId() => Interlocked.Increment(ref _CurrentId);
}