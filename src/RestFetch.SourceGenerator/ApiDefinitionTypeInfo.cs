using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public sealed record ApiDefinitionTypeInfo(
	TypeInfo TypeInfo,
	string BaseAddress,
	string IdGenerator,
	HookInfo? BeforeSendHook,
	HookInfo? AfterResponseHook,
	HookInfo? OnError,
	HookInfo? OnSuccess,
	HookInfo ExceptionFactory,
	HookInfo HttpClientProxy,
	EquatableArray<KeyValuePairInfo> StaticHeaders,
	EquatableArray<ApiDefinitionMethodInfo> Methods,
	EquatableArray<DiagnosticInfo> Diagnostics
) {

	private static readonly DiagnosticDescriptor HeaderRule0 = new("RestFetch0010",
		"错误的Header构造函数",
		@"静态Header必须使用双参数构造函数.eg.[Header(""key"", ""value"")]",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule0 = new("RestFetch0020",
		"错误的BeforeSendHook类型",
		"BeforeSend<THook>中THook必须实现IBeforeSendHook或IBeforeSendHook<TURL, TDisposer, TRequestBodyContent, TResponse, TProxy>",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule1 = new("RestFetch0021",
		"错误的AfterResponseHook类型",
		"AfterResponse<THook>中THook必须实现IAfterResponseHook或IAfterResponseHook<TURL, TDisposer, TResponse, TProxy>",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule2 = new("RestFetch0022",
		"错误的ExceptionFactory类型",
		"ExceptionFactory<THook>中THook必须实现IExceptionFactory或IExceptionFactory<TURL, TDisposer, TResponse, TProxy>",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule3 = new("RestFetch0023",
		"错误的HttpClientProxy类型",
		"HttpClientProxy<THook>中THook必须实现IHttpClientProxy或IHttpClientProxy<TURL, TDisposer, TResponse, TProxy>",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule4 = new("RestFetch0024",
		"错误的OnError类型",
		"OnError<THook>中THook必须实现IOnError或IOnError<TURL, TDisposer, TResponse, TProxy>",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule5 = new("RestFetch0025",
		"错误的OnSuccess类型",
		"OnSuccess<THook>中THook必须实现IOnSuccess或IOnSuccess<TURL, TDisposer, TResponse, TProxy>",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	public static ApiDefinitionTypeInfo CreateFromSymbol(ITypeSymbol symbol, Compilation compilation) {
		var typeInfo = TypeInfo.CreateFromSymbol(symbol);
		var attrList = symbol.GetAttributes();
		var baseAddress = GetBaseAddress(attrList, compilation) ?? "";
		var staticHeaders = GetStaticHeaders(attrList, compilation);
		var validStaticHeaders = staticHeaders
			.Where(static header => header.Diagnostics.Length == 0)
			.GroupBy(static header => header.Key, StringComparer.OrdinalIgnoreCase)
			.Select(static g => g.LastOrDefault());
			// 去重相同的header,但是允许方法级的覆盖类级
			// .Distinct(HeaderEqualityComparer.Default);
		var headerErrors = staticHeaders
			.Where(static header => header.Diagnostics.Length != 0)
			.SelectMany(static header => header.Diagnostics);
		var idGenerator = GetIdGenerator(attrList, compilation) ?? "RestFetch.RequestIdGenerator";
		var beforeSendHook = GetBeforeSendHook(attrList, compilation);
		var afterResponseHook = GetAfterResponseHook(attrList, compilation);
		var exceptionFactory = GetExceptionFactory(attrList, compilation) ?? new(
			FullyQualifiedName: "RestFetch.DefaultExceptionFactory",
			ConfigAwait: false,
			Diagnostics: []
		);
		var httpClientProxy = GetHttpClientProxy(attrList, compilation) ?? new(
			FullyQualifiedName: "RestFetch.HttpClientProxy",
			ConfigAwait: false,
			Diagnostics: []
		);
		var onError = GetOnError(attrList, compilation);
		var onSuccess = GetOnSuccess(attrList, compilation);
		var tempApiDefinition = new ApiDefinitionTypeInfo(
			TypeInfo: typeInfo,
			IdGenerator: idGenerator,
			BaseAddress: baseAddress,
			StaticHeaders: validStaticHeaders.ToImmutableArraySafe(),
			BeforeSendHook: beforeSendHook,
			AfterResponseHook: afterResponseHook,
			ExceptionFactory: exceptionFactory,
			HttpClientProxy: httpClientProxy,
			OnError: onError,
			OnSuccess: onSuccess,
			Methods: [],
			Diagnostics: ImmutableArrayExtension.CombineSafe(
				headerErrors,
				beforeSendHook?.Diagnostics.AsImmutableArray(),
				afterResponseHook?.Diagnostics.AsImmutableArray(),
				exceptionFactory.Diagnostics.AsImmutableArray(),
				httpClientProxy.Diagnostics.AsImmutableArray(),
				onError?.Diagnostics.AsImmutableArray(),
				onSuccess?.Diagnostics.AsImmutableArray()
			)
		);
		var methods = symbol.GetInstanceMethods().Select(item => ApiDefinitionMethodInfo.CreateFromSymbol(item, compilation, tempApiDefinition));
		return tempApiDefinition with {
			Methods = methods.ToImmutableArraySafe(),
			Diagnostics = tempApiDefinition.Diagnostics.Concat(methods.SelectMany(static m => m.Diagnostics)).ToImmutableArraySafe()
		};
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetBaseAddress(ImmutableArray<AttributeData> attr, Compilation compilation)
		=> attr
			.GetAttributesByFullName("RestFetch.BaseAddressAttribute", compilation)
			.SelectMany(static item => item.ConstructorArguments)
			.Where(Util.IsPrimitiveType<string>)
			.Select(static item => (string)item.Value!)
			.FirstOrDefault();


	public static IEnumerable<KeyValuePairInfo> GetStaticHeaders(ImmutableArray<AttributeData> attrs, Compilation compilation) {
		var headerAttrs = attrs.GetAttributesByFullName("RestFetch.HeaderAttribute", compilation);
		foreach (var headerAttr in headerAttrs) {
			if (headerAttr.ConstructorArguments.Length == 2) {
				yield return new(
					Key: (string)headerAttr.ConstructorArguments[0].Value!,
					Value: (string)headerAttr.ConstructorArguments[1].Value!,
					Diagnostics: []
				);
			} else {
				var errLocation = headerAttr.ApplicationSyntaxReference?.GetSyntax().GetLocation();
				yield return new(
					Key: "",
					Value: null,
					Diagnostics: errLocation is null ? [] : (ImmutableArray<DiagnosticInfo>)[new(HeaderRule0, new(errLocation), null)]
				);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetIdGenerator(ImmutableArray<AttributeData> attrs, Compilation compilation) {
		var idAttr = attrs.GetAttributesByFullName("RestFetch.IdGeneratorAttribute`1", compilation)
			.FirstOrDefault();
		return idAttr?.AttributeClass?.TypeArguments[0]?.ToDisplayString();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetBeforeSendHook(ImmutableArray<AttributeData> attrs, Compilation compilation) {
		var beforeSendHookAttr = attrs.GetAttributesByFullName("RestFetch.BeforeSendAttribute`1", compilation).FirstOrDefault();
		if (beforeSendHookAttr is null || beforeSendHookAttr.AttributeClass is null) {
			return null;
		}
		var beforeSendHookInterface = compilation.GetTypeByMetadataName("RestFetch.IBeforeSendHook")!;
		var beforeSendHookGenericInterface = compilation.GetTypeByMetadataName("RestFetch.IBeforeSendHook`5")!;
		var targetType = beforeSendHookAttr.AttributeClass.TypeArguments[0];
		var configAwait = beforeSendHookAttr.NamedArguments.Length > 0 && (bool)beforeSendHookAttr.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(beforeSendHookInterface) || targetType.ImplementsInterface(beforeSendHookGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: []
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule0, new(beforeSendHookAttr.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetAfterResponseHook(ImmutableArray<AttributeData> attrs, Compilation compilation) {
		var afterResponseAttr = attrs.GetAttributesByFullName("RestFetch.AfterResponseAttribute`1", compilation).FirstOrDefault();
		if (afterResponseAttr is null || afterResponseAttr.AttributeClass is null) {
			return null;
		}
		var afterResponseHookInterface = compilation.GetTypeByMetadataName("RestFetch.IAfterResponseHook")!;
		var afterResponseHookGenericInterface = compilation.GetTypeByMetadataName("RestFetch.IAfterResponseHook`4")!;
		var targetType = afterResponseAttr.AttributeClass.TypeArguments[0];
		var configAwait = afterResponseAttr.NamedArguments.Length > 0 && (bool)afterResponseAttr.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(afterResponseHookInterface) || targetType.ImplementsInterface(afterResponseHookGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: []
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule1, new(afterResponseAttr.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetExceptionFactory(ImmutableArray<AttributeData> attrs, Compilation compilation) {
		var exceptionFactory = attrs.GetAttributesByFullName("RestFetch.ExceptionFactoryAttribute`1", compilation).FirstOrDefault();
		if (exceptionFactory is null || exceptionFactory.AttributeClass is null) {
			return null;
		}
		var exceptionFactoryInterface = compilation.GetTypeByMetadataName("RestFetch.IExceptionFactory")!;
		var exceptionFactoryGenericInterface = compilation.GetTypeByMetadataName("RestFetch.IExceptionFactory`4")!;
		var targetType = exceptionFactory.AttributeClass.TypeArguments[0];
		var configAwait = exceptionFactory.NamedArguments.Length > 0 && (bool)exceptionFactory.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(exceptionFactoryInterface) || targetType.ImplementsInterface(exceptionFactoryGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: []
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule2, new(exceptionFactory.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetHttpClientProxy(ImmutableArray<AttributeData> attrs, Compilation compilation) {
		var httpClientProxyAttr = attrs.GetAttributesByFullName("RestFetch.HttpClientProxyAttribute`1", compilation).FirstOrDefault();
		if (httpClientProxyAttr is null || httpClientProxyAttr.AttributeClass is null) {
			return null;
		}
		var httpClientProxyInterface = compilation.GetTypeByMetadataName("RestFetch.IHttpClientProxy")!;
		var httpClientProxyGenericInterface = compilation.GetTypeByMetadataName("RestFetch.IHttpClientProxy`4")!;
		var targetType = httpClientProxyAttr.AttributeClass.TypeArguments[0];
		var configAwait = httpClientProxyAttr.NamedArguments.Length > 0 && (bool)httpClientProxyAttr.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(httpClientProxyInterface) || targetType.ImplementsInterface(httpClientProxyGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: []
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule3, new(httpClientProxyAttr.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetOnError(ImmutableArray<AttributeData> attrs, Compilation compilation) {
		var onErrorAttr = attrs.GetAttributesByFullName("RestFetch.OnErrorAttribute`1", compilation).FirstOrDefault();
		if (onErrorAttr is null || onErrorAttr.AttributeClass is null) {
			return null;
		}
		var onErrorInterface = compilation.GetTypeByMetadataName("RestFetch.IOnError")!;
		var onErrorGenericInterface = compilation.GetTypeByMetadataName("RestFetch.IOnError`4")!;
		var targetType = onErrorAttr.AttributeClass.TypeArguments[0];
		var configAwait = onErrorAttr.NamedArguments.Length > 0 && (bool)onErrorAttr.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(onErrorInterface) || targetType.ImplementsInterface(onErrorGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: []
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule4, new(onErrorAttr.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetOnSuccess(ImmutableArray<AttributeData> attrs, Compilation compilation) {
		var onSuccessAttr = attrs.GetAttributesByFullName("RestFetch.OnSuccessAttribute`1", compilation).FirstOrDefault();
		if (onSuccessAttr is null || onSuccessAttr.AttributeClass is null) {
			return null;
		}
		var onSuccessInterface = compilation.GetTypeByMetadataName("RestFetch.IOnSuccess")!;
		var onSuccessGenericInterface = compilation.GetTypeByMetadataName("RestFetch.IOnSuccess`4")!;
		var targetType = onSuccessAttr.AttributeClass.TypeArguments[0];
		var configAwait = onSuccessAttr.NamedArguments.Length > 0 && (bool)onSuccessAttr.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(onSuccessInterface) || targetType.ImplementsInterface(onSuccessGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: []
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule5, new(onSuccessAttr.GetTypeArgumentLocation()), null)]
			);
	}
}