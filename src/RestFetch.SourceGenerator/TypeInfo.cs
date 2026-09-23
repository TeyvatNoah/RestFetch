using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;


public sealed record TypeInfo(
	// 泛型参数数量,非泛型为0
	int Arity,
	// 父类,如果有的话
	TypeInfo? BaseType,
	// 所属namespace,如果有的话
	NamespaceInfo? ContainingNamespace,
	// 嵌套类的外部类,如果是嵌套类的话
	TypeInfo? ContainingType,
	// 可访问性修饰符
	Accessibility DeclaredAccessibility,
	// 枚举的底层类型,如果不是枚举则null
	TypeInfo? EnumUnderlyingType,
	// 实例构造函数
	EquatableArray<MethodInfo> InstanceConstructors,
	// 是否抽象类
	bool IsAbstract,
	// file可访问性
	bool IsFileLocal,
	// 是否泛型
	bool IsGenericType,
	// 是否readonly
	bool IsReadOnly,
	// 是否record
	bool IsRecord,
	// 是否引用类型
	bool IsReferenceType,
	// 是否sealed
	bool IsSealed,
	// 是否static
	bool IsStatic,
	// 是否tuple
	bool IsTupleType,
	// 是否开放泛型
	bool IsUnboundGenericType,
	SymbolKind Kind,
	// 元数据名称,对于非泛型就是类型名,泛型则是如Foo`1这样包含泛型数量的名称
	// 不包含namespace和外部类的名称
	string MetadataName,
	TypeKind TypeKind,
	// 类名,不包含namespace和外部类的名称,不包含泛型参数
	string Name,
	// 可空性
	NullableAnnotation NullableAnnotation,
	// 泛型实参
	EquatableArray<TypeInfo> TypeArguments,
	// 泛型形参
	EquatableArray<TypeInfo> TypeParameters,
	// 完全限定类名,不带global::
	string FullyQualifiedName,
	// 是interface还是partial class还是partial struct
	// ApiDefinitionKind Kind,
	// 需要补全生成的method
	EquatableArray<MethodInfo> Methods,
	bool IsPartial,
	bool IsInterface,
	LocationInfo? Location,
	EquatableArray<DiagnosticInfo> Diagnostics
) {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeInfo CreateFromSymbol(ITypeSymbol symbol)
		=> symbol is INamedTypeSymbol typeSymbol ? new(
			Arity: typeSymbol.Arity,
			BaseType: symbol.BaseType is null ? null : CreateFromSymbol(symbol.BaseType),
			ContainingNamespace: NamespaceInfo.CreateFromSymbol(symbol.ContainingNamespace),
			ContainingType: symbol.ContainingType is null ? null : CreateFromSymbol(symbol.ContainingType),
			DeclaredAccessibility: symbol.DeclaredAccessibility,
			EnumUnderlyingType: typeSymbol.EnumUnderlyingType is null ? null :  CreateFromSymbol(typeSymbol.EnumUnderlyingType),
			// TODO
			InstanceConstructors: [],
			IsAbstract: symbol.IsAbstract,
			IsFileLocal: typeSymbol.IsFileLocal,
			IsGenericType: typeSymbol.IsGenericType,
			IsReadOnly: symbol.IsReadOnly,
			IsRecord: symbol.IsRecord,
			IsReferenceType: symbol.IsReferenceType,
			IsSealed: symbol.IsSealed,
			IsStatic: symbol.IsStatic,
			IsTupleType: symbol.IsTupleType,
			IsUnboundGenericType: typeSymbol.IsUnboundGenericType,
			Kind: symbol.Kind,
			MetadataName: symbol.MetadataName,
			TypeKind: symbol.TypeKind,
			Name: symbol.Name,
			NullableAnnotation: symbol.NullableAnnotation,
			// TODO
			TypeArguments: [],
			// TODO
			TypeParameters: [],
			FullyQualifiedName: symbol.ToDisplayString(),
			// TODO
			Methods: [],
			IsPartial: typeSymbol.IsPartial(),
			IsInterface: symbol.TypeKind == TypeKind.Interface,
			Location: symbol.Locations[0] is null ? null : new(symbol.Locations[0]),
			// TODO
			Diagnostics: []
		) : new(
			Arity: 0,
			BaseType: symbol.BaseType is null ? null : CreateFromSymbol(symbol.BaseType),
			ContainingNamespace: NamespaceInfo.CreateFromSymbol(symbol.ContainingNamespace),
			ContainingType: symbol.ContainingType is null ? null : CreateFromSymbol(symbol.ContainingType),
			DeclaredAccessibility: symbol.DeclaredAccessibility,
			EnumUnderlyingType: null,
			// TODO
			InstanceConstructors: [],
			IsAbstract: symbol.IsAbstract,
			IsFileLocal: false,
			IsGenericType: false,
			IsReadOnly: symbol.IsReadOnly,
			IsRecord: symbol.IsRecord,
			IsReferenceType: symbol.IsReferenceType,
			IsSealed: symbol.IsSealed,
			IsStatic: symbol.IsStatic,
			IsTupleType: symbol.IsTupleType,
			IsUnboundGenericType: false,
			Kind: symbol.Kind,
			MetadataName: symbol.MetadataName,
			TypeKind: symbol.TypeKind,
			Name: symbol.Name,
			NullableAnnotation: symbol.NullableAnnotation,
			// TODO
			TypeArguments: [],
			// TODO
			TypeParameters: [],
			FullyQualifiedName: symbol.ToDisplayString(),
			// TODO
			Methods: [],
			IsPartial: false,
			IsInterface: symbol.TypeKind == TypeKind.Interface,
			Location: symbol.Locations[0] is null ? null : new(symbol.Locations[0]),
			// TODO
			Diagnostics: []
		);
}
