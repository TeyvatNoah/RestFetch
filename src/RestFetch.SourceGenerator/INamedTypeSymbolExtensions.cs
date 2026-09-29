using System.Runtime.CompilerServices;

using Cysharp.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RestFetch;

public static class INamedTypeSymbolExtensions {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInterface(this INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Interface;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPartialClass(this INamedTypeSymbol symbol)
		=> symbol.TypeKind == TypeKind.Class && symbol.DeclaringSyntaxReferences
			.Select(static syntaxRef => syntaxRef.GetSyntax())
			.OfType<TypeDeclarationSyntax>()
			.All(static typeDecl => typeDecl.Modifiers.Any(static (m) => m.IsKind(SyntaxKind.PartialKeyword)));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPartial(this INamedTypeSymbol symbol)
		=> symbol.DeclaringSyntaxReferences
			.Select(static syntaxRef => syntaxRef.GetSyntax())
			.OfType<TypeDeclarationSyntax>()
			.All(static typeDecl => typeDecl.Modifiers.Any(static (m) => m.IsKind(SyntaxKind.PartialKeyword)));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPartialStruct(this INamedTypeSymbol symbol)
		=> symbol.TypeKind == TypeKind.Struct && symbol.DeclaringSyntaxReferences
			.Select(static syntaxRef => syntaxRef.GetSyntax())
			.OfType<TypeDeclarationSyntax>()
			.All(static typeDecl => typeDecl.Modifiers.Any(static (m) => m.IsKind(SyntaxKind.PartialKeyword)));

	public static IEnumerable<IMethodSymbol> GetInterfaceMethods(this INamedTypeSymbol symbol) {
		if (symbol.TypeKind != TypeKind.Interface) {
			yield break;
		}
		foreach (var member in symbol.GetMembers()) {
			if (member is IMethodSymbol {
				MethodKind: MethodKind.Ordinary
			} method) {
				yield return method;
			}
		}
	}

	public static IEnumerable<IMethodSymbol> GetSelfAndInheritedInterfaceMethods(this INamedTypeSymbol symbol) {
		if (symbol.TypeKind != TypeKind.Interface) {
			yield break;
		}
		var allInterfaces = Enumerable.Repeat(symbol, 1).Concat(symbol.AllInterfaces);
		foreach (var @interface in allInterfaces) {
			foreach (var member in @interface.GetMembers()) {
				if (member is IMethodSymbol {
					MethodKind: MethodKind.Ordinary
				} method) {
					yield return method;
				}
			}
		}
	}


	public static string GetWhereClauses(this INamedTypeSymbol typeSymbol) {
		if (!typeSymbol.IsGenericType || typeSymbol.TypeParameters.IsEmpty) return string.Empty;

		using var builder = ZString.CreateStringBuilder();

		foreach (var typeParam in typeSymbol.TypeParameters) {
			var constraints = new List<string>();

			if (typeParam.HasUnmanagedTypeConstraint) {
				constraints.Add("unmanaged");
			} else if (typeParam.HasValueTypeConstraint) {
				constraints.Add("struct");
			} else if (typeParam.HasReferenceTypeConstraint) {
				constraints.Add(typeParam.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated 
					? "class?"
					: "class");
			} else if (typeParam.HasNotNullConstraint) {
				constraints.Add("notnull");
			}

			foreach (var constraintType in typeParam.ConstraintTypes) {
				constraints.Add(constraintType.ToDisplayString(SourceGenerator.TypeInfo.FullyQualifiedFormat));
			}

			if (typeParam.HasConstructorConstraint) {
				constraints.Add("new()");
			}

			if (typeParam.AllowsRefLikeType) {
				constraints.Add("allows ref struct");
			}

			if (constraints.Count > 0) {
				builder.Append("where ");
				builder.Append(typeParam.Name);
				builder.Append(" : ");
				builder.AppendJoin(", ", constraints);
				builder.Append('\n');
			}
		}

		return builder.ToString();
	}

}