using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RestFetch.SourceGenerator;

public static class AttributeDataExtension {
	private static readonly SymbolDisplayFormat TypeWithoutGenericsFormat = new(
		globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
		typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
		genericsOptions: SymbolDisplayGenericsOptions.None
	);
	public static bool IsAttribute(this AttributeData attrData, string fullyQualifiedName, bool isGeneric = false) {
		var attrClass = attrData.AttributeClass;
		if (attrClass is null) return false;

		ReadOnlySpan<char> fullSpan = fullyQualifiedName.AsSpan();

		var lastDotIndex = fullSpan.LastIndexOf('.');
		ReadOnlySpan<char> expectedShortName = lastDotIndex >= 0
			? fullSpan[(lastDotIndex + 1)..]
			: fullSpan;

		var displayStr = isGeneric ? attrClass.ToDisplayString(TypeWithoutGenericsFormat) : attrClass.ToDisplayString();
		return attrClass.IsGenericType == isGeneric && expectedShortName.Equals(attrClass.Name.AsSpan(), StringComparison.Ordinal) && displayStr == fullyQualifiedName;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsAttribute(this AttributeData attrData, ISymbol? symbol)
		=> SymbolEqualityComparer.Default.Equals(attrData.AttributeClass?.OriginalDefinition, symbol);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrimitiveType<T>(this TypedConstant arg)
		=> arg.Kind == TypedConstantKind.Primitive && arg.Value is T;

	public static Location GetTypeArgumentLocation(this AttributeData attributeData, int argumentIndex = 0) {
		if (attributeData.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax) {
			var genericNameSyntax = attributeSyntax.Name
				.DescendantNodesAndSelf()
				.OfType<GenericNameSyntax>()
				.FirstOrDefault();

			if (genericNameSyntax is not null) {
				var typeArguments = genericNameSyntax.TypeArgumentList.Arguments;
				if (argumentIndex >= 0 && argumentIndex < typeArguments.Count) {
					return typeArguments[argumentIndex].GetLocation();
				}
			}

			// 回退整个Attribute范围
			return attributeSyntax.GetLocation();
		}

		return Location.None;
	}
}