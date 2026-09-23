using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public record AttributeInfo (
	string Name,
	// 完全限定类名,不带global::
	string FullyQualifiedName,
	bool IsGenericType,
	// 泛型参数,如果没有的话为[]
	EquatableArray<TypeInfo> GenericTypeInfos,
	// 命名参数列表
	EquatableArray<(string, TypedConstant)> NamedArgs,
	LocationInfo? Location,
	EquatableArray<DiagnosticInfo> Diagnostics
);

public sealed record AttributeInfo<TTuple> (
	string Name,
	// 完全限定类名,不带global::
	string FullyQualifiedName,
	bool IsGenericType,
	// 泛型参数,如果没有的话为[]
	EquatableArray<TypeInfo> GenericTypeInfos,
	// 构造函数参数列表
	TTuple ConstructorArgs,
	// 命名参数列表
	EquatableArray<(string, TypedConstant)> NamedArgs,
	LocationInfo? Location,
	EquatableArray<DiagnosticInfo> Diagnostics
): AttributeInfo(
	Name,
	FullyQualifiedName,
	IsGenericType,
	GenericTypeInfos,
	NamedArgs,
	Location,
	Diagnostics
) where TTuple: IEquatable<TTuple>;