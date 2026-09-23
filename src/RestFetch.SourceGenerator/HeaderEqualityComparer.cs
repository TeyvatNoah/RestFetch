using System.Runtime.CompilerServices;

namespace RestFetch.SourceGenerator;

public sealed class HeaderEqualityComparer : IEqualityComparer<KeyValuePairInfo> {
	public static readonly HeaderEqualityComparer Default = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(KeyValuePairInfo x, KeyValuePairInfo y) => x.Key.Equals(y.Key, StringComparison.OrdinalIgnoreCase);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetHashCode(KeyValuePairInfo obj) => obj.Key.GetHashCode();
}