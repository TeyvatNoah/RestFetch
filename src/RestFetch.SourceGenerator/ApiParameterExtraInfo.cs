using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public record ApiParameterExtraInfo(
	string Name,
	ApiParameterKind Kind,
	bool IsValid,
	EquatableArray<DiagnosticInfo>? Diagnostics
) {
	public static readonly DiagnosticDescriptor ParameterRule0 = new("RestFetch0050",
		"多个HTTP Parameter Attribute",
		@"[Body],[Body<TSerializer>],[Param],[param<TSerializer>],[Metadata],[Query],[Query<TSerializer>],[Header]中只能同时存在最多一个",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule1 = new("RestFetch0051",
		"未知的参数类型",
		@"参数必须被[Body],[Body<TSerializer>],[Param],[param<TSerializer>],[Metadata],[Query],[Query<TSerializer>],[Header]中的一个修饰,或为Cancellation",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule2 = new("RestFetch0052",
		"错误的请求body序列化器",
		@"{0}必须实现IRequestBodySerializer或IRequestBodySerializer<TRequestBodyContent, TResponse, TProxy>或IRequestBodySerializer<TRequestBody, TRequestBodyContent, TResponse, TProxy>",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule3 = new("RestFetch0053",
		"错误的请求param序列化器",
		@"{0}必须实现IParamEncodeSerializer<in TValue, out TReturn>",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule4 = new("RestFetch0054",
		"错误的请求param序列化器",
		@"{0}必须实现IParamSerializer<in TValue, out TReturn>或IParamEncodeSerializer<in TValue, out TReturn>",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule5 = new("RestFetch0055",
		"错误的Metadata参数",
		@"动态Metadata参数只能使用[Metadata]或[Metadata(""key"")]",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule6 = new("RestFetch0056",
		"错误的请求param参数",
		@"[Param]/[Param<T>]必须修饰非nullable参数",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule7 = new("RestFetch0057",
		"错误的请求querystring序列化器",
		@"{0}必须实现IAliasQueryStringSerializer<T>",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule8 = new("RestFetch0058",
		"错误的请求querystring序列化器",
		@"{0}必须实现IQueryStringSerializer或IQueryStringSerializer<T>或IAliasQueryStringSerializer<T>",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule9 = new("RestFetch0059",
		"错误的请求header参数",
		@"动态header必须使用[Header]或[Header(""name"")]而不是[Header(""name"", ""value"")]",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule10 = new("RestFetch0060",
		"错误的请求header参数",
		@"[Header(""name"")]只能修饰string类型的参数",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule11 = new("RestFetch0061",
		"错误的请求header参数",
		@"[Header]只能修饰IEnumerable<KeyValuePair<string, string>>类型的参数",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule12 = new("RestFetch0062",
		"未知错误",
		"不存在的参数类型",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule13 = new("RestFetch0063",
		"错误的body参数",
		"不允许同时存在多个[Body]/[Body<T>]修饰的参数",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule14 = new("RestFetch0064",
		"错误的QueryString参数类型",
		"默认QueryString序列化器仅支持QueryString,FastQueryString,以及基本类型及其IEnumerable<primitive>或IEnumerable<KeyValuePair<string, primitive>>",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule15 = new("RestFetch0065",
		"Cancellation参数不能为null",
		"Cancellation参数不能为null",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	public static readonly DiagnosticDescriptor ParameterRule16 = new("RestFetch0066",
		"错误的JsonSerializerContext类型",
		"{0}必须继承System.Text.Json.Serialization.JsonSerializerContext",
		"SourceGenerator", DiagnosticSeverity.Error, true);
	// public static readonly Dictionary<string, ApiParameterKind> ParamAttrsMap = new() {
	// 	{ "None", ApiParameterKind.None },
	// 	{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.BodyAttribute", ApiParameterKind.Body },
	// 	{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.BodyAttribute`1", ApiParameterKind.Body },
	// 	{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.ParamAttribute", ApiParameterKind.Param },
	// 	{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.ParamAttribute`1", ApiParameterKind.Param },
	// 	{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.MetadataAttribute", ApiParameterKind.Metadata },
	// 	{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.QueryAttribute", ApiParameterKind.QueryString },
	// 	{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.QueryAttribute`1", ApiParameterKind.QueryString },
	// 	{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.HeaderAttribute", ApiParameterKind.Header },
	// };
	public static ApiParameterExtraInfo CreateFromSymbol(IParameterSymbol symbol, KnownTypes knownTypes) {
		INamedTypeSymbol[] paramAttrSymbols = [
			knownTypes.BodyAttribute!,
			knownTypes.BodyAttributeT!,
			knownTypes.ParamAttribute!,
			knownTypes.ParamAttributeT!,
			knownTypes.MetadataAttribute!,
			knownTypes.QueryAttribute!,
			knownTypes.QueryAttributeT!,
			knownTypes.HeaderAttribute!,
		];
		// Dictionary<INamespaceOrTypeSymbol, ApiParameterKind> ParamAttrsMap = new(SymbolEqualityComparer.Default) {
		// 	{ knownTypes.BodyAttribute!, ApiParameterKind.Body },
		// 	{ knownTypes.BodyAttributeT!, ApiParameterKind.Body },
		// 	{ knownTypes.ParamAttribute!, ApiParameterKind.Param },
		// 	{ knownTypes.ParamAttributeT!, ApiParameterKind.Param },
		// 	{ knownTypes.MetadataAttribute!, ApiParameterKind.Metadata },
		// 	{ knownTypes.QueryAttribute!, ApiParameterKind.QueryString },
		// 	{ knownTypes.QueryAttributeT!, ApiParameterKind.QueryString },
		// 	{ knownTypes.HeaderAttribute!, ApiParameterKind.Header },
		// };
		var attrs = symbol.GetAttributes();
		var paramAttrs = paramAttrSymbols.SelectMany(key => attrs.GetAttributesBySymbol(key)).ToArray();
		if (paramAttrs.Length > 1) {
			var errors = paramAttrs
				.Select(attr => attr.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? symbol.Locations.FirstOrDefault() ?? Location.None)
				.Select(static location => new DiagnosticInfo(ParameterRule0, new(location), null))
				.ToArray();
			return new(
				Name: symbol.Name,
				Kind: ApiParameterKind.None,
				IsValid: false,
				Diagnostics: errors.ToImmutableArray()
			);
		} else if (paramAttrs.Length == 0) {
			var cancellationTokenType = knownTypes.CancellationToken;
			var originalType = symbol.Type.GetTFromNullable();
			if (originalType.InheritsFrom(cancellationTokenType)) {
				if (symbol.Type.IsNullable()) {
					return new ApiParameterExtraInfo(
						Name: symbol.Name,
						Kind: ApiParameterKind.None,
						IsValid: false,
						Diagnostics: (ImmutableArray<DiagnosticInfo>)[new DiagnosticInfo(ParameterRule15, new(symbol.Locations.FirstOrDefault() ?? Location.None), null)]
					);
				} else {
					return new CancellationTokenParameter(
						Name: symbol.Name,
						IsValid: true,
						Diagnostics: null
					);
				}
			} else {
				return new ApiParameterExtraInfo(
					Name: symbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new DiagnosticInfo(ParameterRule1, new(symbol.Locations.FirstOrDefault() ?? Location.None), null)]
				);
			}
		} else {
			return CreateInstanceInternal(paramAttrs[0], symbol, knownTypes);
		}
	}
	private static ApiParameterExtraInfo CreateInstanceInternal(AttributeData attr, IParameterSymbol parameterSymbol, KnownTypes knownTypes) {
		var attrTypeName = attr.AttributeClass!.MetadataName;
		if (attrTypeName == "BodyAttribute") {
			var configureAwait = false;
			string? jsonContextFullyQualifiedNameWithGlobal = null;
			foreach (var item in attr.NamedArguments) {
				// var (key, value) = item;
				if (item.Key == "ConfigureAwait") {
					configureAwait = (bool)item.Value.Value!;
				} else if (item.Key == "JsonContext") {
					if (item.Value.Value is INamedTypeSymbol jsonContextType && jsonContextType.InheritsFrom(knownTypes.JsonSerializerContext)) {
						jsonContextFullyQualifiedNameWithGlobal = jsonContextType.ToDisplayString(TypeInfo.FullyQualifiedFormat);
					} else {
						return new ApiParameterExtraInfo(
							Name: parameterSymbol.Name,
							Kind: ApiParameterKind.None,
							IsValid: false,
							Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule16, new(attr.GetNamedArgumentLocation("JsonContext")),
								(ImmutableArray<string>)[item.Value.Value is ITypeSymbol contextType ? contextType.Name : "JsonContext"])]
						);
					}
				}
			}
			// TODO 检查参数类型是否是默认序列化器支持的
			return new BodyParameter(
				Name: parameterSymbol.Name,
				IsValid: true,
				ConfigureAwait: configureAwait,
				JsonContext: jsonContextFullyQualifiedNameWithGlobal,
				Serializer: new(
					ConfigureAwait: configureAwait,
					FullyQualifiedName: $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.RequestBodySerializer.Json",
					FullyQualifiedNameWithGlobal: $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.RequestBodySerializer.Json",
				TypeArguments: null,
					Diagnostics: null
				),
				Diagnostics: null
			);
		} else if (attrTypeName == "BodyAttribute`1") {
			var iRequestBodySerializerSymbol = knownTypes.RequestBodySerializer;
			var iRequestBodySerializerGenericSymbol1 = knownTypes.RequestBodySerializer5;
			var iRequestBodySerializerGenericSymbol2 = knownTypes.RequestBodySerializer6;
			var serializerTypeSymbol = attr.AttributeClass.TypeArguments[0];
			if (serializerTypeSymbol.ImplementsInterface(iRequestBodySerializerSymbol)
			|| serializerTypeSymbol.ImplementsInterface(iRequestBodySerializerGenericSymbol1)
			|| serializerTypeSymbol.ImplementsInterface(iRequestBodySerializerGenericSymbol2)
			) {
				var configureAwait = false;
				string? jsonContextFullyQualifiedNameWithGlobal = null;
				foreach (var item in attr.NamedArguments) {
					// var (key, value) = item;
					if (item.Key == "ConfigureAwait") {
						configureAwait = (bool)item.Value.Value!;
					} else if (item.Key == "JsonContext") {
						if (item.Value.Value is INamedTypeSymbol jsonContextType && jsonContextType.InheritsFrom(knownTypes.JsonSerializerContext)) {
							jsonContextFullyQualifiedNameWithGlobal = jsonContextType.ToDisplayString(TypeInfo.FullyQualifiedFormat);
						} else {
							return new ApiParameterExtraInfo(
								Name: parameterSymbol.Name,
								Kind: ApiParameterKind.None,
								IsValid: false,
								Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule16, new(attr.GetNamedArgumentLocation("JsonContext")),
									(ImmutableArray<string>)[item.Value.Value is ITypeSymbol contextType ? contextType.Name : "JsonContext"])]
							);
						}
					}
				}
				return new BodyParameter(
					Name: parameterSymbol.Name,
					IsValid: true,
					ConfigureAwait: configureAwait,
					JsonContext: jsonContextFullyQualifiedNameWithGlobal,
					Serializer: new(
						ConfigureAwait: configureAwait,
						FullyQualifiedName: serializerTypeSymbol.ToDisplayString(),
						FullyQualifiedNameWithGlobal: serializerTypeSymbol.ToDisplayString(TypeInfo.FullyQualifiedFormat),
						TypeArguments: null,
						Diagnostics: null
					),
					Diagnostics: null
				);
			} else {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule2, new(attr.GetTypeArgumentLocation()), (ImmutableArray<string>)[serializerTypeSymbol.Name])]
				);
			}
		} else if (attrTypeName == "ParamAttribute") {
			if (parameterSymbol.Type.IsNullable()) {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule6,
						new(attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()
						?? parameterSymbol.Locations.FirstOrDefault()
						?? Location.None), null)]
				);
			}
			var alias = !attr.ConstructorArguments.IsEmpty ? (string)attr.ConstructorArguments[0].Value! : parameterSymbol.Name;
			var urlEncode = attr.NamedArguments.IsEmpty || (bool)attr.NamedArguments[0].Value.Value!;
			return new ParamParameter(
				Name: parameterSymbol.Name,
				UrlEncode: urlEncode,
				Alias: alias,
				IsValid: true,
				Serializer: new(
					FullyQualifiedName: "RestFetch.ParamSerializer",
					FullyQualifiedNameWithGlobal: "global::RestFetch.ParamSerializer",
					ConfigureAwait: false,
					TypeArguments: null,
					Diagnostics: null
				),
				SerializerInterfaceMetadataNames: (ImmutableArray<string>)["IParamSerializer`2", "IParamEncodeSerializer`2"],
				Diagnostics: null
			);
		} else if (attrTypeName == "ParamAttribute`1") {
			if (parameterSymbol.Type.IsNullable()) {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule6,
						new(attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()
						?? parameterSymbol.Locations.FirstOrDefault()
						?? Location.None), null)]
				);
			}
			var alias = !attr.ConstructorArguments.IsEmpty ? (string)attr.ConstructorArguments[0].Value! : parameterSymbol.Name;
			var urlEncode = attr.NamedArguments.IsEmpty || (bool)attr.NamedArguments[0].Value.Value!;
			var iParamSerializer = knownTypes.ParamSerializer;
			var iParamEncodeSerializer = knownTypes.ParamEncodeSerializer;
			var serializerTypeSymbol = attr.AttributeClass.TypeArguments[0];
			var interfaceList = new List<string>();
			var isParamSerializer = serializerTypeSymbol.ImplementsInterface(iParamSerializer);
			var isParamEncodeSerializer = serializerTypeSymbol.ImplementsInterface(iParamEncodeSerializer);
			if (isParamSerializer) {
				interfaceList.Add(iParamSerializer!.MetadataName);
			}
			if (isParamEncodeSerializer) {
				interfaceList.Add(iParamEncodeSerializer!.MetadataName);
			} else if (urlEncode) {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule3, new(attr.GetTypeArgumentLocation()), (ImmutableArray<string>)[serializerTypeSymbol.Name])]
				);
			}

			return !isParamSerializer && !isParamEncodeSerializer
					? new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule4, new(attr.GetTypeArgumentLocation()), (ImmutableArray<string>)[serializerTypeSymbol.Name])]
				)
					: new ParamParameter(
				Name: parameterSymbol.Name,
				UrlEncode: urlEncode,
				Alias: alias,
				IsValid: true,
				Serializer: new(
					FullyQualifiedName: serializerTypeSymbol.ToDisplayString(),
					FullyQualifiedNameWithGlobal: serializerTypeSymbol.ToDisplayString(TypeInfo.FullyQualifiedFormat),
					ConfigureAwait: false,
					TypeArguments: null,
					Diagnostics: null
				),
				SerializerInterfaceMetadataNames: interfaceList.ToImmutableArray(),
				Diagnostics: null
			);
		} else if (attrTypeName == "MetadataAttribute") {
			if (attr.ConstructorArguments.Length == 2) {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule5,
						new(attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()
						?? parameterSymbol.Locations.FirstOrDefault()
						?? Location.None), null)]
				);
			} else {
				var key = attr.ConstructorArguments.Length == 1 ? (string)attr.ConstructorArguments[0].Value! : parameterSymbol.Name;
				return new MetadataParameter(
					Name: parameterSymbol.Name,
					Key: key,
					IsValid: true,
					Diagnostics: null
				);
			}
		} else if (attrTypeName == "QueryAttribute") {
			var alias = attr.ConstructorArguments.Length == 1 ? (string)attr.ConstructorArguments[0].Value! : parameterSymbol.Name;
			var autoAlias = attr.NamedArguments.Length != 1 || (bool)attr.NamedArguments[0].Value.Value!;
			if (!IsDefaultQueryStringSerializerSupportedType(parameterSymbol.Type, null, knownTypes)) {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule14, new(parameterSymbol.Locations.FirstOrDefault() ?? Location.None), null)]
				);
			} else {
				return new QueryParameter(
					Name: parameterSymbol.Name,
					Alias: alias,
					AutoAlias: autoAlias,
					IsValid: true,
					Serializer: new(
						FullyQualifiedName: "RestFetch.WebUtilities.QueryString",
						FullyQualifiedNameWithGlobal: "global::RestFetch.WebUtilities.QueryString",
						ConfigureAwait: false,
						TypeArguments: null,
						Diagnostics: null
					),
					SerializerInterfaceMetadataNames: (ImmutableArray<string>)["IQueryStringSerializer", "IQueryStringSerializer`1", "IAliasQueryStringSerializer`1"],
					Diagnostics: null
				);
			}
		} else if (attrTypeName == "QueryAttribute`1") {
			var queryStringGeneric = knownTypes.QueryStringT;
			var iQueryStringSerializer = knownTypes.QueryStringSerializer;
			var iQueryStringGenericSerializer = knownTypes.QueryStringSerializerT;
			var iAliasQueryStringSerializer = knownTypes.AliasQueryStringSerializer;
			var serializerTypeSymbol = attr.AttributeClass.TypeArguments[0];
			var alias = attr.ConstructorArguments.Length == 1 ? (string)attr.ConstructorArguments[0].Value! : parameterSymbol.Name;
			var autoAlias = attr.NamedArguments.Length != 1 || (bool)attr.NamedArguments[0].Value.Value!;
			var interfaceList = new List<string>();
			var isIQueryStringSerializer = serializerTypeSymbol.ImplementsInterface(iQueryStringSerializer);
			var isQueryStringGenericSerializer = serializerTypeSymbol.ImplementsInterface(iQueryStringGenericSerializer);
			var isAliasQueryStringSerializer = serializerTypeSymbol.ImplementsInterface(iAliasQueryStringSerializer);
			if (serializerTypeSymbol.InheritsFrom(queryStringGeneric) && !IsDefaultQueryStringSerializerSupportedType(parameterSymbol.Type, serializerTypeSymbol, knownTypes)) {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule14, new(parameterSymbol.Locations.FirstOrDefault() ?? Location.None), null)]
				);
			}
			if (isIQueryStringSerializer) {
				interfaceList.Add("IQueryStringSerializer");
			}
			if (isQueryStringGenericSerializer) {
				interfaceList.Add("IQueryStringSerializer`1");
			}
			if (isAliasQueryStringSerializer) {
				interfaceList.Add("IAliasQueryStringSerializer`1");
			} else if (autoAlias) {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule7, new(attr.GetTypeArgumentLocation()), (ImmutableArray<string>)[serializerTypeSymbol.Name])]
				);
			}

			return !isIQueryStringSerializer && !isQueryStringGenericSerializer && !isAliasQueryStringSerializer
				? new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule8, new(attr.GetTypeArgumentLocation()), (ImmutableArray<string>)[serializerTypeSymbol.Name])]
				)
				: new QueryParameter(
				Name: parameterSymbol.Name,
				Alias: alias,
				AutoAlias: autoAlias,
				IsValid: true,
				Serializer: new(
					FullyQualifiedName: serializerTypeSymbol.ToDisplayString(),
					FullyQualifiedNameWithGlobal: serializerTypeSymbol.ToDisplayString(TypeInfo.FullyQualifiedFormat),
					ConfigureAwait: false,
					TypeArguments: null,
					Diagnostics: null
				),
				SerializerInterfaceMetadataNames: interfaceList.ToImmutableArray(),
				Diagnostics: null
			);
		} else if (attrTypeName == "HeaderAttribute") {
			if (attr.ConstructorArguments.Length == 2) {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule9,
					new(attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()
					?? parameterSymbol.Locations.FirstOrDefault()
					?? Location.None), null)]
				);
			} else if (attr.ConstructorArguments.Length == 1 && parameterSymbol.Type.SpecialType != SpecialType.System_String) {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule10,
					new(attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()
					?? parameterSymbol.Locations.FirstOrDefault()
					?? Location.None), null)]
				);
			} else if (attr.ConstructorArguments.IsEmpty && !IsEnumerableKeyPairStringString(parameterSymbol.Type, knownTypes)) {
				return new ApiParameterExtraInfo(
					Name: parameterSymbol.Name,
					Kind: ApiParameterKind.None,
					IsValid: false,
					Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule11,
					new(attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()
					?? parameterSymbol.Locations.FirstOrDefault()
					?? Location.None), null)]
				);
			} else {
				return new HeaderParameter(
					Name: parameterSymbol.Name,
					Key: attr.ConstructorArguments.IsEmpty ? null : (string?)attr.ConstructorArguments[0].Value,
					IsValid: true,
					Diagnostics: null
				);
			}
		} else {
			return new ApiParameterExtraInfo(
				Name: parameterSymbol.Name,
				Kind: ApiParameterKind.None,
				IsValid: false,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ParameterRule12,
				new(parameterSymbol.Locations.FirstOrDefault() ?? Location.None), null)]
			);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsEnumerableKeyPairStringString(ITypeSymbol symbol, KnownTypes knownTypes) {
		var targetInterface = knownTypes.IEnumerableT?
			.Construct(
				knownTypes.KeyValuePair!
			.Construct(
				knownTypes.SystemString!,
				knownTypes.SystemString!
			)
		);
		return SymbolEqualityComparer.Default.Equals(symbol, targetInterface)
    || symbol.AllInterfaces.Any(iface => SymbolEqualityComparer.Default.Equals(iface, targetInterface));
	}

	public static bool IsDefaultQueryStringSerializerSupportedType(ITypeSymbol paramType, ITypeSymbol? serializerSymbol, KnownTypes knownTypes, bool end = false) {
		return true;
		// var querystringType = compilation.GetTypeByMetadataName("RestFetch.WebUtility.QueryString");
		// var querystringGenericType = compilation.GetTypeByMetadataName("RestFetch.WebUtility.QueryString`1");
		// var fastQueryStringType = compilation.GetTypeByMetadataName("RestFetch.WebUtility.FastQueryString");
		// var fastQueryStringGenericType = compilation.GetTypeByMetadataName("RestFetch.WebUtility.FastQueryString`1");
		// var isQueryString = paramType.InheritsFrom(querystringType);
		// var isQueryStringGeneric = paramType.InheritsFrom(querystringGenericType);
		// var isFastQueryString = paramType.InheritsFrom(fastQueryStringType);
		// var isFastQueryStringGeneric = paramType.InheritsFrom(fastQueryStringGenericType);

		// if (isQueryString || isFastQueryString) {
		// 	return true;
		// }

		// if (
		// 	paramType.SpecialType == SpecialType.System_Int16
		// 	|| paramType.SpecialType == SpecialType.System_Int32
		// 	|| paramType.SpecialType == SpecialType.System_Int64
		// 	|| paramType.SpecialType == SpecialType.System_UInt16
		// 	|| paramType.SpecialType == SpecialType.System_UInt32
		// 	|| paramType.SpecialType == SpecialType.System_UInt64
		// 	|| paramType.SpecialType == SpecialType.System_Byte
		// 	|| paramType.SpecialType == SpecialType.System_Single
		// 	|| paramType.SpecialType == SpecialType.System_Double
		// 	|| paramType.SpecialType == SpecialType.System_Decimal
		// 	|| paramType.SpecialType == SpecialType.System_String
		// 	|| paramType.SpecialType == SpecialType.System_Boolean
		// 	) {
		// 	return true;
		// }


		// var isNullable = paramType.IsNullable();
		// if (isNullable && (paramType.SpecialType == SpecialType.System_String || paramType.SpecialType == SpecialType.System_Decimal)) {
		// 	return true;
		// } else if (isNullable && paramType is INamedTypeSymbol {
		// 	SpecialType: SpecialType.System_Nullable_T,
		// 	OriginalDefinition: var originalType
		// }) {
		// 	if (
		// 		originalType.SpecialType == SpecialType.System_Int16
		// 		|| originalType.SpecialType == SpecialType.System_Int32
		// 		|| originalType.SpecialType == SpecialType.System_Int64
		// 		|| originalType.SpecialType == SpecialType.System_UInt16
		// 		|| originalType.SpecialType == SpecialType.System_UInt32
		// 		|| originalType.SpecialType == SpecialType.System_UInt64
		// 		|| originalType.SpecialType == SpecialType.System_Byte
		// 		|| originalType.SpecialType == SpecialType.System_Single
		// 		|| originalType.SpecialType == SpecialType.System_Double
		// 		|| originalType.SpecialType == SpecialType.System_Boolean
		// 		) {
		// 		return true;
		// 	}
		// }

		// if (paramType is IArrayTypeSymbol arrayTypeSymbol && end) {
		// 	return IsDefaultQueryStringSerializerSupportedType(arrayTypeSymbol.ElementType, serializerSymbol, compilation, true);
		// }
		// // TODO IEnumerable<T>和IEnumerable<KeyValuePair<string, T>>

		// if (serializerSymbol is not null && SymbolEqualityComparer.Default.Equals(serializerSymbol, querystringType)) {
		// 	return IsDefaultQueryStringSerializerSupportedType(paramType, null, compilation);
		// } else if (serializerSymbol is not null && serializerSymbol.InheritsFrom(querystringGenericType)) {
		// 	// TODO
		// }
		// return false;
	}
}

public sealed record BodyParameter(
	string Name,
	bool IsValid,
	EquatableArray<DiagnosticInfo>? Diagnostics,

	bool ConfigureAwait,
	string? JsonContext,
	HookInfo Serializer
): ApiParameterExtraInfo(
	Name,
	Kind: ApiParameterKind.Body,
	IsValid,
	Diagnostics
);

public sealed record ParamParameter(
	string Name,
	bool IsValid,
	EquatableArray<DiagnosticInfo>? Diagnostics,

	HookInfo Serializer,
	EquatableArray<string> SerializerInterfaceMetadataNames,
	string Alias,
	bool UrlEncode
): ApiParameterExtraInfo(
	Name,
	Kind: ApiParameterKind.Param,
	IsValid,
	Diagnostics
);

public sealed record QueryParameter(
	string Name,
	bool IsValid,
	EquatableArray<DiagnosticInfo>? Diagnostics,

	HookInfo Serializer,
	EquatableArray<string> SerializerInterfaceMetadataNames,
	string? Alias,
	bool AutoAlias
): ApiParameterExtraInfo(
	Name,
	Kind: ApiParameterKind.QueryString,
	IsValid,
	Diagnostics
);

public sealed record MetadataParameter(
	string Name,
	bool IsValid,
	EquatableArray<DiagnosticInfo>? Diagnostics,

	string Key
): ApiParameterExtraInfo(
	Name,
	Kind: ApiParameterKind.Metadata,
	IsValid,
	Diagnostics
);

public sealed record HeaderParameter(
	string Name,
	bool IsValid,
	EquatableArray<DiagnosticInfo>? Diagnostics,

	string? Key
): ApiParameterExtraInfo(
	Name,
	Kind: ApiParameterKind.Header,
	IsValid,
	Diagnostics
);

public sealed record CancellationTokenParameter(
	string Name,
	bool IsValid,
	EquatableArray<DiagnosticInfo>? Diagnostics
): ApiParameterExtraInfo(
	Name,
	Kind: ApiParameterKind.Header,
	IsValid,
	Diagnostics
);