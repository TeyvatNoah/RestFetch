using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public sealed record ApiDefinitionTypeInfo(
	TypeInfo TypeInfo,
	bool IsValid,
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
	EquatableArray<DiagnosticInfo>? Diagnostics
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

	public static ApiDefinitionTypeInfo CreateFromSymbol(ITypeSymbol symbol, KnownTypes knownTypes) {
		var typeInfo = TypeInfo.CreateFromSymbol(symbol, knownTypes, 1);
		var attrList = symbol.GetAttributes();
		var baseAddress = GetBaseAddress(attrList, knownTypes) ?? "";
		var staticHeaders = GetStaticHeaders(attrList, knownTypes);
		var validStaticHeaders = staticHeaders
			.Where(static header => header.Diagnostics is null)
			.GroupBy(static header => header.Key, StringComparer.OrdinalIgnoreCase)
			.Select(static g => g.LastOrDefault());
			// 去重相同的header,但是允许方法级的覆盖类级
			// .Distinct(HeaderEqualityComparer.Default);
		var headerErrors = staticHeaders
			.Where(static header => header.Diagnostics is not null)
			.SelectMany(static header => header.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArraySafe());
		var idGenerator = GetIdGenerator(attrList, knownTypes) ?? $"{RestFetchSourceGenerator.RestFetchNamespaceName}.RequestIdGenerator";
		var beforeSendHook = GetBeforeSendHook(attrList, knownTypes);
		var afterResponseHook = GetAfterResponseHook(attrList, knownTypes);
		var exceptionFactory = GetExceptionFactory(attrList, knownTypes) ?? new(
			FullyQualifiedName: $"{RestFetchSourceGenerator.RestFetchNamespaceName}.DefaultExceptionFactory",
			ConfigAwait: false,
			Diagnostics: null
		);
		var httpClientProxy = GetHttpClientProxy(attrList, knownTypes) ?? new(
			FullyQualifiedName: $"{RestFetchSourceGenerator.RestFetchNamespaceName}.HttpClientProxy",
			ConfigAwait: false,
			Diagnostics: null
		);
		var onError = GetOnError(attrList, knownTypes);
		var onSuccess = GetOnSuccess(attrList, knownTypes);
		var tempApiDefinition = new ApiDefinitionTypeInfo(
			IsValid: false,
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
				beforeSendHook?.Diagnostics,
				afterResponseHook?.Diagnostics,
				exceptionFactory.Diagnostics,
				httpClientProxy.Diagnostics,
				onError?.Diagnostics,
				onSuccess?.Diagnostics
			)
		);
		var methods = symbol.GetInstanceMethods().Select(item => ApiDefinitionMethodInfo.CreateFromSymbol(item, knownTypes, tempApiDefinition));
		return tempApiDefinition with {
			IsValid = methods.All(static m => m.IsValid),
			Methods = methods.ToImmutableArraySafe(),
			Diagnostics = tempApiDefinition.Diagnostics
				.Concat(methods
					.SelectMany(static m => m.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArraySafe())).ToImmutableArraySafe()
		};
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetBaseAddress(ImmutableArray<AttributeData> attr, KnownTypes knownTypes)
		=> attr
			.GetAttributesBySymbol(knownTypes.BaseAddress)
			.SelectMany(static item => item.ConstructorArguments)
			.Where(Util.IsPrimitiveType<string>)
			.Select(static item => (string)item.Value!)
			.FirstOrDefault();


	public static IEnumerable<KeyValuePairInfo> GetStaticHeaders(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var headerAttrs = attrs.GetAttributesBySymbol(knownTypes.HeaderAttribute);
		foreach (var headerAttr in headerAttrs) {
			if (headerAttr.ConstructorArguments.Length == 2) {
				yield return new(
					Key: (string)headerAttr.ConstructorArguments[0].Value!,
					Value: (string)headerAttr.ConstructorArguments[1].Value!,
					Diagnostics: null
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
	public static string? GetIdGenerator(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var idAttr = attrs.GetAttributesBySymbol(knownTypes.IdGeneratorAttribute)
			.FirstOrDefault();
		return idAttr?.AttributeClass?.TypeArguments[0]?.ToDisplayString();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetBeforeSendHook(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var beforeSendHookAttr = attrs.GetAttributesBySymbol(knownTypes.BeforeSendAttribute).FirstOrDefault();
		if (beforeSendHookAttr is null || beforeSendHookAttr.AttributeClass is null) {
			return null;
		}
		var beforeSendHookInterface = knownTypes.BeforeSendHookInterface;
		var beforeSendHookGenericInterface = knownTypes.BeforeSendHookGenericInterface;
		var targetType = beforeSendHookAttr.AttributeClass.TypeArguments[0];
		var configAwait = !beforeSendHookAttr.NamedArguments.IsEmpty && (bool)beforeSendHookAttr.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(beforeSendHookInterface) || targetType.ImplementsInterface(beforeSendHookGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: null
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule0, new(beforeSendHookAttr.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetAfterResponseHook(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var afterResponseAttr = attrs.GetAttributesBySymbol(knownTypes.AfterResponseAttribute).FirstOrDefault();
		if (afterResponseAttr is null || afterResponseAttr.AttributeClass is null) {
			return null;
		}
		var afterResponseHookInterface = knownTypes.AfterResponseHookInterface;
		var afterResponseHookGenericInterface = knownTypes.AfterResponseHookGenericInterface;
		var targetType = afterResponseAttr.AttributeClass.TypeArguments[0];
		var configAwait = !afterResponseAttr.NamedArguments.IsEmpty && (bool)afterResponseAttr.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(afterResponseHookInterface) || targetType.ImplementsInterface(afterResponseHookGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: null
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule1, new(afterResponseAttr.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetExceptionFactory(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var exceptionFactory = attrs.GetAttributesBySymbol(knownTypes.ExceptionFactoryAttribute).FirstOrDefault();
		if (exceptionFactory is null || exceptionFactory.AttributeClass is null) {
			return null;
		}
		var exceptionFactoryInterface = knownTypes.ExceptionFactoryInterface;
		var exceptionFactoryGenericInterface = knownTypes.ExceptionFactoryGenericInterface;
		var targetType = exceptionFactory.AttributeClass.TypeArguments[0];
		var configAwait = !exceptionFactory.NamedArguments.IsEmpty && (bool)exceptionFactory.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(exceptionFactoryInterface) || targetType.ImplementsInterface(exceptionFactoryGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: null
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule2, new(exceptionFactory.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetHttpClientProxy(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var httpClientProxyAttr = attrs.GetAttributesBySymbol(knownTypes.HttpClientProxyAttribute).FirstOrDefault();
		if (httpClientProxyAttr is null || httpClientProxyAttr.AttributeClass is null) {
			return null;
		}
		var httpClientProxyInterface = knownTypes.HttpClientProxyInterface;
		var httpClientProxyGenericInterface = knownTypes.HttpClientProxyGenericInterface;
		var targetType = httpClientProxyAttr.AttributeClass.TypeArguments[0];
		var configAwait = !httpClientProxyAttr.NamedArguments.IsEmpty && (bool)httpClientProxyAttr.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(httpClientProxyInterface) || targetType.ImplementsInterface(httpClientProxyGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: null
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule3, new(httpClientProxyAttr.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetOnError(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var onErrorAttr = attrs.GetAttributesBySymbol(knownTypes.OnErrorAttribute).FirstOrDefault();
		if (onErrorAttr is null || onErrorAttr.AttributeClass is null) {
			return null;
		}
		var onErrorInterface = knownTypes.OnErrorInterface;
		var onErrorGenericInterface = knownTypes.OnSuccessGenericInterface;
		var targetType = onErrorAttr.AttributeClass.TypeArguments[0];
		var configAwait = !onErrorAttr.NamedArguments.IsEmpty && (bool)onErrorAttr.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(onErrorInterface) || targetType.ImplementsInterface(onErrorGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: null
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule4, new(onErrorAttr.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetOnSuccess(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var onSuccessAttr = attrs.GetAttributesBySymbol(knownTypes.OnSuccessAttribute).FirstOrDefault();
		if (onSuccessAttr is null || onSuccessAttr.AttributeClass is null) {
			return null;
		}
		var onSuccessInterface = knownTypes.OnSuccessInterface;
		var onSuccessGenericInterface = knownTypes.OnSuccessGenericInterface;
		var targetType = onSuccessAttr.AttributeClass.TypeArguments[0];
		var configAwait = !onSuccessAttr.NamedArguments.IsEmpty && (bool)onSuccessAttr.NamedArguments[0].Value.Value!;
		return targetType.ImplementsInterface(onSuccessInterface) || targetType.ImplementsInterface(onSuccessGenericInterface)
			? new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: null
			)
			: new(
				FullyQualifiedName: targetType.ToDisplayString(),
				ConfigAwait: configAwait,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule5, new(onSuccessAttr.GetTypeArgumentLocation()), null)]
			);
	}
}