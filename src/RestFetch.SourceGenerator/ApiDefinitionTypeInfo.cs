using System.Collections.Immutable;
using System.Runtime.CompilerServices;


using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public sealed record ApiDefinitionTypeInfo(
	TypeInfo TypeInfo,
	bool IsValid,
	string BaseAddress,
	string IdGenerator,
	string TResponse,
	// string THttpClient,
	string THttpClientProxy,
	string TRequestBodyContent,
	string? ImplIRestFetchApiFullName,
	RetryOptions? RetryOptions,
	TimeoutOptions? TimeoutOptions,
	HookInfo? BeforeSendHook,
	HookInfo? AfterResponseHook,
	HookInfo? OnError,
	HookInfo? OnSseEventError,
	HookInfo? OnSuccess,
	HookInfo ExceptionFactory,
	HookInfo HttpClientProxy,
	EquatableArray<KeyValuePairInfo> StaticHeaders,
	EquatableArray<ApiDefinitionMethodInfo> Methods,
	EquatableArray<DiagnosticInfo>? Diagnostics
) {
	private const string TResponseName = "global::System.Net.Http.HttpResponseMessage";
	private const string THttpClientName = "global::System.Net.Http.HttpClient";
	private const string THttpClientProxyName = "global::RestFetch.HttpClientProxy";
	private const string TRequestBodyContentName = "global::System.Net.Http.HttpResponseMessage";

	private static readonly DiagnosticDescriptor HeaderRule0 = new("RestFetch0010",
		"Invalid header attribute constructor",
		@"Static headers must use the two-argument constructor, e.g. [Header(""key"", ""value"")].",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule0 = new("RestFetch0020",
		"Invalid BeforeSend hook type",
		"The type argument for BeforeSend<T> must implement IBeforeSendHook or IBeforeSendHook<TRequestBodyContent>.",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule1 = new("RestFetch0021",
		"Invalid AfterResponse hook type",
		"The type argument for AfterResponse<T> must implement IAfterResponseHook or IAfterResponseHook<TResponse>.",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule2 = new("RestFetch0022",
		"Invalid exception factory type",
		"The type argument for ExceptionFactory<T> must implement IExceptionFactory or IExceptionFactory<TResponse>.",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule3 = new("RestFetch0023",
		"Invalid HttpClientProxy type",
		"The type argument for HttpClientProxy<THttpClientProxy> must implement IHttpClientProxy<TSelf, TResponse>.",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule4 = new("RestFetch0024",
		"Invalid OnError hook type",
		"The type argument for OnError<T> must implement IOnError.",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule5 = new("RestFetch0025",
		"Invalid OnSuccess hook type",
		"The type argument for OnSuccess<T> must implement IOnSuccess.",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	private static readonly DiagnosticDescriptor HookRule7 = new("RestFetch0027",
		"Invalid OnSseEventError hook type",
		"The type argument for OnSseEventError<T> must implement IOnSseEventError.",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	public static readonly DiagnosticDescriptor HookRule8 = new("RestFetch0028",
		"Invalid retry predicate type",
		"{0} must implement IRetryPredicate or IRetryPredicate<TResponse>.",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	public static readonly DiagnosticDescriptor HookRule9 = new("RestFetch0029",
		"Invalid OnRetry hook type",
		"{0} must implement IOnRetry or IOnRetry<TResponse>.",
		"SourceGenerator", DiagnosticSeverity.Error, true);

	public static ApiDefinitionTypeInfo CreateFromSymbol(ITypeSymbol symbol, KnownTypes knownTypes) {
		var typeInfo = TypeInfo.CreateFromSymbol(symbol, knownTypes, 1);
		var attrList = symbol.GetAttributes();
		var baseAddress = GetBaseAddress(attrList, knownTypes) ?? "";
		var staticHeaders = GetStaticHeaders(attrList, knownTypes).ToArray();
		var validStaticHeaders = staticHeaders
			.Where(static header => header.Diagnostics is null)
			.Distinct(HeaderEqualityComparer.Default)
			.GroupBy(static header => header.Key, StringComparer.OrdinalIgnoreCase)
			.Where(static headers => !headers.Any(static h => h.Value is null))
			.SelectMany(static headers => headers);


			// 去重相同的header,但是允许方法级的覆盖类级
			// .Distinct(HeaderEqualityComparer.Default);
		var headerErrors = staticHeaders
			.Where(static header => header.Diagnostics is not null)
			.SelectMany(static header => header.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArray())
			.ToArray();
		var idGenerator = GetIdGenerator(attrList, knownTypes) ?? $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.RequestIdGenerator";
		var beforeSendHook = GetBeforeSendHook(attrList, knownTypes);
		var afterResponseHook = GetAfterResponseHook(attrList, knownTypes);
		var exceptionFactory = GetExceptionFactory(attrList, knownTypes) ?? new(
			FullyQualifiedName: $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.DefaultExceptionFactory",
			FullyQualifiedNameWithGlobal: $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.DefaultExceptionFactory",
			ConfigureAwait: false,
			TypeArguments: null,
			Diagnostics: null
		);
		var httpClientProxy = GetHttpClientProxy(attrList, knownTypes) ?? new(
			FullyQualifiedName: $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.HttpClientProxy",
			FullyQualifiedNameWithGlobal: $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.HttpClientProxy",
			ConfigureAwait: false,
			TypeArguments: new string[] {
				// THttpClientName,
				THttpClientProxyName,
				TResponseName,
			}.ToImmutableArray(),
			Diagnostics: null
		);
		var onError = GetOnError(attrList, knownTypes);
		var onSuccess = GetOnSuccess(attrList, knownTypes);
		var onSseEventError = GetOnSseEventError(attrList, knownTypes);

		var retryOptions = GetRetryOptions(attrList, knownTypes);
		var timeoutOptions = GetTimeoutOptions(attrList, knownTypes);

		var tempApiDefinition = new ApiDefinitionTypeInfo(
			ImplIRestFetchApiFullName: GetIRestFetchApiInterfaceFullName(symbol, knownTypes),
			RetryOptions: retryOptions,
			TimeoutOptions: timeoutOptions,
			TResponse: httpClientProxy.TypeArguments?[1] ?? TResponseName,
			// THttpClient: httpClientProxy.TypeArguments?[0] ?? THttpClientName,
			THttpClientProxy: httpClientProxy.TypeArguments?[0] ?? THttpClientProxyName,
			TRequestBodyContent: beforeSendHook?.TypeArguments?[0] ?? TRequestBodyContentName,
			IsValid: false,
			TypeInfo: typeInfo,
			IdGenerator: idGenerator,
			BaseAddress: baseAddress,
			StaticHeaders: validStaticHeaders.ToImmutableArray(),
			BeforeSendHook: beforeSendHook,
			AfterResponseHook: afterResponseHook,
			ExceptionFactory: exceptionFactory,
			HttpClientProxy: httpClientProxy,
			OnError: onError,
			OnSseEventError: onSseEventError,
			OnSuccess: onSuccess,
			Methods: [],
			Diagnostics: ImmutableArrayExtension.CombineSafe(
				headerErrors,
				beforeSendHook?.Diagnostics,
				afterResponseHook?.Diagnostics,
				exceptionFactory.Diagnostics,
				httpClientProxy.Diagnostics,
				onError?.Diagnostics,
				onSseEventError?.Diagnostics,
				onSuccess?.Diagnostics,
				retryOptions?.Diagnostics
			)
		);
		var methods = symbol.GetInstanceMethods().Select(item => ApiDefinitionMethodInfo.CreateFromSymbol(item, knownTypes, tempApiDefinition)).ToArray();
		return tempApiDefinition with {
			IsValid = methods.All(static m => m.IsValid),
			Methods = methods.ToImmutableArray(),
			Diagnostics = tempApiDefinition.Diagnostics
				.Concat(methods
				.SelectMany(static m => m.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArray()))
				.ToImmutableArray()
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetIRestFetchApiInterfaceFullName(ITypeSymbol symbol, KnownTypes knownTypes) {
		var iRestFetchT = symbol.GetInterface(knownTypes.IRestFetchApiT);
		var iRestFetch = symbol.GetInterface(knownTypes.IRestFetchApi);
		if (iRestFetch is not null) {
			return iRestFetch.ToDisplayString(TypeInfo.FullyQualifiedFormat);
		} else if (iRestFetchT is not null) {
			return iRestFetchT.ToDisplayString(TypeInfo.FullyQualifiedFormat);
		} else {
			return null;
		}
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

	public static RetryOptions? GetRetryOptions(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var retryAttrs = attrs.GetAttributesBySymbol(knownTypes.RetryAttribute).ToArray();

		if (retryAttrs is [var retryAttr, ..]) {
			int? maxRetryAttempts = default;
			DelayBackoffType? backoffType = default;
			int? delay = default;
			int? maxDelay = default;
			bool? useJitter = default;
			bool ignore = false;
			bool allowUnsafeHttpMethod = false;
			bool allowUnsafeRequestBody = false;
			string retryPredicateFullyQualifiedNameWithGlobal = "global::RestFetch.DefaultRetryPredicate";
			string onRetryFullyQualifiedNameWithGlobal = "global::RestFetch.DefaultOnRetry";

			if (retryAttr.ConstructorArguments.Length == 0 && retryAttr.NamedArguments.Length == 0) {
				return null;
			}

			if (retryAttr.ConstructorArguments is [var retryAttempts, ..]) {
				maxRetryAttempts = (int?)retryAttempts.Value;
			}

			foreach (var item in retryAttr.NamedArguments) {
				if (item.Key == "MaxRetryAttempts") {
					maxRetryAttempts = (int?)item.Value.Value;
				} else if (item.Key == "AllowUnsafeHttpMethod") {
					allowUnsafeHttpMethod = (bool)(item.Value.Value ?? false);
				} else if (item.Key == "AllowUnsafeRequestBody") {
					allowUnsafeRequestBody = (bool)(item.Value.Value ?? false);
				} else if (item.Key == "BackoffType") {
					backoffType = (DelayBackoffType?)item.Value.Value;
				} else if (item.Key == "Delay") {
					delay = (int?)item.Value.Value;
				} else if (item.Key == "MaxDelay") {
					maxDelay = (int?)item.Value.Value;
				} else if (item.Key == "UseJitter") {
					useJitter = (bool?)item.Value.Value;
				} else if (item.Key == "Ignore") {
					ignore = (bool)item.Value.Value!;
				} else if (item.Key == "Predicate") {
					if (item.Value.Value is INamedTypeSymbol predicateType && predicateType.ImplementsInterface(knownTypes.RetryPredicate)) {
						retryPredicateFullyQualifiedNameWithGlobal = predicateType.ToDisplayString(TypeInfo.FullyQualifiedFormat);
					} else {
						return new(
							Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule8, new(retryAttr.GetNamedArgumentLocation("Predicate")),
								(ImmutableArray<string>)[item.Value.Value is ITypeSymbol contextType ? contextType.Name : "Predicate"])]
						);
					}
				} else if (item.Key == "OnRetry") {
					if (item.Value.Value is INamedTypeSymbol onRetryType && onRetryType.ImplementsInterface(knownTypes.OnRetry)) {
						onRetryFullyQualifiedNameWithGlobal = onRetryType.ToDisplayString(TypeInfo.FullyQualifiedFormat);
					} else {
						return new(
							Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule9, new(retryAttr.GetNamedArgumentLocation("OnRetry")),
								(ImmutableArray<string>)[item.Value.Value is ITypeSymbol contextType ? contextType.Name : "OnRetry"])]
						);
					}
				}
			}

			return new(
				MaxRetryAttempts: maxRetryAttempts,
				AllowUnsafeHttpMethod: allowUnsafeHttpMethod,
				AllowUnsafeRequestBody: allowUnsafeRequestBody,
				BackoffType: backoffType,
				Delay: delay,
				MaxDelay: maxDelay,
				UseJitter: useJitter,
				Ignore: ignore,
				Predicate: retryPredicateFullyQualifiedNameWithGlobal,
				OnRetry: onRetryFullyQualifiedNameWithGlobal
			);
		} else {
			return null;
		}
	}

	public static TimeoutOptions? GetTimeoutOptions(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var timeoutAttrs = attrs.GetAttributesBySymbol(knownTypes.TimeoutAttribute).ToArray();

		if (timeoutAttrs is [var timeoutAttr, ..]) {
			int? attemptTimeout = default;
			int? totalTimeout = default;
			bool ignore = false;

			if (timeoutAttr.ConstructorArguments.Length == 0 && timeoutAttr.NamedArguments.Length == 0) {
				return null;
			}

			if (timeoutAttr.ConstructorArguments is [var total, ..]) {
				totalTimeout = (int?)total.Value;
			}

			foreach (var item in timeoutAttr.NamedArguments) {
				if (item.Key == "AttemptTimeout") {
					attemptTimeout = (int?)item.Value.Value;
				} else if (item.Key == "TotalTimeout") {
					totalTimeout = (int?)item.Value.Value;
				} else if (item.Key == "Ignore") {
					ignore = (bool)item.Value.Value!;
				}
			}

			return new(
				AttemptTimeout: attemptTimeout,
				TotalTimeout: totalTimeout,
				Ignore: ignore
			);
		} else {
			return null;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetIdGenerator(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var idAttr = attrs.GetAttributesBySymbol(knownTypes.IdGeneratorAttribute)
			.FirstOrDefault();

		return idAttr?.AttributeClass?.TypeArguments.IsEmpty ?? true ? null : idAttr?.AttributeClass?.TypeArguments[0].ToDisplayString(TypeInfo.FullyQualifiedFormat);
	}

	public static HookInfo? GetBeforeSendHook(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var beforeSendHookAttr = attrs.GetAttributesBySymbol(knownTypes.BeforeSendAttribute).FirstOrDefault();
		if (beforeSendHookAttr is null || beforeSendHookAttr.AttributeClass is null) {
			return null;
		}
		var beforeSendHookInterface = knownTypes.BeforeSendHookInterface;
		var beforeSendHookGenericInterface = knownTypes.BeforeSendHookGenericInterface;
		var targetType = beforeSendHookAttr.AttributeClass.TypeArguments[0];
		var configureAwait = !beforeSendHookAttr.NamedArguments.IsEmpty && (bool)beforeSendHookAttr.NamedArguments[0].Value.Value!;

		if (targetType.ImplementsInterface(beforeSendHookInterface) || targetType.ImplementsInterface(beforeSendHookGenericInterface)) {
			var beforeSendHookGenericInterfaceImpl = targetType.GetInterface(beforeSendHookGenericInterface);
			var typeArguments = beforeSendHookGenericInterfaceImpl!.TypeArguments
				.Select(static t => t.ToDisplayString(RestFetch.SourceGenerator.TypeInfo.FullyQualifiedFormat))
				.ToArray();
			return new(
				FullyQualifiedName: targetType.ToDisplayString(),
				FullyQualifiedNameWithGlobal: targetType.ToDisplayString(TypeInfo.FullyQualifiedFormat),
				ConfigureAwait: configureAwait,
				TypeArguments: typeArguments.ToImmutableArray(),
				Diagnostics: null
			);
		} else {
			return new(
				FullyQualifiedName: targetType.ToDisplayString(),
				FullyQualifiedNameWithGlobal: targetType.ToDisplayString(TypeInfo.FullyQualifiedFormat),
				ConfigureAwait: configureAwait,
				TypeArguments: null,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule0, new(beforeSendHookAttr.GetTypeArgumentLocation()), null)]
			);
		}
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
		var configureAwait = !afterResponseAttr.NamedArguments.IsEmpty && (bool)afterResponseAttr.NamedArguments[0].Value.Value!;

		return targetType.ImplementsInterface(afterResponseHookInterface) || targetType.ImplementsInterface(afterResponseHookGenericInterface)
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
		var configureAwait = !exceptionFactory.NamedArguments.IsEmpty && (bool)exceptionFactory.NamedArguments[0].Value.Value!;

		return targetType.ImplementsInterface(exceptionFactoryInterface) || targetType.ImplementsInterface(exceptionFactoryGenericInterface)
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
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule2, new(exceptionFactory.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetHttpClientProxy(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var httpClientProxyAttr = attrs.GetAttributesBySymbol(knownTypes.HttpClientProxyAttribute).FirstOrDefault();
		if (httpClientProxyAttr is null || httpClientProxyAttr.AttributeClass is null) {
			return null;
		}
		var httpClientProxyGenericInterface = knownTypes.HttpClientProxyGenericInterface;
		var targetType = httpClientProxyAttr.AttributeClass.TypeArguments[0];
		var configureAwait = !httpClientProxyAttr.NamedArguments.IsEmpty && (bool)httpClientProxyAttr.NamedArguments[0].Value.Value!;

		if (targetType.ImplementsInterface(httpClientProxyGenericInterface)) {
			var httpClientProxyGenericInterfaceImpl = targetType.GetInterface(httpClientProxyGenericInterface);
			var typeArguments = httpClientProxyGenericInterfaceImpl!.TypeArguments
				.Select(static t => t.ToDisplayString(TypeInfo.FullyQualifiedFormat))
				.ToArray();
			// foreach (var type in httpClientProxyGenericInterfaceImpl!.TypeArguments) {
			// 	var t = TypeInfo.CreateFromSymbolWithoutBaseTypeAndMethods(type, knownTypes);
			// }
			return new(
				FullyQualifiedName: targetType.ToDisplayString(),
				FullyQualifiedNameWithGlobal: targetType.ToDisplayString(TypeInfo.FullyQualifiedFormat),
				ConfigureAwait: configureAwait,
				TypeArguments: typeArguments.ToImmutableArray(),
				Diagnostics: null
			);
		} else {
			return new(
				FullyQualifiedName: targetType.ToDisplayString(),
				FullyQualifiedNameWithGlobal: targetType.ToDisplayString(TypeInfo.FullyQualifiedFormat),
				ConfigureAwait: configureAwait,
				TypeArguments: null,
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule3, new(httpClientProxyAttr.GetTypeArgumentLocation()), null)]
			);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetOnError(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var onErrorAttr = attrs.GetAttributesBySymbol(knownTypes.OnErrorAttribute).FirstOrDefault();
		if (onErrorAttr is null || onErrorAttr.AttributeClass is null) {
			return null;
		}

		var onErrorInterface = knownTypes.OnErrorInterface;
		var targetType = onErrorAttr.AttributeClass.TypeArguments[0];
		var configureAwait = !onErrorAttr.NamedArguments.IsEmpty && (bool)onErrorAttr.NamedArguments[0].Value.Value!;

		return targetType.ImplementsInterface(onErrorInterface)
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
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule4, new(onErrorAttr.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetOnSseEventError(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var onSseEventErrorAttr = attrs.GetAttributesBySymbol(knownTypes.OnSseEventErrorAttribute).FirstOrDefault();
		if (onSseEventErrorAttr is null || onSseEventErrorAttr.AttributeClass is null) {
			return null;
		}

		var onSseEventErrorInterface = knownTypes.OnSseEventErrorInterface;
		var targetType = onSseEventErrorAttr.AttributeClass.TypeArguments[0];
		var configureAwait = !onSseEventErrorAttr.NamedArguments.IsEmpty && (bool)onSseEventErrorAttr.NamedArguments[0].Value.Value!;

		return targetType.ImplementsInterface(onSseEventErrorInterface)
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
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule7, new(onSseEventErrorAttr.GetTypeArgumentLocation()), null)]
			);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HookInfo? GetOnSuccess(ImmutableArray<AttributeData> attrs, KnownTypes knownTypes) {
		var onSuccessAttr = attrs.GetAttributesBySymbol(knownTypes.OnSuccessAttribute).FirstOrDefault();
		if (onSuccessAttr is null || onSuccessAttr.AttributeClass is null) {
			return null;
		}

		var onSuccessInterface = knownTypes.OnSuccessInterface;
		var targetType = onSuccessAttr.AttributeClass.TypeArguments[0];
		var configureAwait = !onSuccessAttr.NamedArguments.IsEmpty && (bool)onSuccessAttr.NamedArguments[0].Value.Value!;

		return targetType.ImplementsInterface(onSuccessInterface)
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
				Diagnostics: (ImmutableArray<DiagnosticInfo>)[new(HookRule5, new(onSuccessAttr.GetTypeArgumentLocation()), null)]
			);
	}
}