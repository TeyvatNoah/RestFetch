using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;
public sealed record NamespaceInfo(
	string Name,
	string FullyQualifiedName,
	NamespaceInfo? ContainingNamespace
) {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NamespaceInfo CreateFromSymbol(INamespaceSymbol symbol)
		=> new(
				Name: symbol.Name,
				FullyQualifiedName: symbol.ToDisplayString(),
				ContainingNamespace: symbol.ContainingNamespace is null ? null : CreateFromSymbol(symbol.ContainingNamespace)
			);
}