namespace RestFetch.Extensions;

internal static class DefaultHttpClientName<TApi> {
	internal static readonly string Value = $"RestFetch.HttpClientProxy.{Interlocked.Increment(ref HttpClientNameCounter.Value)}";

	internal static class HttpClientNameCounter {
		internal static long Value;
	}
}