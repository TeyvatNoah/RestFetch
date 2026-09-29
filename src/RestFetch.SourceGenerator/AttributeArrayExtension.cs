using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public static class AttributeArrayExtension {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<AttributeData> GetAttributesByFullName(this ImmutableArray<AttributeData> attrs, string fullName, bool isGeneric = false)
		=> attrs.Where(item => item.IsAttribute(fullName, isGeneric));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<AttributeData> GetAttributesByFullName(this ImmutableArray<AttributeData> attrs, string metadataName, Compilation compilation)
		=> attrs.Where(item => item.IsAttribute(compilation.GetTypeByMetadataName(metadataName)));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<AttributeData> GetAttributesBySymbol(this ImmutableArray<AttributeData> attrs, ISymbol? symbol)
		=> attrs.Where(item => item.IsAttribute(symbol));
}