using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public sealed record MethodInfo(
	// 泛型参数数量,非泛型为0
	int Arity,
	// 所属类型的类型名,非完全限定名
	string ContainingTypeName,
	// 所属类型的类型名,完全限定名
	string ContainingTypeFullyQualifiedName,
	// 可访问性修饰符
	Accessibility DeclaredAccessibility,
	bool IsAbstract,
	bool IsAsync,
	bool IsGenericMethod,
	bool IsIterator,
	bool IsOverride,
	bool IsPartialDefinition,
	bool IsReadOnly,
	bool IsSealed,
	bool IsStatic,
	bool IsVirtual,
	SymbolKind Kind,
	MethodKind MethodKind,
	// 方法名,不包含泛型参数
	string Name,
	// 参数列表
	EquatableArray<ParameterInfo> Parameters,
	// 返回类型的可空性
	NullableAnnotation ReturnNullableAnnotation,
	// 是否ref返回
	bool ReturnsByRef,
	// 是否ref readonly返回
	bool ReturnsByRefReadonly,
	// 是否返回void
	bool ReturnsVoid,
	// 返回类型
	TypeInfo ReturnType,
	// 泛型参数可空性,无法使用NullableAnnotation,需要强转byte
	EquatableArray<byte> TypeArgumentNullableAnnotations,
	// 泛型实参
	EquatableArray<TypeInfo> TypeArguments,
	// 泛型形参
	EquatableArray<TypeInfo> TypeParameters,
	LocationInfo? Location,
	EquatableArray<DiagnosticInfo> Diagnostics
) {
	public static MethodInfo CreateFromSymbol(IMethodSymbol symbol) {
		return new(
			Arity: symbol.Arity,
			ContainingTypeName: symbol.ContainingType.Name,
			ContainingTypeFullyQualifiedName: symbol.ContainingType.ToDisplayString(),
			DeclaredAccessibility: symbol.DeclaredAccessibility,
			IsAbstract: symbol.IsAbstract,
			IsAsync: symbol.IsAsync,
			IsGenericMethod: symbol.IsGenericMethod,
			IsIterator: symbol.IsIterator,
			IsOverride: symbol.IsOverride,
			IsPartialDefinition: symbol.IsPartialDefinition,
			IsReadOnly: symbol.IsReadOnly,
			IsSealed: symbol.IsSealed,
			IsStatic: symbol.IsStatic,
			IsVirtual: symbol.IsVirtual,
			Kind: symbol.Kind,
			MethodKind: symbol.MethodKind,
			Name: symbol.Name,
			// TODO
			Parameters: [],
			ReturnNullableAnnotation: symbol.ReturnNullableAnnotation,
			ReturnsByRef: symbol.ReturnsByRef,
			ReturnsByRefReadonly: symbol.ReturnsByRefReadonly,
			ReturnsVoid: symbol.ReturnsVoid,
			ReturnType: TypeInfo.CreateFromSymbol(symbol.ReturnType),
			// TODO
			TypeArgumentNullableAnnotations: [],
			// TODO
			TypeArguments: [],
			// TODO
			TypeParameters: [],
			Location: symbol.Locations[0] is null ? null : new(symbol.Locations[0]),
			// TODO
			Diagnostics: []
		);
	}
}
