using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public static class ITypeSymbolExtension {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsMatch(ITypeSymbol current, INamedTypeSymbol target) {
		var currentDefinition = current is INamedTypeSymbol { IsGenericType: true } named
		? named.OriginalDefinition
		: current;
		return SymbolEqualityComparer.Default.Equals(currentDefinition, target);
	}

	public static bool ImplementsInterface(this ITypeSymbol? self, INamedTypeSymbol? interfaceSymbol) {
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
	}

	public static INamedTypeSymbol? GetInterface(this ITypeSymbol? self, INamedTypeSymbol? interfaceSymbol) {
		if (self is null || interfaceSymbol is null) return null;

		// 提取其开放泛型
		var targetDefinition = interfaceSymbol.IsGenericType
		? interfaceSymbol.OriginalDefinition
		: interfaceSymbol;

		if (IsMatch(self, targetDefinition)) {
			return (INamedTypeSymbol)self;
		}

		foreach (var iface in self.AllInterfaces) {
			if (IsMatch(iface, targetDefinition)) {
				return iface;
			}
		}

		return null;
	}

	public static bool InheritsFrom(this ITypeSymbol? symbol, ITypeSymbol? baseTypeSymbol) {
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
	public static ITypeSymbol[] GetTGeneric(this ITypeSymbol? self)
		=> self is INamedTypeSymbol {
			IsGenericType: true,
			TypeArguments.Length: > 0
		} genericT
		? genericT.TypeArguments.ToArray()
		: Array.Empty<ITypeSymbol>();

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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsObservable(this ITypeSymbol returnType, KnownTypes knownTypes)
		=> returnType.SpecialType != SpecialType.System_Void && SymbolEqualityComparer.Default.Equals(returnType.OriginalDefinition, knownTypes.IObservable);

	public static ReturnTypeWrapperKind GetAsyncReturnTypeKind(this ITypeSymbol returnType, KnownTypes knownTypes) {
		if (returnType.SpecialType == SpecialType.System_Void) {
			return ReturnTypeWrapperKind.Void;
		}

		var realReturnType = returnType.OriginalDefinition;
		var comparer = SymbolEqualityComparer.Default;
		if (comparer.Equals(realReturnType, knownTypes.Task)) {
			return ReturnTypeWrapperKind.Task;
		} else if (comparer.Equals(realReturnType, knownTypes.TaskT)) {
			return ReturnTypeWrapperKind.TaskT;
		} else if (comparer.Equals(realReturnType, knownTypes.ValueTask)) {
			return ReturnTypeWrapperKind.ValueTask;
		} else if (comparer.Equals(realReturnType, knownTypes.ValueTaskT)) {
			return ReturnTypeWrapperKind.ValueTaskT;
		} else if (comparer.Equals(realReturnType, knownTypes.IAsyncEnumerableT)) {
			return ReturnTypeWrapperKind.IAsyncEnumerableT;
		} else if (comparer.Equals(realReturnType, knownTypes.IObservable)) {
			return ReturnTypeWrapperKind.IObservableT;
		}

		if (knownTypes.AsyncMethodBuilderAttribute is not null) {
			foreach (var attr in realReturnType.GetAttributes()) {
				if (comparer.Equals(knownTypes.AsyncMethodBuilderAttribute, attr.AttributeClass)) {
					return ReturnTypeWrapperKind.CustomTask;
				}
			}
		}

		return ReturnTypeWrapperKind.NotAsync;
	}

	public static ReturnTypeKind GetResponseTypeKind(this ITypeSymbol returnType, KnownTypes knownTypes, bool returnsRawResponse) {
		if (returnType.SpecialType == SpecialType.System_Void) {
			return ReturnTypeKind.Void;
		}

		if (returnType is INamedTypeSymbol {
			IsGenericType: true,
			TypeArguments: [var respType, ..]
		}) {
			var maybeSseOrApiResponse = respType.OriginalDefinition;
			var comparer = SymbolEqualityComparer.Default;
			var isSseEventT = comparer.Equals(maybeSseOrApiResponse, knownTypes.SseEventT);
			var isSseItemT = comparer.Equals(maybeSseOrApiResponse, knownTypes.SseItemT);
			var isApiResponseT = comparer.Equals(maybeSseOrApiResponse, knownTypes.ApiResponseT);
			if (respType.SpecialType == SpecialType.System_String) {
				return ReturnTypeKind.SystemString;
			} else if (comparer.Equals(respType, knownTypes.Stream)) {
				return ReturnTypeKind.Stream;
			} else if (comparer.Equals(respType, knownTypes.JsonNode)) {
				return ReturnTypeKind.JsonNode;
			} else if (comparer.Equals(respType, knownTypes.JsonDocument)) {
				return ReturnTypeKind.JsonDocument;
			} else if (comparer.Equals(respType, knownTypes.SseEvent)) {
				return ReturnTypeKind.SseEvent;
			} else if (isSseEventT) {
				if (respType is INamedTypeSymbol {
					IsGenericType: true,
					TypeArguments: [var sseDataType0, ..]
				}) {
					if (sseDataType0.SpecialType == SpecialType.System_String) {
						return ReturnTypeKind.SseEventString;
					} else if (comparer.Equals(sseDataType0, knownTypes.JsonNode)) {
						return ReturnTypeKind.SseEventJsonNode;
					} else if (comparer.Equals(sseDataType0, knownTypes.JsonDocument)) {
						return ReturnTypeKind.SseEventJsonDocument;
					} else {
						return ReturnTypeKind.SseEventT;
					}
				} else {
					throw new NotSupportedException("Unreachable");
				}
			} else if (isSseItemT) {
				if (respType is INamedTypeSymbol {
					IsGenericType: true,
					TypeArguments: [var sseDataType1, ..]
				}) {
					if (sseDataType1.SpecialType == SpecialType.System_String) {
						return ReturnTypeKind.SseItemString;
					} else if (comparer.Equals(sseDataType1, knownTypes.JsonNode)) {
						return ReturnTypeKind.SseItemJsonNode;
					} else if (comparer.Equals(sseDataType1, knownTypes.JsonDocument)) {
						return ReturnTypeKind.SseItemJsonDocument;
					} else {
						return ReturnTypeKind.SseItemT;
					}
				} else {
					throw new NotSupportedException("Unreachable");
				}
			} else if (isApiResponseT) {
				// return ReturnTypeKind.ApiResponseT;
				if (respType is INamedTypeSymbol {
					IsGenericType: true,
					TypeArguments: [var sseDataType2, ..]
				}) {
					if (sseDataType2.SpecialType == SpecialType.System_String) {
						return ReturnTypeKind.ApiResponseString;
					} else if (comparer.Equals(sseDataType2, knownTypes.JsonNode)) {
						return ReturnTypeKind.ApiResponseJsonNode;
					} else if (comparer.Equals(sseDataType2, knownTypes.JsonDocument)) {
						return ReturnTypeKind.ApiResponseJsonDocument;
					} else {
						return ReturnTypeKind.ApiResponseT;
					}
				} else {
					throw new NotSupportedException("Unreachable");
				}
			} else if (returnsRawResponse) {
				return ReturnTypeKind.RawResponse;
			} else {
				return ReturnTypeKind.Other;
			}
		} else {
			return ReturnTypeKind.Void;
		}
	}
}