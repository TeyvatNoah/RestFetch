using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public static class ITypeSymbolExtension {
	public static bool ImplementsInterface(this ITypeSymbol self, INamedTypeSymbol? interfaceSymbol) {
		if (self is null || interfaceSymbol is null) return false;

		// 提取其开放泛型
		var targetDefinition = interfaceSymbol.IsGenericType
		? interfaceSymbol.OriginalDefinition
		: interfaceSymbol;

		if (IsMatch(self, targetDefinition)) {
			return true;
		}

		foreach (var iface in self.AllInterfaces) {
			if (IsMatch(iface, targetDefinition)) {
				return true;
			}
		}

		return false;

		static bool IsMatch(ITypeSymbol current, INamedTypeSymbol target) {
			var currentDefinition = current is INamedTypeSymbol { IsGenericType: true } named
			? named.OriginalDefinition
			: current;

			return SymbolEqualityComparer.Default.Equals(currentDefinition, target);
		}
	}
	public static bool InheritsFrom(this ITypeSymbol? symbol, INamedTypeSymbol? baseTypeSymbol) {
		if (symbol is null || baseTypeSymbol is null) return false;

		if (SymbolEqualityComparer.Default.Equals(symbol, baseTypeSymbol)) {
			return true;
		}

		var current = symbol.BaseType;
		while (current is not null) {
			if (SymbolEqualityComparer.Default.Equals(current, baseTypeSymbol)) {
				return true;
			}
			current = current.BaseType;
		}

		return false;
	}

	public static IEnumerable<IMethodSymbol> GetInstanceMethods(this ITypeSymbol symbol) {
		if (symbol.TypeKind != TypeKind.Class && symbol.TypeKind != TypeKind.Struct && symbol.TypeKind != TypeKind.Interface) {
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ITypeSymbol? GetTFromNullable(this ITypeSymbol? self)
		=> self is INamedTypeSymbol {
			IsValueType: true,
			IsGenericType: true,
			OriginalDefinition.SpecialType: SpecialType.System_Nullable_T,
			TypeArguments.Length: > 0
		} nullableT
		? nullableT.TypeArguments[0]
		: self;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullable(this ITypeSymbol? self)
		=> self is not null && (self.IsValueType
			? self is INamedTypeSymbol { IsGenericType: true, OriginalDefinition.SpecialType: SpecialType.System_Nullable_T }
			: self.NullableAnnotation == NullableAnnotation.Annotated);

	public static bool IsAsyncReturnType(this ITypeSymbol returnType, KnownTypes knownTypes) {
		if (returnType.SpecialType == SpecialType.System_Void) {
			return false;
		}

		var realReturnType = returnType.OriginalDefinition;
		// var realReturnType = returnType is INamedTypeSymbol genericReturnType
		// 	? genericReturnType.IsGenericType
		// 		? genericReturnType.OriginalDefinition
		// 		: genericReturnType
		// 	: returnType;
		var comparer = SymbolEqualityComparer.Default;
		if (comparer.Equals(realReturnType, knownTypes.Task)
			|| comparer.Equals(realReturnType, knownTypes.TaskT)
			|| comparer.Equals(realReturnType, knownTypes.ValueTask)
			|| comparer.Equals(realReturnType, knownTypes.ValueTask)
			|| comparer.Equals(realReturnType, knownTypes.IAsyncEnumerableT)
		) {
			return true;
		}

		if (knownTypes.AsyncMethodBuilderAttribute is not null) {
			foreach (var attr in realReturnType.GetAttributes()) {
				if (comparer.Equals(knownTypes.AsyncMethodBuilderAttribute, attr.AttributeClass)) {
					return true;
				}
			}
		}

		return false;
	}

	public static ReturnTypeKind GetAsyncReturnTypeKind(this ITypeSymbol returnType, KnownTypes knownTypes) {
		if (returnType.SpecialType == SpecialType.System_Void) {
			return ReturnTypeKind.Void;
		}

		var realReturnType = returnType.OriginalDefinition;
		var comparer = SymbolEqualityComparer.Default;
		if (comparer.Equals(realReturnType, knownTypes.Task)) {
			return ReturnTypeKind.Task;
		} else if (comparer.Equals(realReturnType, knownTypes.TaskT)) {
			return ReturnTypeKind.TaskT;
		} else if (comparer.Equals(realReturnType, knownTypes.ValueTask)) {
			return ReturnTypeKind.ValueTask;
		} else if (comparer.Equals(realReturnType, knownTypes.ValueTask)) {
			return ReturnTypeKind.ValueTaskT;
		} else if (comparer.Equals(realReturnType, knownTypes.IAsyncEnumerableT)) {
			return ReturnTypeKind.IAsyncEnumerableT;
		}

		if (knownTypes.AsyncMethodBuilderAttribute is not null) {
			foreach (var attr in realReturnType.GetAttributes()) {
				if (comparer.Equals(knownTypes.AsyncMethodBuilderAttribute, attr.AttributeClass)) {
					return ReturnTypeKind.CustomTask;
				}
			}
		}

		return ReturnTypeKind.NotAsync;
	}
}