using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public sealed record ApiDefinitionMethodInfo(
	MethodInfo MethodInfo,
	bool IsValid,
	string Path,
	string HttpMethod,
	string BaseAddress,
	string IdGenerator,
	ReturnTypeKind AsyncReturnTypeKind,
	HookInfo? BeforeSendHook,
	HookInfo? AfterResponseHook,
	HookInfo HttpClientProxy,
	HookInfo ResponseBodyDeserializer,
	HookInfo? OnError,
	HookInfo? OnSuccess,
	HookInfo ExceptionFactory,
	EquatableArray<KeyValuePairInfo> StaticHeaders,
	EquatableArray<KeyValuePairInfo> StaticMetadata,
	EquatableArray<ApiParameterExtraInfo> ApiParameterExtraInfos,
	EquatableArray<DiagnosticInfo>? Diagnostics
) {
	private static readonly Dictionary<string, string> HttpMethodAttrMap = new() {
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.GetAttribute", "HttpMethod.Get" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.PostAttribute", "HttpMethod.Post" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.PutAttribute", "HttpMethod.Put" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.PatchAttribute", "HttpMethod.Patch" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.DeleteAttribute", "HttpMethod.Delete" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.HeadAttribute", "HttpMethod.Head" },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.OptionsAttribute", "HttpMethod.Options"  },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.ConnectAttribute", "HttpMethod.Connect"  },
		{ $"{RestFetchSourceGenerator.RestFetchNamespaceName}.TraceAttribute", "HttpMethod.Trace"  },
	};
	private static readonly DiagnosticDescriptor HeaderRule0 = new("RestFetch0030",
		"错误的Metadata构造函数",
		@"静态Metadata必须使用双参数构造函数.eg.[Metadata(""key"", ""value"")]",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule6 = new("RestFetch0026",
		"错误的Response类型",
		"Response<TDeserializer>中TDeserializer必须实现IResponseBodyDeserializer或IResponseBodyDeserializer<TURL, TDisposer, TResponse, TProxy>",
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

	public static ApiDefinitionMethodInfo CreateFromSymbol(IMethodSymbol symbol, KnownTypes knownTypes, ApiDefinitionTypeInfo apiDefinitionType) {
		var methodInfo = MethodInfo.CreateFromSymbol(symbol, knownTypes);
		var attrList = symbol.GetAttributes();
		var baseAddress = ApiDefinitionTypeInfo.GetBaseAddress(attrList, knownTypes) ?? apiDefinitionType.BaseAddress;
		var staticHeaders = ApiDefinitionTypeInfo.GetStaticHeaders(attrList, knownTypes);
		var validStaticHeaders = staticHeaders
			.Where(static header => header.Diagnostics is null)
			.Union(apiDefinitionType.StaticHeaders.ToImmutableArraySafe(), HeaderEqualityComparer.Default)
			.Where(static header => header.Value is not null);
			// 去重相同的header,但是允许方法级的覆盖类级
			// .Distinct(HeaderEqualityComparer.Default);
		var headerErrors = staticHeaders
			.Where(static header => header.Diagnostics is not null)
			.SelectMany(static header => header.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArraySafe());
		var staticMetadata = GetStaticMetadata(attrList, knownTypes);
		var validStaticMetadata = staticMetadata
			.Where(static metadata => metadata.Diagnostics is null)
			.GroupBy(static header => header.Key, StringComparer.OrdinalIgnoreCase)
			.Select(static g => g.LastOrDefault())
			.Distinct();
		var metadataErrors = staticMetadata
			.Where(static metadata => metadata.Diagnostics is not null)
			.SelectMany(static metadata => metadata.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArraySafe());
		var idGenerator = ApiDefinitionTypeInfo.GetIdGenerator(attrList, knownTypes) ?? apiDefinitionType.IdGenerator;
		var beforeSendHook = ApiDefinitionTypeInfo.GetBeforeSendHook(attrList, knownTypes) ?? apiDefinitionType.BeforeSendHook;
		var afterResponseHook = ApiDefinitionTypeInfo.GetAfterResponseHook(attrList, knownTypes) ?? apiDefinitionType.AfterResponseHook;
		var exceptionFactory = ApiDefinitionTypeInfo.GetExceptionFactory(attrList, knownTypes) ?? apiDefinitionType.ExceptionFactory;
		var onError = ApiDefinitionTypeInfo.GetOnError(attrList, knownTypes) ?? apiDefinitionType.OnError;
		var onSuccess = ApiDefinitionTypeInfo.GetOnSuccess(attrList, knownTypes) ?? apiDefinitionType.OnSuccess;
		// TODO 应当根据方法的返回类型提供默认值,应当检查返回类型是否被默认反序列化器支持
		var responseBodyDeserializer = GetResponseBodyDeserializer(attrList, knownTypes) ?? new();
		var (path, method, httpAttrErrors) = GetPathAndHttpMethod(attrList, knownTypes, symbol);

		var parameterExtraInfos = symbol.Parameters.Select(parameter => ApiParameterExtraInfo.CreateFromSymbol(parameter, knownTypes));
		var hasMultipleBody = parameterExtraInfos.Where(static item => item.Kind == ApiParameterKind.Body).ToArray().Length > 1;
		var isValid = parameterExtraInfos.All(static param => param.IsValid) && !hasMultipleBody;
		var paramErrors = parameterExtraInfos.SelectMany(static param => param.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArraySafe());

		if (hasMultipleBody) {
			paramErrors = paramErrors.Concat(Enumerable.Repeat(
				new DiagnosticInfo(
					ApiParameterExtraInfo.ParameterRule13,
					new(symbol.Locations.FirstOrDefault() ?? Location.None),
					null
				)
			, 1));
		}


		return new(
			MethodInfo: methodInfo,
			IsValid: isValid,
			AsyncReturnTypeKind: symbol.ReturnType.GetAsyncReturnTypeKind(knownTypes),
			Path: path,
			HttpMethod: method,
			ApiParameterExtraInfos: parameterExtraInfos.ToImmutableArraySafe(),
			IdGenerator: idGenerator,
			BaseAddress: baseAddress,
			StaticHeaders: validStaticHeaders.ToImmutableArraySafe(),
			StaticMetadata: validStaticMetadata.ToImmutableArraySafe(),
			BeforeSendHook: beforeSendHook,
			AfterResponseHook: afterResponseHook,
			HttpClientProxy: apiDefinitionType.HttpClientProxy,
			ExceptionFactory: exceptionFactory,
			ResponseBodyDeserializer: responseBodyDeserializer,
			OnError: onError,
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
				onSuccess?.Diagnostics
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
		var responseBodyDeserializerInterface = knownTypes.ResponseBodyDeserializer;
		var responseBOdyDeserializerGenericInterface = knownTypes.ResponseBodyGenericDeserializer;
		var targetType = responseBodyDeserializer.AttributeClass.TypeArguments[0];
		var configureAwait = !responseBodyDeserializer.NamedArguments.IsEmpty && (bool)responseBodyDeserializer.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(responseBodyDeserializerInterface) || targetType.ImplementsInterface(responseBOdyDeserializerGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				FullyQualifiedNameWithGlobal: targetType.ToDisplayString(TypeInfo.FullyQualifiedFormat),
				ConfigureAwait: configureAwait,
				TypeArguments: null,
				Diagnostics: null
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				FullyQualifiedNameWithGlobal: targetType.ToDisplayString(TypeInfo.FullyQualifiedFormat),
				ConfigureAwait: configureAwait,
				TypeArguments: null,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule6, new(responseBodyDeserializer.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (string Path, string HttpMethod, List<DiagnosticInfo>? Diagnostics) GetPathAndHttpMethod(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes, IMethodSymbol methodSymbol) {
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
			return ("", "", errors);
		} else if (!(methodSymbol.IsPartialDefinition || methodSymbol.ContainingType.TypeKind == TypeKind.Interface) && httpMethodAttrs.Length == 1) {
			var errLocation = httpMethodAttrs[0].ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? methodSymbol.Locations.FirstOrDefault() ?? Location.None;
			return ("", "", [new(HttpMethodRule1, new(errLocation), null)]);
			// 报错
		} else if ((methodSymbol.IsPartialDefinition || methodSymbol.ContainingType.TypeKind == TypeKind.Interface) && httpMethodAttrs.Length == 0) {
			return ("", "", [new(HttpMethodRule2, new(methodSymbol.Locations.FirstOrDefault() ?? Location.None), null)]);
		} else {
			var path = (string?)httpMethodAttrs[0]?.ConstructorArguments[0].Value ?? "";
			var method = HttpMethodAttrMap[httpMethodAttrs[0].AttributeClass!.ToDisplayString()];
			return (path, method, null);
		}
	}
}