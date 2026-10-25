using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RestFetch.SourceGenerator;

public sealed record ApiDefinitionMethodInfo(
	MethodInfo MethodInfo,
	bool IsValid,
	bool ReturnsVoid,
	bool ReturnsStream,
	bool ReturnsRawResponse,
	bool ReturnsIAsyncEnumerableT,
	bool IsMethodBaseAddress,
	string Path,
	string HttpMethod,
	string BaseAddress,
	string IdGenerator,
	string DeserializedResponseTypeName,
	ReturnTypeWrapperKind AsyncReturnTypeKind,
	ReturnTypeKind ReturnDataTypeKind,
	TypeInfo? ExceptedReturnType,
	TypeInfo? ReturnDataType,
	HookInfo? BeforeSendHook,
	HookInfo? AfterResponseHook,
	HookInfo HttpClientProxy,
	HookInfo ResponseBodyDeserializer,
	HookInfo? OnError,
	HookInfo? OnSseEventError,
	HookInfo? OnSuccess,
	HookInfo ExceptionFactory,
	EquatableArray<KeyValuePairInfo> StaticHeaders,
	EquatableArray<KeyValuePairInfo> StaticMetadata,
	EquatableArray<ApiParameterExtraInfo> ApiParameterExtraInfos,
	EquatableArray<PathSegment>? PathSegments,
	EquatableArray<DiagnosticInfo>? Diagnostics
) {
	private static readonly Dictionary<string, string> HttpMethodAttrMap = new() {
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.GetAttribute", "global::System.Net.Http.HttpMethod.Get" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.PostAttribute", "global::System.Net.Http.HttpMethod.Post" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.PutAttribute", "global::System.Net.Http.HttpMethod.Put" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.PatchAttribute", "global::System.Net.Http.HttpMethod.Patch" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.DeleteAttribute", "global::System.Net.Http.HttpMethod.Delete" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.HeadAttribute", "global::System.Net.Http.HttpMethod.Head" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.OptionsAttribute", "global::System.Net.Http.HttpMethod.Options"  },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.ConnectAttribute", "global::System.Net.Http.HttpMethod.Connect"  },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.TraceAttribute", "global::System.Net.Http.HttpMethod.Trace"  },
	};
	private static readonly DiagnosticDescriptor HeaderRule0 = new("RestFetch0030",
		"错误的Metadata构造函数",
		@"静态Metadata必须使用双参数构造函数.eg.[Metadata(""key"", ""value"")]",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule6 = new("RestFetch0026",
		"错误的Response类型",
		"Response<TDeserializer>中TDeserializer必须实现IResponseBodyDeserializer或IResponseBodyDeserializer<TResponse>",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HttpMethodRule0 = new("RestFetch0040",
		"不允许多个HttpMethodAttribute修饰同一个方法",
		"方法不能同时标记多个HTTP方法特性(如[Get],[Post]等",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HttpMethodRule1 = new("RestFetch0041",
		"HTTP方法必须修饰partial方法定义或接口方法定义",
		"HTTP方法必须修饰partial方法定义或接口方法定义",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HttpMethodRule2 = new("RestFetch0042",
		"方法必须有一个HttpMethodAttribute标记",
		"方法必须有一个HttpMethodAttribute标记(如[Get],[Post]等",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor ReturnTypeRule0 = new("RestFetch0080",
		"错误的返回类型",
		"返回类型必须是Task/Task<T>/ValueTask/ValueTask<T>/IAsyncEnumerable<T>/IObservable<T>",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor ParameterRule17 = new("RestFetch0067",
		"错误的Path格式",
		"Path中的{}需要闭合",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor ParameterRule18 = new("RestFetch0068",
		"错误的Path参数",
		"Path中未找到{0}对应的参数",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	public static ApiDefinitionMethodInfo CreateFromSymbol(IMethodSymbol symbol, KnownTypes knownTypes, ApiDefinitionTypeInfo apiDefinitionType) {
		var methodInfo = MethodInfo.CreateFromSymbol(symbol, knownTypes);
		var attrList = symbol.GetAttributes();
		var baseAddress = ApiDefinitionTypeInfo.GetBaseAddress(attrList, knownTypes);
		var isMethodBaseAddress = baseAddress is not null;
		if (!isMethodBaseAddress) {
			baseAddress = apiDefinitionType.BaseAddress;
		}
		var staticHeaders = ApiDefinitionTypeInfo.GetStaticHeaders(attrList, knownTypes).ToArray();
		var validStaticHeaders = staticHeaders
			.Where(static header => header.Diagnostics is null)
			.Union(apiDefinitionType.StaticHeaders, HeaderEqualityComparer.Default)
			.Where(static header => header.Value is not null);
			// 去重相同的header,但是允许方法级的覆盖类级
			// .Distinct(HeaderEqualityComparer.Default);
		var headerErrors = staticHeaders
			.Where(static header => header.Diagnostics is not null)
			.SelectMany(static header => header.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArray())
			.ToArray();
		var staticMetadata = GetStaticMetadata(attrList, knownTypes).ToArray();
		var validStaticMetadata = staticMetadata
			.Where(static metadata => metadata.Diagnostics is null)
			.GroupBy(static header => header.Key, StringComparer.OrdinalIgnoreCase)
			.Select(static g => g.LastOrDefault())
			.Distinct();
		var metadataErrors = staticMetadata
			.Where(static metadata => metadata.Diagnostics is not null)
			.SelectMany(static metadata => metadata.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArray())
			.ToArray();
		var idGenerator = ApiDefinitionTypeInfo.GetIdGenerator(attrList, knownTypes) ?? apiDefinitionType.IdGenerator;
		var beforeSendHook = ApiDefinitionTypeInfo.GetBeforeSendHook(attrList, knownTypes) ?? apiDefinitionType.BeforeSendHook;
		var afterResponseHook = ApiDefinitionTypeInfo.GetAfterResponseHook(attrList, knownTypes) ?? apiDefinitionType.AfterResponseHook;
		var exceptionFactory = ApiDefinitionTypeInfo.GetExceptionFactory(attrList, knownTypes) ?? apiDefinitionType.ExceptionFactory;
		var onError = ApiDefinitionTypeInfo.GetOnError(attrList, knownTypes) ?? apiDefinitionType.OnError;
		var onSseEventError = ApiDefinitionTypeInfo.GetOnSseEventError(attrList, knownTypes) ?? apiDefinitionType.OnSseEventError;
		var onSuccess = ApiDefinitionTypeInfo.GetOnSuccess(attrList, knownTypes) ?? apiDefinitionType.OnSuccess;
		// TODO 应当根据方法的返回类型提供默认值,应当检查返回类型是否被默认反序列化器支持

		var miscErrors = new List<DiagnosticInfo>();
		var parameterExtraInfos = symbol.Parameters.Select(parameter => ApiParameterExtraInfo.CreateFromSymbol(parameter, knownTypes)).ToArray();
		var hasQueryString = parameterExtraInfos.Any(static p => p is QueryParameter);
		var paramParameters = parameterExtraInfos.Where(static p => p is ParamParameter).Select(static p => (ParamParameter)p).ToArray();
		var (path, httpMethod, pathSegments, httpAttrErrors) = GetPathAndHttpMethod(attrList, knownTypes, symbol, paramParameters, hasQueryString);
		var hasMultipleBody = parameterExtraInfos.Where(static item => item.Kind == ApiParameterKind.Body).ToArray().Length > 1;
		var isValid = parameterExtraInfos.All(static param => param.IsValid) && !hasMultipleBody && httpAttrErrors is null;
		var paramErrors = parameterExtraInfos.SelectMany(static param => param.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArray());

		if (hasMultipleBody) {
			paramErrors = paramErrors.Concat(Enumerable.Repeat(
				new DiagnosticInfo(
					ApiParameterExtraInfo.ParameterRule13,
					new(symbol.Locations.FirstOrDefault() ?? Location.None),
					null
				)
			, 1));
		}

		if (!methodInfo.IsAsyncReturnType) {
			isValid = false;
			miscErrors.Add(new(
				ReturnTypeRule0,
				new(symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is MethodDeclarationSyntax methodSyntax
					? methodSyntax.ReturnType.GetLocation()
					: symbol.Locations.FirstOrDefault() ?? Location.None),
				null
			));
		}

		var returnVoid = SymbolEqualityComparer.Default.Equals(symbol.ReturnType.OriginalDefinition, knownTypes.Task)
			|| SymbolEqualityComparer.Default.Equals(symbol.ReturnType.OriginalDefinition, knownTypes.ValueTask);

		var deserializedRespTypeName = "";
		var returnsStream = false;
		var returnsRawResponse = false;
		var returnsIAsyncEnumerableT = symbol.ReturnType.OriginalDefinition.InheritsFrom(knownTypes.IAsyncEnumerableT);
		TypeInfo? returnDataType = null;
		TypeInfo exceptReturnTypeInfo = default!;
		if (symbol.ReturnType is INamedTypeSymbol {
			IsGenericType: true,
			TypeArguments: [var exceptedReturnType, ..]
		}) {
			exceptReturnTypeInfo = TypeInfo.CreateFromSymbolWithoutBaseTypeAndMethods(exceptedReturnType, knownTypes);
			if (exceptedReturnType.InheritsFrom(knownTypes.Stream)) {
				returnsStream = true;
			}
			if (exceptedReturnType.ToDisplayString(TypeInfo.FullyQualifiedFormat) == apiDefinitionType.TResponse) {
				returnsRawResponse = true;
			}

			if (returnsIAsyncEnumerableT) {
				var deserializedRespType = knownTypes.IAsyncEnumerableT!.Construct(knownTypes.SseEventResultT!.Construct(exceptedReturnType));
				deserializedRespTypeName = deserializedRespType.ToDisplayString(TypeInfo.FullyQualifiedFormat);
			}

			if (exceptedReturnType is INamedTypeSymbol {
				IsGenericType: true,
				TypeArguments: [ITypeSymbol returnDatType, ..]
			}) {
				if (exceptedReturnType.OriginalDefinition.InheritsFrom(knownTypes.SseEventT)
					|| exceptedReturnType.OriginalDefinition.InheritsFrom(knownTypes.SseItemT)
					|| exceptedReturnType.OriginalDefinition.InheritsFrom(knownTypes.ApiResponseT)) {
					returnDataType = TypeInfo.CreateFromSymbolWithoutBaseTypeAndMethods(returnDatType, knownTypes);
				}
			} else {
				returnDataType = TypeInfo.CreateFromSymbolWithoutBaseTypeAndMethods(exceptedReturnType, knownTypes);
			}
		}

		var asyncReturnTypeKind = symbol.ReturnType.GetAsyncReturnTypeKind(knownTypes);
		var returnDataTypeKind = symbol.ReturnType.GetResponseTypeKind(knownTypes, returnsRawResponse);
		// var returnWrapperTypeIsTask = symbol.ReturnType.OriginalDefinition.InheritsFrom(knownTypes.TaskT) || symbol.ReturnType.OriginalDefinition.InheritsFrom(knownTypes.ValueTaskT);
		// var returnWrapperTypeIsAsyncEnumerable = symbol.ReturnType.OriginalDefinition.InheritsFrom(knownTypes.IAsyncEnumerableT);
		var responseBodyDeserializer = GetResponseBodyDeserializer(attrList, knownTypes);
		var defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.Json";
		var callWithGenericArgs = false;
		var isSseDeserializer = false;
		if (responseBodyDeserializer is null && returnDataType is not null) {
			if (asyncReturnTypeKind == ReturnTypeWrapperKind.TaskT || asyncReturnTypeKind == ReturnTypeWrapperKind.ValueTaskT) {
				if (returnDataTypeKind == ReturnTypeKind.SystemString) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.SystemString";
				} else if (returnDataTypeKind == ReturnTypeKind.Stream) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.Stream";
				} else if (returnDataTypeKind == ReturnTypeKind.JsonNode) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.JsonNode";
				} else if (returnDataTypeKind == ReturnTypeKind.JsonDocument) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.JsonDocument";
				} else if (returnDataTypeKind == ReturnTypeKind.Other) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.Json";
					callWithGenericArgs = true;
				} else if (returnDataTypeKind == ReturnTypeKind.ApiResponseT
					|| returnDataTypeKind == ReturnTypeKind.ApiResponseString
					|| returnDataTypeKind == ReturnTypeKind.ApiResponseJsonNode
					|| returnDataTypeKind == ReturnTypeKind.ApiResponseJsonDocument) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.ApiResponse<{returnDataType.FullyQualifiedNameWithGlobal}>";
				}
			} else if (asyncReturnTypeKind == ReturnTypeWrapperKind.IAsyncEnumerableT) {
				isSseDeserializer = true;
				if (returnDataTypeKind == ReturnTypeKind.SystemString) {
					if (returnDataType.IsNullable) {
						defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.SseOrJsonArray";
						callWithGenericArgs = true;
					} else {
						defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.SseString";
					}
				} else if (returnDataTypeKind == ReturnTypeKind.JsonNode) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.SseOrJsonArray";
					callWithGenericArgs = true;
				} else if (returnDataTypeKind == ReturnTypeKind.JsonDocument) {
					if (returnDataType.IsNullable) {
						defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.SseOrJsonArray";
						callWithGenericArgs = true;
					} else {
						defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.SseJsonDocument";
					}
				} else if (returnDataTypeKind == ReturnTypeKind.SseEvent) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.SseOrJsonArray";
					callWithGenericArgs = true;
				} else if (returnDataTypeKind == ReturnTypeKind.Other) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.SseOrJsonArray";
					callWithGenericArgs = true;
				} else if (returnDataTypeKind == ReturnTypeKind.SseEventT
					|| returnDataTypeKind == ReturnTypeKind.SseEventString
					|| returnDataTypeKind == ReturnTypeKind.SseEventJsonNode
					|| returnDataTypeKind == ReturnTypeKind.SseEventJsonDocument) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.SseEvent<{returnDataType.FullyQualifiedNameWithGlobal}>";
				} else if (returnDataTypeKind == ReturnTypeKind.SseItemT
					|| returnDataTypeKind == ReturnTypeKind.SseItemString
					|| returnDataTypeKind == ReturnTypeKind.SseItemJsonNode
					|| returnDataTypeKind == ReturnTypeKind.SseItemJsonDocument) {
					defaultDeserializer = $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseBodyDeserializer.SseItem<{returnDataType.FullyQualifiedNameWithGlobal}>";
				}

			} else {
				// TODO
			}
		} else {
				// TODO
		}
		responseBodyDeserializer = responseBodyDeserializer ?? new(
			FullyQualifiedName: defaultDeserializer,
			FullyQualifiedNameWithGlobal: defaultDeserializer,
			ConfigureAwait: false,
			Diagnostics: null,
			TypeArguments: null,
			JsonContext: null,
			CallWithGenericArgs: callWithGenericArgs,
			IsSseDeserializer: isSseDeserializer
		);


		// 对于IAsyncEnumerable<T>应当去掉async关键字
		// var newMethodInfo = methodInfo with {
		// 	IsAsyncReturnType = methodInfo.IsAsyncReturnType && !symbol.ReturnType.OriginalDefinition.InheritsFrom(knownTypes.IAsyncEnumerableT)
		// };

		return new(
			DeserializedResponseTypeName: deserializedRespTypeName,
			MethodInfo: methodInfo,
			IsValid: isValid,
			IsMethodBaseAddress: isMethodBaseAddress,
			ReturnsVoid: returnVoid,
			ReturnsIAsyncEnumerableT: returnsIAsyncEnumerableT,
			ReturnsStream: returnsStream,
			ReturnsRawResponse: returnsRawResponse,
			AsyncReturnTypeKind: asyncReturnTypeKind,
			ReturnDataTypeKind: returnDataTypeKind,
			ExceptedReturnType: returnVoid ? null : exceptReturnTypeInfo,
			ReturnDataType: returnDataType,
			Path: path,
			PathSegments: pathSegments?.ToImmutableArray(),
			HttpMethod: httpMethod,
			ApiParameterExtraInfos: parameterExtraInfos.ToImmutableArray(),
			IdGenerator: idGenerator,
			BaseAddress: baseAddress!,
			StaticHeaders: validStaticHeaders.ToImmutableArray(),
			StaticMetadata: validStaticMetadata.ToImmutableArray(),
			BeforeSendHook: beforeSendHook,
			AfterResponseHook: afterResponseHook,
			HttpClientProxy: apiDefinitionType.HttpClientProxy,
			ExceptionFactory: exceptionFactory,
			ResponseBodyDeserializer: responseBodyDeserializer!.Value,
			OnError: onError,
			OnSseEventError: onSseEventError,
			OnSuccess: onSuccess,
			Diagnostics: ImmutableArrayExtension.CombineSafe(
				headerErrors,
				metadataErrors,
				httpAttrErrors,
				paramErrors,
				beforeSendHook?.Diagnostics,
				afterResponseHook?.Diagnostics,
				exceptionFactory.Diagnostics,
				onError?.Diagnostics,
				onSseEventError?.Diagnostics,
				onSuccess?.Diagnostics,
				responseBodyDeserializer?.Diagnostics,
				miscErrors.ToImmutableArray()
			)
		);
	}

	public static IEnumerable<KeyValuePairInfo> GetStaticMetadata(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var metadataAttrs = attrs.GetAttributesBySymbol(knownTypes.MetadataAttribute);
		foreach (var metadataAttr in metadataAttrs) {
			if (metadataAttr.ConstructorArguments.Length == 2) {
				yield return new(
					Key: (string)metadataAttr.ConstructorArguments[0].Value!,
					Value: (string)metadataAttr.ConstructorArguments[1].Value!,
					Diagnostics: null
				);
			} else {
				var errLocation = metadataAttr.ApplicationSyntaxReference?.GetSyntax().GetLocation();
				yield return new(
					Key: "",
					Value: null,
					Diagnostics: errLocation is null ? [] : (ImmutableArray<DiagnosticInfo>)[new(HeaderRule0, new(errLocation), null)]
				);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetResponseBodyDeserializer(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var responseBodyDeserializer = attrs.GetAttributesBySymbol(knownTypes.ResponseAttribute).FirstOrDefault();
		if (responseBodyDeserializer is null || responseBodyDeserializer.AttributeClass is null) {
			return null;
		}
		var targetType = responseBodyDeserializer.AttributeClass.TypeArguments[0];
		var configureAwait = false;
		var skipNullableCheck = false;
		string? jsonContextFullyQualifiedNameWithGlobal = null;

		foreach (var item in responseBodyDeserializer.NamedArguments) {
			// var (key, value) = item;
			if (item.Key == "ConfigureAwait") {
				configureAwait = (bool)item.Value.Value!;
			} else if (item.Key == "SkipNullableCheck") {
				skipNullableCheck = (bool)item.Value.Value!;
			} else if (item.Key == "JsonContext") {
				if (item.Value.Value is INamedTypeSymbol jsonContextType && jsonContextType.InheritsFrom(knownTypes.JsonSerializerContext)) {
					jsonContextFullyQualifiedNameWithGlobal = jsonContextType.ToDisplayString(TypeInfo.FullyQualifiedFormat);
				} else {
					return new(
						FullyQualifiedName: targetType.ToDisplayString(),
						FullyQualifiedNameWithGlobal: targetType.ToDisplayString(TypeInfo.FullyQualifiedFormat),
						JsonContext: jsonContextFullyQualifiedNameWithGlobal,
						ConfigureAwait: configureAwait,
						TypeArguments: null,
						Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(ApiParameterExtraInfo.ParameterRule16, new(responseBodyDeserializer.GetNamedArgumentLocation("JsonContext")),
							(ImmutableArray<string>)[item.Value.Value is ITypeSymbol contextType ? contextType.Name : "JsonContext"])]
					);
				}
			}
		}

		var implItf0 = targetType.ImplementsInterface(knownTypes.ResponseBodyDeserializer);
		var implItf1 = targetType.ImplementsInterface(knownTypes.ResponseBodyGenericDeserializer1);
		var implItf2 = targetType.ImplementsInterface(knownTypes.ResponseBodyGenericDeserializer2);
		var implItf3 = targetType.ImplementsInterface(knownTypes.ResponseAsyncEnumerableDeserializer);
		var implItf4 = targetType.ImplementsInterface(knownTypes.ResponseAsyncEnumerableGenericDeserializer1);
		var implItf5 = targetType.ImplementsInterface(knownTypes.ResponseAsyncEnumerableGenericDeserializer2);

		if (implItf0 || implItf1 || implItf2 || implItf3 || implItf4 || implItf5) {
			return new(
				FullyQualifiedName: targetType.ToDisplayString(),
				FullyQualifiedNameWithGlobal: targetType.ToDisplayString(TypeInfo.FullyQualifiedFormat),
				JsonContext: jsonContextFullyQualifiedNameWithGlobal,
				ConfigureAwait: configureAwait,
				SkipNullableCheck: skipNullableCheck,
				CallWithGenericArgs: implItf1 || implItf4,
				IsSseDeserializer: implItf3 || implItf4 || implItf5,
				TypeArguments: null,
				Diagnostics: null
			);
		} else {
			return new(
				FullyQualifiedName: targetType.ToDisplayString(),
				FullyQualifiedNameWithGlobal: targetType.ToDisplayString(TypeInfo.FullyQualifiedFormat),
				JsonContext: jsonContextFullyQualifiedNameWithGlobal,
				ConfigureAwait: configureAwait,
				TypeArguments: null,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule6, new(responseBodyDeserializer.GetTypeArgumentLocation()), null)]
			);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (string Path, string HttpMethod, List<PathSegment>? pathSegments, List<DiagnosticInfo>? Diagnostics) GetPathAndHttpMethod(
		ImmutableArray<AttributeData> attrs,
		KnownTypes knownTypes,
		IMethodSymbol methodSymbol,
		ParamParameter[] @params,
		bool hasQueryString) {
		var httpMethodAttrBase = knownTypes.HttpMethodAttribute;
		var httpMethodAttrs = attrs
			.Where(attr => attr.AttributeClass.InheritsFrom(httpMethodAttrBase))
			.ToArray();
		// var httpMethodAttrs = HttpMethodAttrMap.Keys.SelectMany(httpMethodAttr => attrs.GetAttributesByFullName(httpMethodAttr, compilation)).ToArray();
		if (httpMethodAttrs.Length > 1) {
			var errors = new List<DiagnosticInfo>();
			foreach (var attr in httpMethodAttrs) {
				var errLocation = attr.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? methodSymbol.Locations.FirstOrDefault() ?? Location.None;
				errors.Add(new(HttpMethodRule0, new(errLocation), null));
			}
			return ("", "", null, errors);
		} else if (!(methodSymbol.IsPartialDefinition || methodSymbol.ContainingType.TypeKind == TypeKind.Interface) && httpMethodAttrs.Length == 1) {
			var errLocation = httpMethodAttrs[0].ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? methodSymbol.Locations.FirstOrDefault() ?? Location.None;
			return ("", "", null, [new(HttpMethodRule1, new(errLocation), null)]);
			// 报错
		} else if ((methodSymbol.IsPartialDefinition || methodSymbol.ContainingType.TypeKind == TypeKind.Interface) && httpMethodAttrs.Length == 0) {
			return ("", "", null, [new(HttpMethodRule2, new(methodSymbol.Locations.FirstOrDefault() ?? Location.None), null)]);
		} else {
			var path = httpMethodAttrs[0].ConstructorArguments.Length > 0 ? (string?)httpMethodAttrs[0].ConstructorArguments[0].Value ?? "/" : "/";
			var method = HttpMethodAttrMap[httpMethodAttrs[0].AttributeClass!.ToDisplayString()];

			if (hasQueryString) {
				if (!path.Contains('?')) {
					path += '?';
				} else {
					path += '&';
				}
			}

			if (!PathSegment.TryParse(path, out var paramSegments)) {
				var attrSyntax = httpMethodAttrs[0].ApplicationSyntaxReference?.GetSyntax();
				var errLocation = Location.None;
				if (attrSyntax is AttributeSyntax syntax) {
					errLocation = syntax.ArgumentList?.Arguments.FirstOrDefault()?.GetLocation() ?? Location.None;
				}
				return ("", "", null, [new(ParameterRule17, new(errLocation), null)]);
			}

			foreach (var seg in paramSegments) {
				if (seg is ParamSegment paramSeg && !@params.Any(p => p.Alias == paramSeg.RootParamName)) {
					var attrSyntax = httpMethodAttrs[0].ApplicationSyntaxReference?.GetSyntax();
					var errLocation = Location.None;
					if (attrSyntax is AttributeSyntax syntax) {
						errLocation = syntax.ArgumentList?.Arguments.FirstOrDefault()?.GetLocation() ?? Location.None;
					}
					return ("", "", null, [new(ParameterRule18, new(errLocation), new string[] { paramSeg.RootParamName }.ToImmutableArray())]);
				}
			}
			return (path, method, paramSegments, null);
		}
	}
}