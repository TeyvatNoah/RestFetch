using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace RestFetch.SourceGenerator;

public static class ImmutableArrayExtension {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImmutableArray<T> ToImmutableArraySafe<T>(this IEnumerable<T>? source)
		=> source is null
			? ImmutableArray<T>.Empty
			: source is ImmutableArray<T> immutableArray
			? immutableArray.IsDefault ? ImmutableArray<T>.Empty : immutableArray
			: source.ToImmutableArray();

	public static ImmutableArray<T> CombineSafe<T>(params IEnumerable<T>?[] sources) {
		var builder = ImmutableArray.CreateBuilder<T>();

		foreach (var source in sources) {
			if (source is null) continue;

			if (source is ImmutableArray<T> array) {
				if (!array.IsDefaultOrEmpty) {
					builder.AddRange(array);
				}
				continue;
			}
			builder.AddRange(source);
		}

		return builder.ToImmutable();
	}
}