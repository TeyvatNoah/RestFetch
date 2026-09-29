using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;


namespace RestFetch.SourceGenerator;

public sealed record ParameterInfo(
	string Name,
	// 参数默认值字符串形式
	string ExplicitDefaultValue,
	// 是否有默认值
	bool HasExplicitDefaultValue,
	// 是否弃元
	bool IsDiscard,
	// 是否可选
	bool IsOptional,
	// 是否param关键字
	bool IsParams,
	// 是否param参数
	bool IsParamsArray,
	// 可空性
	NullableAnnotation NullableAnnotation,
	// 参数位置序号,从0开始
	int Ordinal,
	// ref或in或ref readonly等
	RefKind RefKind,
	// scoped标记
	ScopedKind ScopedKind,
	// 参数类型
	TypeInfo Type,
	LocationInfo? Location,
	EquatableArray<DiagnosticInfo>? Diagnostics
) {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ParameterInfo CreateFromSymbol(IParameterSymbol symbol, KnownTypes knownTypes)
		=> new(
			Name: symbol.Name,
			ExplicitDefaultValue: symbol.HasExplicitDefaultValue ? symbol.ExplicitDefaultValue?.ToString() ?? "null" : "null",
			HasExplicitDefaultValue: symbol.HasExplicitDefaultValue,
			IsDiscard: symbol.IsDiscard,
			IsOptional: symbol.IsOptional,
			IsParams: symbol.IsParams,
			IsParamsArray: symbol.IsParamsArray,
			NullableAnnotation: symbol.NullableAnnotation,
			Ordinal: symbol.Ordinal,
			RefKind: symbol.RefKind,
			ScopedKind: symbol.ScopedKind,
			Type: TypeInfo.CreateFromSymbolWithoutMethods(symbol.Type, knownTypes),
			Location: new(symbol.Locations.FirstOrDefault() ?? Microsoft.CodeAnalysis.Location.None),
			// TODO
			Diagnostics: null
		);
}
