using System.Collections.Immutable;
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
	// 是否数组
	bool IsArray,
	// 数组元素是否可空
	bool IsElementNullable,
	// 是否泛型形参
	bool IsTypeParameter,
	// 是否抽象类
	bool IsAbstract,
	// file可访问性
	bool IsFileLocal,
	// 是否泛型
	bool IsGenericType,
	// 是否readonly
	bool IsReadOnly,
	// 是否ref
	bool IsRefLikeType,
	// 是否record
	bool IsRecord,
	// 是否引用类型
	bool IsReferenceType,
	// 是否值类型
	bool IsValueType,
	// 是否可空,更可靠
	bool IsNullable,
	// 是否sealed
	bool IsSealed,
	// 是否static
	bool IsStatic,
	// 是否tuple
	bool IsTupleType,
	// 是否开放泛型
	bool IsUnboundGenericType,
	// 是否void
	bool IsVoid,
	SymbolKind Kind,
	// 元数据名称,对于非泛型就是类型名,泛型则是如Foo`1这样包含泛型数量的名称
	// 不包含namespace和外部类的名称
	string MetadataName,
	TypeKind TypeKind,
	SpecialType SpecialType,
	// 类名,不包含namespace和外部类的名称,不包含泛型参数
	string Name,
	// 可空性,值类型不能依靠它
	NullableAnnotation NullableAnnotation,
	// 泛型实参
	EquatableArray<TypeInfo> TypeArguments,
	// 泛型形参
	EquatableArray<TypeInfo> TypeParameters,
	// 完全限定类名,不带global::
	string FullyQualifiedName,
	// 完全限定类名,带global::
	string FullyQualifiedNameWithGlobal,
	// 完全限定类名,带global::和泛型约束
	string FullyQualifiedNameWithGlobalAndWhereClauses,
	// 类名带泛型,不带命名空间
	string MinimallyQualifiedName,
	// where泛型约束
	string? WhereClauses,
	// 是interface还是partial class还是partial struct
	// ApiDefinitionKind Kind,
	// 需要补全生成的method
	EquatableArray<MethodInfo> Methods,
	bool IsPartial,
	bool IsInterface,
	LocationInfo? Location,
	EquatableArray<DiagnosticInfo>? Diagnostics
) {
	public static readonly SymbolDisplayFormat FullyQualifiedFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions:
            SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
            SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
    );
	public static readonly SymbolDisplayFormat FullyQualifiedWithWhereClausesFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeConstraints | SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions:
            SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
            SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
    );

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeInfo CreateFromSymbol(ITypeSymbol symbol, KnownTypes knownTypes, int depth = 1) {
		var newDepth = depth - 1;
		if (symbol is INamedTypeSymbol typeSymbol) {
			return new(
				IsElementNullable: false,
				IsArray: false,
				IsTypeParameter: false,
				WhereClauses: typeSymbol.GetWhereClauses(),
				Arity: typeSymbol.Arity,
				BaseType: typeSymbol.BaseType is null
				|| typeSymbol.Kind == SymbolKind.TypeParameter
				|| typeSymbol.BaseType.SpecialType == SpecialType.System_Object
				|| typeSymbol.BaseType.SpecialType == SpecialType.System_ValueType ? null : CreateFromSymbol(typeSymbol.BaseType, knownTypes, newDepth),
				ContainingNamespace: NamespaceInfo.CreateFromSymbol(typeSymbol.ContainingNamespace),
				ContainingType: typeSymbol.ContainingType is null || typeSymbol.Kind == SymbolKind.TypeParameter ? null : CreateFromSymbolWithoutBaseTypeAndMethods(typeSymbol.ContainingType, knownTypes),
				DeclaredAccessibility: typeSymbol.DeclaredAccessibility,
				EnumUnderlyingType: typeSymbol.EnumUnderlyingType is null ? null :  CreateFromSymbolWithoutBaseTypeAndMethods(typeSymbol.EnumUnderlyingType, knownTypes),
				InstanceConstructors: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsVoid: typeSymbol.SpecialType == SpecialType.System_Void,
				IsAbstract: typeSymbol.IsAbstract,
				IsFileLocal: typeSymbol.IsFileLocal,
				IsGenericType: typeSymbol.IsGenericType,
				IsNullable: typeSymbol.IsNullable(),
				IsReadOnly: typeSymbol.IsReadOnly,
				IsRefLikeType: typeSymbol.IsRefLikeType,
				IsRecord: typeSymbol.IsRecord,
				IsReferenceType: typeSymbol.IsReferenceType,
				IsValueType: typeSymbol.IsValueType,
				IsSealed: typeSymbol.IsSealed,
				IsStatic: typeSymbol.IsStatic,
				IsTupleType: typeSymbol.IsTupleType,
				IsUnboundGenericType: typeSymbol.IsUnboundGenericType,
				Kind: typeSymbol.Kind,
				MetadataName: typeSymbol.MetadataName,
				TypeKind: typeSymbol.TypeKind,
				SpecialType: typeSymbol.SpecialType,
				Name: typeSymbol.Name,
				NullableAnnotation: typeSymbol.NullableAnnotation,
				TypeArguments: depth > 0 ? typeSymbol.TypeArguments.Select(t => CreateFromSymbolWithoutMethods(t, knownTypes)).ToImmutableArray() : Array.Empty<TypeInfo>().ToImmutableArray(),
				TypeParameters: depth > 0 ? typeSymbol.TypeParameters.Select(t => CreateFromSymbolWithoutBaseTypeAndMethods(t, knownTypes)).ToImmutableArray() : Array.Empty<TypeInfo>().ToImmutableArray(),
				FullyQualifiedName: typeSymbol.ToDisplayString(),
				FullyQualifiedNameWithGlobal: typeSymbol.ToDisplayString(FullyQualifiedFormat),
				FullyQualifiedNameWithGlobalAndWhereClauses: typeSymbol.ToDisplayString(FullyQualifiedWithWhereClausesFormat),
				MinimallyQualifiedName: typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
				Methods: depth > 0 ? typeSymbol.GetInstanceMethods().Select(m => MethodInfo.CreateFromSymbol(m, knownTypes)).ToImmutableArray() : Array.Empty<MethodInfo>().ToImmutableArray(),
				IsPartial: typeSymbol.IsPartial(),
				IsInterface: typeSymbol.TypeKind == TypeKind.Interface,
				Location: typeSymbol.Locations.IsEmpty ? null : new(typeSymbol.Locations[0]),
				// TODO
				Diagnostics: null
			);
		} else {
			return CreateFromSymbolWithoutMethods(symbol, knownTypes);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeInfo CreateFromSymbolWithoutMethods(ITypeSymbol symbol, KnownTypes knownTypes) {
		if (symbol is INamedTypeSymbol typeSymbol) {
			return new(
				IsElementNullable: false,
				IsArray: false,
				IsTypeParameter: false,
				WhereClauses: typeSymbol.GetWhereClauses(),
				Arity: typeSymbol.Arity,
				BaseType: typeSymbol.BaseType is null
				|| typeSymbol.Kind == SymbolKind.TypeParameter
				|| typeSymbol.BaseType.SpecialType == SpecialType.System_Object
				|| typeSymbol.BaseType.SpecialType == SpecialType.System_ValueType ? null : CreateFromSymbolWithoutMethods(typeSymbol.BaseType, knownTypes),
				ContainingNamespace: NamespaceInfo.CreateFromSymbol(typeSymbol.ContainingNamespace),
				ContainingType: typeSymbol.ContainingType is null || typeSymbol.Kind == SymbolKind.TypeParameter ? null : CreateFromSymbolWithoutBaseTypeAndMethods(typeSymbol.ContainingType, knownTypes),
				DeclaredAccessibility: typeSymbol.DeclaredAccessibility,
				EnumUnderlyingType: typeSymbol.EnumUnderlyingType is null ? null :  CreateFromSymbolWithoutBaseTypeAndMethods(typeSymbol.EnumUnderlyingType, knownTypes),
				InstanceConstructors: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsVoid: typeSymbol.SpecialType == SpecialType.System_Void,
				IsAbstract: typeSymbol.IsAbstract,
				IsFileLocal: typeSymbol.IsFileLocal,
				IsGenericType: typeSymbol.IsGenericType,
				IsNullable: typeSymbol.IsNullable(),
				IsReadOnly: typeSymbol.IsReadOnly,
				IsRefLikeType: typeSymbol.IsRefLikeType,
				IsRecord: typeSymbol.IsRecord,
				IsReferenceType: typeSymbol.IsReferenceType,
				IsValueType: typeSymbol.IsValueType,
				IsSealed: typeSymbol.IsSealed,
				IsStatic: typeSymbol.IsStatic,
				IsTupleType: typeSymbol.IsTupleType,
				IsUnboundGenericType: typeSymbol.IsUnboundGenericType,
				Kind: typeSymbol.Kind,
				MetadataName: typeSymbol.MetadataName,
				TypeKind: typeSymbol.TypeKind,
				SpecialType: typeSymbol.SpecialType,
				Name: typeSymbol.Name,
				NullableAnnotation: typeSymbol.NullableAnnotation,
				TypeArguments: typeSymbol.TypeArguments.Select(t => CreateFromSymbolWithoutMethods(t, knownTypes)).ToImmutableArray(),
				TypeParameters: typeSymbol.TypeParameters.Select(t => CreateFromSymbolWithoutBaseTypeAndMethods(t, knownTypes)).ToImmutableArray(),
				FullyQualifiedName: typeSymbol.ToDisplayString(),
				FullyQualifiedNameWithGlobal: typeSymbol.ToDisplayString(FullyQualifiedFormat),
				FullyQualifiedNameWithGlobalAndWhereClauses: typeSymbol.ToDisplayString(FullyQualifiedWithWhereClausesFormat),
				MinimallyQualifiedName: typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
				Methods: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsPartial: typeSymbol.IsPartial(),
				IsInterface: typeSymbol.TypeKind == TypeKind.Interface,
				Location: typeSymbol.Locations.IsEmpty ? null : new(typeSymbol.Locations[0]),
				// TODO
				Diagnostics: null
			);
		} else {
			return CreateFromSymbolWithoutBaseTypeAndMethods(symbol, knownTypes);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeInfo CreateFromSymbolWithoutBaseTypeAndMethods(ITypeSymbol symbol, KnownTypes knownTypes) {
		if (symbol is INamedTypeSymbol typeSymbol) {
			return new(
				IsElementNullable: false,
				IsArray: false,
				IsTypeParameter: false,
				WhereClauses: typeSymbol.GetWhereClauses(),
				Arity: typeSymbol.Arity,
				BaseType: null,
				ContainingNamespace: NamespaceInfo.CreateFromSymbol(typeSymbol.ContainingNamespace),
				ContainingType: typeSymbol.ContainingType is null || typeSymbol.Kind == SymbolKind.TypeParameter ? null : CreateFromSymbolWithoutBaseTypeAndMethods(typeSymbol.ContainingType, knownTypes),
				DeclaredAccessibility: typeSymbol.DeclaredAccessibility,
				EnumUnderlyingType: typeSymbol.EnumUnderlyingType is null ? null :  CreateFromSymbolWithoutBaseTypeAndMethods(typeSymbol.EnumUnderlyingType, knownTypes),
				InstanceConstructors: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsVoid: typeSymbol.SpecialType == SpecialType.System_Void,
				IsAbstract: typeSymbol.IsAbstract,
				IsFileLocal: typeSymbol.IsFileLocal,
				IsGenericType: typeSymbol.IsGenericType,
				IsNullable: typeSymbol.IsNullable(),
				IsReadOnly: typeSymbol.IsReadOnly,
				IsRefLikeType: typeSymbol.IsRefLikeType,
				IsRecord: typeSymbol.IsRecord,
				IsReferenceType: typeSymbol.IsReferenceType,
				IsValueType: typeSymbol.IsValueType,
				IsSealed: typeSymbol.IsSealed,
				IsStatic: typeSymbol.IsStatic,
				IsTupleType: typeSymbol.IsTupleType,
				IsUnboundGenericType: typeSymbol.IsUnboundGenericType,
				Kind: typeSymbol.Kind,
				MetadataName: typeSymbol.MetadataName,
				TypeKind: typeSymbol.TypeKind,
				SpecialType: typeSymbol.SpecialType,
				Name: typeSymbol.Name,
				NullableAnnotation: typeSymbol.NullableAnnotation,
				TypeArguments: typeSymbol.TypeArguments.Select(t => CreateFromSymbolWithoutMethods(t, knownTypes)).ToImmutableArray(),
				TypeParameters: typeSymbol.TypeParameters.Select(t => CreateFromSymbolWithoutBaseTypeAndMethods(t, knownTypes)).ToImmutableArray(),
				FullyQualifiedName: typeSymbol.ToDisplayString(),
				FullyQualifiedNameWithGlobal: typeSymbol.ToDisplayString(FullyQualifiedFormat),
				FullyQualifiedNameWithGlobalAndWhereClauses: typeSymbol.ToDisplayString(FullyQualifiedWithWhereClausesFormat),
				MinimallyQualifiedName: typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
				Methods: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsPartial: typeSymbol.IsPartial(),
				IsInterface: typeSymbol.TypeKind == TypeKind.Interface,
				Location: typeSymbol.Locations.IsEmpty ? null : new(typeSymbol.Locations[0]),
				// TODO
				Diagnostics: null
			);
		} else if (symbol is IArrayTypeSymbol arraySymbol) {
			return new(
				IsElementNullable: arraySymbol.ElementNullableAnnotation == NullableAnnotation.Annotated,
				IsArray: true,
				IsTypeParameter: false,
				WhereClauses: null,
				Arity: 0,
				BaseType: null,
				ContainingNamespace: NamespaceInfo.CreateFromSymbol(arraySymbol.ContainingNamespace),
				ContainingType: arraySymbol.ContainingType is null || arraySymbol.Kind == SymbolKind.TypeParameter ? null : CreateFromSymbolWithoutBaseTypeAndMethods(arraySymbol.ContainingType, knownTypes),
				DeclaredAccessibility: arraySymbol.DeclaredAccessibility,
				EnumUnderlyingType: null,
				InstanceConstructors: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsVoid: arraySymbol.SpecialType == SpecialType.System_Void,
				IsAbstract: arraySymbol.IsAbstract,
				IsFileLocal: false,
				IsGenericType: false,
				IsNullable: arraySymbol.IsNullable(),
				IsReadOnly: arraySymbol.IsReadOnly,
				IsRefLikeType: arraySymbol.IsRefLikeType,
				IsRecord: arraySymbol.IsRecord,
				IsReferenceType: arraySymbol.IsReferenceType,
				IsValueType: arraySymbol.IsValueType,
				IsSealed: arraySymbol.IsSealed,
				IsStatic: arraySymbol.IsStatic,
				IsTupleType: arraySymbol.IsTupleType,
				IsUnboundGenericType: false,
				Kind: arraySymbol.Kind,
				MetadataName: arraySymbol.MetadataName,
				TypeKind: arraySymbol.TypeKind,
				SpecialType: arraySymbol.SpecialType,
				Name: arraySymbol.Name,
				NullableAnnotation: arraySymbol.NullableAnnotation,
				TypeArguments: Array.Empty<TypeInfo>().ToImmutableArray(),
				TypeParameters: Array.Empty<TypeInfo>().ToImmutableArray(),
				FullyQualifiedName: arraySymbol.ToDisplayString(),
				FullyQualifiedNameWithGlobal: arraySymbol.ToDisplayString(FullyQualifiedFormat),
				FullyQualifiedNameWithGlobalAndWhereClauses: arraySymbol.ToDisplayString(FullyQualifiedFormat),
				MinimallyQualifiedName: arraySymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
				Methods: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsPartial: false,
				IsInterface: arraySymbol.TypeKind == TypeKind.Interface,
				Location: arraySymbol.Locations.IsEmpty ? null : new(arraySymbol.Locations[0]),
				// TODO
				Diagnostics: null
			);
		} else if (symbol is ITypeParameterSymbol typeParamSymbol) {
			return new(
				IsElementNullable: false,
				IsArray: false,
				IsTypeParameter: true,
				WhereClauses: null,
				Arity: 0,
				BaseType: null,
				ContainingNamespace: NamespaceInfo.CreateFromSymbol(typeParamSymbol.ContainingNamespace),
				ContainingType: typeParamSymbol.ContainingType is null || typeParamSymbol.Kind == SymbolKind.TypeParameter ? null : CreateFromSymbolWithoutBaseTypeAndMethods(typeParamSymbol.ContainingType, knownTypes),
				DeclaredAccessibility: typeParamSymbol.DeclaredAccessibility,
				EnumUnderlyingType: null,
				InstanceConstructors: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsVoid: typeParamSymbol.SpecialType == SpecialType.System_Void,
				IsAbstract: typeParamSymbol.IsAbstract,
				IsFileLocal: false,
				IsGenericType: false,
				IsNullable: typeParamSymbol.IsNullable(),
				IsReadOnly: typeParamSymbol.IsReadOnly,
				IsRefLikeType: typeParamSymbol.IsRefLikeType,
				IsRecord: typeParamSymbol.IsRecord,
				IsReferenceType: typeParamSymbol.IsReferenceType,
				IsValueType: typeParamSymbol.IsValueType,
				IsSealed: typeParamSymbol.IsSealed,
				IsStatic: typeParamSymbol.IsStatic,
				IsTupleType: typeParamSymbol.IsTupleType,
				IsUnboundGenericType: false,
				Kind: typeParamSymbol.Kind,
				MetadataName: typeParamSymbol.MetadataName,
				TypeKind: typeParamSymbol.TypeKind,
				SpecialType: typeParamSymbol.SpecialType,
				Name: typeParamSymbol.Name,
				NullableAnnotation: typeParamSymbol.NullableAnnotation,
				TypeArguments: Array.Empty<TypeInfo>().ToImmutableArray(),
				TypeParameters: Array.Empty<TypeInfo>().ToImmutableArray(),
				FullyQualifiedName: typeParamSymbol.ToDisplayString(),
				FullyQualifiedNameWithGlobal: typeParamSymbol.ToDisplayString(FullyQualifiedFormat),
				FullyQualifiedNameWithGlobalAndWhereClauses: typeParamSymbol.ToDisplayString(FullyQualifiedFormat),
				MinimallyQualifiedName: typeParamSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
				Methods: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsPartial: false,
				IsInterface: typeParamSymbol.TypeKind == TypeKind.Interface,
				Location: typeParamSymbol.Locations.IsEmpty ? null : new(typeParamSymbol.Locations[0]),
				// TODO
				Diagnostics: null
			);
		} else {
			return new(
				IsElementNullable: false,
				IsArray: false,
				IsTypeParameter: false,
				WhereClauses: null,
				Arity: 0,
				BaseType: null,
				ContainingNamespace: NamespaceInfo.CreateFromSymbol(symbol.ContainingNamespace),
				ContainingType: symbol.ContainingType is null || symbol.Kind == SymbolKind.TypeParameter ? null : CreateFromSymbolWithoutBaseTypeAndMethods(symbol.ContainingType, knownTypes),
				DeclaredAccessibility: symbol.DeclaredAccessibility,
				EnumUnderlyingType: null,
				InstanceConstructors: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsVoid: symbol.SpecialType == SpecialType.System_Void,
				IsAbstract: symbol.IsAbstract,
				IsFileLocal: false,
				IsGenericType: false,
				IsNullable: symbol.IsNullable(),
				IsReadOnly: symbol.IsReadOnly,
				IsRefLikeType: symbol.IsRefLikeType,
				IsRecord: symbol.IsRecord,
				IsReferenceType: symbol.IsReferenceType,
				IsValueType: symbol.IsValueType,
				IsSealed: symbol.IsSealed,
				IsStatic: symbol.IsStatic,
				IsTupleType: symbol.IsTupleType,
				IsUnboundGenericType: false,
				Kind: symbol.Kind,
				MetadataName: symbol.MetadataName,
				TypeKind: symbol.TypeKind,
				SpecialType: symbol.SpecialType,
				Name: symbol.Name,
				NullableAnnotation: symbol.NullableAnnotation,
				TypeArguments: Array.Empty<TypeInfo>().ToImmutableArray(),
				TypeParameters: Array.Empty<TypeInfo>().ToImmutableArray(),
				FullyQualifiedName: symbol.ToDisplayString(),
				FullyQualifiedNameWithGlobal: symbol.ToDisplayString(FullyQualifiedFormat),
				FullyQualifiedNameWithGlobalAndWhereClauses: symbol.ToDisplayString(FullyQualifiedFormat),
				MinimallyQualifiedName: symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
				Methods: Array.Empty<MethodInfo>().ToImmutableArray(),
				IsPartial: false,
				IsInterface: symbol.TypeKind == TypeKind.Interface,
				Location: symbol.Locations.IsEmpty ? null : new(symbol.Locations[0]),
				// TODO
				Diagnostics: null
			);
		}
	}
}
