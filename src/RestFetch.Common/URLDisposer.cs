using System.Runtime.CompilerServices;

namespace RestFetch;

public class URLDisposer: IDisposer<URLProxy> {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Dispose(ref URLProxy instance) {}
}
