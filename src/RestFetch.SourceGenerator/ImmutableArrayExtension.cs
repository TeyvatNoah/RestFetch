using System.Collections.Immutable;

namespace RestFetch.SourceGenerator;

public static class ImmutableArrayExtension {

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