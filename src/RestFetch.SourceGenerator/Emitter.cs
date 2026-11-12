using System.Globalization;
using System.Runtime.CompilerServices;

using Cysharp.Text;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public static class Emitter {
	private const string GeneratedClassNamePrefix = "RestFetchGen";
	public static string GenerateSource(ApiDefinitionTypeInfo apiDefinition) {
		using var sourceBuilder = new SourceBuilder();
		sourceBuilder.CreateFile(sb => {
			static void GenerateType(SourceBuilder sb, ApiDefinitionTypeInfo apiDefinition, bool withContainingType, string typeNameFullyQualifiedNameWithGlobal, string? typeName = null) {
				sb.CreateType(RestFetchSourceGenerator.RestFetchNamespaceName, new("1.0.0"),
					apiDefinition.TypeInfo with {
						IsSealed = true
					},
					sb => sb.CreateIndent(sb => {

					// sb.Append($$"""
					// 		struct ClientWrapper(global::{{RestFetchSourceGenerator.RestFetchNamespaceName}}.IHttpClientProxy<{{apiDefinition.THttpClient}}, {{apiDefinition.TResponse}}> client) {
					// 			public readonly global::{{RestFetchSourceGenerator.RestFetchNamespaceName}}.IHttpClientProxy<{{apiDefinition.THttpClient}}, {{apiDefinition.TResponse}}> Client = client;
					// 		}

					// """);

					sb.AppendLine($@"public static readonly string CompileTimeBaseAddress = ""{apiDefinition.BaseAddress}"";")
					.Append('\n');
					sb.AppendLine(@"public static readonly global::Polly.ResiliencePropertyKey<global::RestFetch.ApiContext> ApiContextKey = new(""RestFetch.ApiContext"");")
					.Append('\n');
					sb.AppendLine($"private readonly global::System.Collections.ObjectModel.ReadOnlySet<string> _methodNames = [{string.Join(", ", apiDefinition.Methods.Select(static m => $@"""{m.MethodInfo.Name}"""))}];")
					.Append('\n');
					sb.AppendLine($@"public readonly global::{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxyService<{apiDefinition.TResponse}> Client;")
					.Append('\n');
					sb.AppendLine("private readonly bool _ownedClient = true;")
					.Append('\n');
					sb.AppendLine("private global::System.Threading.Lock _lock = new();")
					.Append('\n');
					sb.AppendLine("private bool _isBuilt = false;")
					.Append('\n');
					sb.AppendLine($"private object? _retryOptions = {(apiDefinition.RetryOptions is null ? "null"
					: $"""
					new global::RestFetch.RetryOptions<{apiDefinition.TResponse}>(
						Ignore: {(apiDefinition.RetryOptions.Ignore ? "true" : "false")},
						AllowUnsafeHttpMethod: {(apiDefinition.RetryOptions.AllowUnsafeHttpMethod ? "true" : "false")},
						MaxRetryAttempts: {(apiDefinition.RetryOptions.MaxRetryAttempts is null ? "null" : apiDefinition.RetryOptions.MaxRetryAttempts)},
						BackoffType: {apiDefinition.RetryOptions.BackoffType switch {
							DelayBackoffType.Constant => "global::Polly.DelayBackoffType.Constant",
							DelayBackoffType.Exponential => "global::Polly.DelayBackoffType.Exponential",
							DelayBackoffType.Linear => "global::Polly.DelayBackoffType.Linear",
							null => "global::Polly.DelayBackoffType.Constant",
							_ => "global::Polly.DelayBackoffType.Constant"
						}},
						Delay: {(apiDefinition.RetryOptions.Delay is null ? "null" : apiDefinition.RetryOptions.Delay)},
						MaxDelay: {(apiDefinition.RetryOptions.MaxDelay is null ? "null" : apiDefinition.RetryOptions.MaxDelay)},
						UseJitter: {(apiDefinition.RetryOptions.UseJitter is {} useJitter ? useJitter ? "true" : "false" : "null")}
					)
					""")};")
					.Append('\n');
					sb.AppendLine($"private object? _timeoutOptions = {(apiDefinition.TimeoutOptions is null ? "null"
					: $"""
					new global::RestFetch.TimeoutOptions(
						Ignore: {(apiDefinition.TimeoutOptions.Ignore ? "true" : "false")},
						AttemptTimeout: {(apiDefinition.TimeoutOptions.AttemptTimeout is null ? "null" : apiDefinition.TimeoutOptions.AttemptTimeout)},
						TotalTimeout: {(apiDefinition.TimeoutOptions.TotalTimeout is null ? "null" : apiDefinition.TimeoutOptions.TotalTimeout)}
					)
					""")};")
					.Append('\n');
					sb.AppendLine($"private global::Polly.ResiliencePipeline<{apiDefinition.TResponse}>? _pipeline = default;")
					.Append('\n');
					sb.AppendLine($"private global::System.Collections.Generic.Dictionary<string, object?>? _methodRetryOptionsDraft = {(apiDefinition.Methods.All(static m => m.RetryOptions is null)
					? "null"
					: $$"""
					new() {
						{{string.Join(",",
						apiDefinition.Methods.Select(m => $$"""
						{ "{{m.MethodInfo.Name}}", {{(m.RetryOptions is null ? "null" : $"""
						new global::RestFetch.RetryOptions<{apiDefinition.TResponse}>(
							Ignore: {(m.RetryOptions.Ignore ? "true" : "false")},
							AllowUnsafeHttpMethod: {(m.RetryOptions.AllowUnsafeHttpMethod ? "true" : "false")},
							MaxRetryAttempts: {(m.RetryOptions.MaxRetryAttempts is null ? "null" : m.RetryOptions.MaxRetryAttempts)},
							BackoffType: {m.RetryOptions.BackoffType switch {
								DelayBackoffType.Constant => "global::Polly.DelayBackoffType.Constant",
								DelayBackoffType.Exponential => "global::Polly.DelayBackoffType.Exponential",
								DelayBackoffType.Linear => "global::Polly.DelayBackoffType.Linear",
								null => "global::Polly.DelayBackoffType.Constant",
								_ => "global::Polly.DelayBackoffType.Constant"
							}},
							Delay: {(m.RetryOptions.Delay is null ? "null" : m.RetryOptions.Delay)},
							MaxDelay: {(m.RetryOptions.MaxDelay is null ? "null" : m.RetryOptions.MaxDelay)},
							UseJitter: {(m.RetryOptions.UseJitter is {} useJitter ? useJitter ? "true" : "false" : "null")}
						)
						"""
						)}} }
						""").ToArray())}}
					}
					""")};")
					.Append('\n');
					sb.AppendLine($"private global::System.Collections.Generic.Dictionary<string, object?>? _methodTimeoutOptionsDraft = {(apiDefinition.Methods.All(static m => m.TimeoutOptions is null)
					? "null"
					: $$"""
					new() {
						{{string.Join(",",
						apiDefinition.Methods.Select(static m => $$"""
						{ "{{m.MethodInfo.Name}}", {{(m.TimeoutOptions is null ? "null" : $"""
						new global::RestFetch.TimeoutOptions(
							Ignore: {(m.TimeoutOptions.Ignore ? "true" : "false")},
							AttemptTimeout: {(m.TimeoutOptions.AttemptTimeout is null ? "null" : m.TimeoutOptions.AttemptTimeout)},
							TotalTimeout: {(m.TimeoutOptions.TotalTimeout is null ? "null" : m.TimeoutOptions.TotalTimeout)}
						)
						"""
						)}} }
						"""))}}
					}
					""")};")
					.Append('\n');
					sb.AppendLine($"private global::System.Collections.Generic.Dictionary<string, global::Polly.ResiliencePipeline<{apiDefinition.TResponse}>> _methodPipelines = new({(
						apiDefinition.Methods.Length > 0
						? apiDefinition.Methods.Length
						: 4)});")
					.Append('\n');
					sb.AppendLine($"private global::System.Collections.Frozen.FrozenDictionary<string, object?>? _methodRetryOptions = default;")
					.Append('\n');
					sb.AppendLine($"private global::System.Collections.Frozen.FrozenDictionary<string, object?>? _methodTimeoutOptions = default;")
					.Append('\n');
					// sb.AppendLine($"private global::System.Collections.Frozen.FrozenDictionary<string, global::Polly.ResiliencePipeline<{apiDefinition.TResponse}>?>? _methodPipelines = null;")
					// .Append('\n');

					sb.Append($$"""

							[MethodImpl(MethodImplOptions.AggressiveInlining)]
							public {{typeNameFullyQualifiedNameWithGlobal}} AddRetry(global::RestFetch.RetryOptions<{{apiDefinition.TResponse}}>? options) {
								if (_isBuilt) return this;

								lock (_lock) {
									_retryOptions = options;
								}

								return this;
							}

					""");

					sb.Append($$"""

							[MethodImpl(MethodImplOptions.AggressiveInlining)]
							public {{typeNameFullyQualifiedNameWithGlobal}} AddRetry(global::Polly.Retry.RetryStrategyOptions<{{apiDefinition.TResponse}}>? options) {
								if (_isBuilt) return this;

								lock (_lock) {
									_retryOptions = options;
								}

								return this;
							}

					""");

					sb.Append($$"""

							[MethodImpl(MethodImplOptions.AggressiveInlining)]
							public {{typeNameFullyQualifiedNameWithGlobal}} AddTimeout(global::RestFetch.TimeoutOptions? options) {
								if (_isBuilt) return this;

								lock (_lock) {
									_timeoutOptions = options;
								}

								return this;
							}

					""");

					sb.Append($$"""

							[MethodImpl(MethodImplOptions.AggressiveInlining)]
							public {{typeNameFullyQualifiedNameWithGlobal}} AddTimeout(global::Polly.Timeout.TimeoutStrategyOptions? options) {
								if (_isBuilt) return this;

								lock (_lock) {
									_timeoutOptions = options;
								}

								return this;
							}

					""");

					sb.Append($$"""

							public {{typeNameFullyQualifiedNameWithGlobal}} AddRetryFor(string methodName, global::RestFetch.RetryOptions<{{apiDefinition.TResponse}}>? options) {
								if (_isBuilt) return this;

								lock (_lock) {
									if (_methodRetryOptionsDraft is null) {
										_methodRetryOptionsDraft = new();
										if (_methodNames.Contains(methodName)) {
											_methodRetryOptionsDraft.Add(methodName, options);
										} else {
											global::RestFetch.ThrowHelper.ThrowMethodNameNotFound("{{typeNameFullyQualifiedNameWithGlobal}}", methodName, nameof(methodName));
										}
									} else {
										_methodRetryOptionsDraft[methodName] = options;
									}
								}

								return this;
							}

					""");

					sb.Append($$"""

							public {{typeNameFullyQualifiedNameWithGlobal}} AddRetryFor(string methodName, global::Polly.Retry.RetryStrategyOptions<{{apiDefinition.TResponse}}>? options) {
								if (_isBuilt) return this;

								lock (_lock) {
									if (_methodRetryOptionsDraft is null) {
										_methodRetryOptionsDraft = new();
										if (_methodNames.Contains(methodName)) {
											_methodRetryOptionsDraft.Add(methodName, options);
										} else {
											global::RestFetch.ThrowHelper.ThrowMethodNameNotFound("{{typeNameFullyQualifiedNameWithGlobal}}", methodName, nameof(methodName));
										}
									} else {
										_methodRetryOptionsDraft[methodName] = options;
									}
								}

								return this;
							}

					""");

					sb.Append($$"""

							public {{typeNameFullyQualifiedNameWithGlobal}} AddTimeoutFor(string methodName, global::RestFetch.TimeoutOptions? options) {
								if (_isBuilt) return this;

								lock (_lock) {
									if (_methodTimeoutOptionsDraft is null) {
										_methodTimeoutOptionsDraft = new();
										if (_methodNames.Contains(methodName)) {
											_methodTimeoutOptionsDraft.Add(methodName, options);
										} else {
											global::RestFetch.ThrowHelper.ThrowMethodNameNotFound("{{typeNameFullyQualifiedNameWithGlobal}}", methodName, nameof(methodName));
										}
									} else {
										_methodTimeoutOptionsDraft[methodName] = options;
									}
								}

								return this;
							}

					""");

					sb.Append($$"""

							public {{typeNameFullyQualifiedNameWithGlobal}} AddTimeoutFor(string methodName, global::Polly.Timeout.TimeoutStrategyOptions? options) {
								if (_isBuilt) return this;

								lock (_lock) {
									if (_methodTimeoutOptionsDraft is null) {
										_methodTimeoutOptionsDraft = new();
										if (_methodNames.Contains(methodName)) {
											_methodTimeoutOptionsDraft.Add(methodName, options);
										} else {
											global::RestFetch.ThrowHelper.ThrowMethodNameNotFound("{{typeNameFullyQualifiedNameWithGlobal}}", methodName, nameof(methodName));
										}
									} else {
										_methodTimeoutOptionsDraft[methodName] = options;
									}
								}

								return this;
							}

					""");

					sb.Append($$"""

							[MethodImpl(MethodImplOptions.AggressiveInlining)]
							public {{typeNameFullyQualifiedNameWithGlobal}} AddResiliencePipeline(global::Polly.ResiliencePipeline<{{apiDefinition.TResponse}}> pipeline) {
								if (_isBuilt) return this;

								lock (_lock) {
									_pipeline = pipeline;
								}

								return this;
							}

					""");

					sb.Append($$"""

							public {{typeNameFullyQualifiedNameWithGlobal}} AddResiliencePipelineFor(string methodName, global::Polly.ResiliencePipeline<{{apiDefinition.TResponse}}> pipeline) {
								if (_isBuilt) return this;

								lock (_lock) {
									if (_methodNames.Contains(methodName)) {
										if (_methodPipelines.ContainsKey(methodName)) {
											_methodPipelines[methodName] = pipeline;
										} else {
											_methodPipelines.Add(methodName, pipeline);
										}
									} else {
										global::RestFetch.ThrowHelper.ThrowMethodNameNotFound("{{typeNameFullyQualifiedNameWithGlobal}}", methodName, nameof(methodName));
									}
								}

								return this;
							}

					""");

					sb.Append($$"""

							[MethodImpl(MethodImplOptions.AggressiveInlining)]
							public {{typeNameFullyQualifiedNameWithGlobal}} Build() {
								if (_isBuilt) return this;

								lock (_lock) {
									_isBuilt = true;
								}

								_methodRetryOptions = _methodRetryOptionsDraft is null ? null : _methodRetryOptionsDraft.ToFrozenDictionary();
								_methodTimeoutOptions = _methodTimeoutOptionsDraft is null ? null : _methodTimeoutOptionsDraft.ToFrozenDictionary();
								_methodRetryOptionsDraft = null;
								_methodTimeoutOptionsDraft = null;

								return this;
							}

					""");

					sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
					.AppendLine("public void Dispose() {")
					.AppendLine("\tif (_ownedClient) Client.Dispose();")
					.AppendLine("}\n");

					sb.AppendLine($"public {typeName}(bool sharedClient, global::{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxyService<{apiDefinition.TResponse}> client) {{")
					.AppendLine("\t_ownedClient = !sharedClient;")
					.AppendLine("\tClient = client;")
					.AppendLine("}\n");

					sb.AppendLine($"public {typeName}(global::{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxyService<{apiDefinition.TResponse}> client): this(true, client) {{}}\n");

					sb.AppendLine($@"public {typeName}(): this(true, {apiDefinition.HttpClientProxy.FullyQualifiedNameWithGlobal}.Create(""{apiDefinition.BaseAddress}"")) {{}}")
					.Append('\n');

					sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create() => new();")
						.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create(string baseAddress) => new(true, {apiDefinition.HttpClientProxy.FullyQualifiedNameWithGlobal}.Create(baseAddress));")
						.AppendLine("")
						.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create(global::{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxyService<{apiDefinition.TResponse}> client) => new(true, client);")
						.AppendLine("")
						.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create(bool sharedClient, global::{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxyService<{apiDefinition.TResponse}> client) => new(sharedClient, client);")
						.AppendLine("");


					foreach (var method in apiDefinition.Methods) {
						var parameters = method.MethodInfo.Parameters.Zip(method.ApiParameterExtraInfos, static (paramInfo, extraInfo) => (paramInfo, extraInfo))
							.ToArray();
						(ParameterInfo, CancellationTokenParameter)? cancellationTokenParam = parameters
							.Where(static p => p.extraInfo is CancellationTokenParameter)
							.Select(static p => (p.paramInfo, extraInfo: (CancellationTokenParameter)p.extraInfo))
							.ToArray() is [var firstCancellation, ..] ? firstCancellation : null;
						(ParameterInfo, BodyParameter)? bodyParam = parameters
							.Where(static p => p.extraInfo is BodyParameter)
							.Select(static p => (p.paramInfo, extraInfo: (BodyParameter)p.extraInfo))
							.ToArray() is [var firstBody, ..] ? firstBody : null;

						sb.CreateMethod(RestFetchSourceGenerator.RestFetchNamespaceName, new("1.0.0"), method.MethodInfo, sb => {
							sb.CreateIndent(sb => {
								sb.AppendLine($@"global::System.Diagnostics.Debug.Assert(_isBuilt, ""The Build() method of <{apiDefinition.TypeInfo.FullyQualifiedName}> must be called before calling the request method <{method.MethodInfo.Name}>."");");

								sb.AppendLine($"{apiDefinition.TResponse} __resp = default!;");

								if (!method.ReturnsVoid && !method.ReturnsRawResponse) {
									if (method.ReturnsIAsyncEnumerableT) {
										sb.AppendLine($"{method.DeserializedResponseTypeName} __respBody = default!;");
									} else {
										sb.AppendLine($"{method.ExpectedReturnType!.FullyQualifiedNameWithGlobal} __respBody = default!;");
									}
								}

								sb.AppendLine("var __resourceManager = new global::RestFetch.ResourceManager();");
								if (bodyParam is not null && bodyParam.Value.Item2.TakeOwnershipOfArgument) {
									sb.AppendLine($"__resourceManager.TakeOwnership({bodyParam.Value.Item1.Name});");
								}
								sb.AppendLine("try {")
								.AppendLine($"var __requestId = {method.IdGenerator}.NextId();");

								var hasMetadata = GenerateMetadata(sb, method, parameters);

								var hasHeaders = GenerateHeaders(sb, method, parameters);

								GenerateApiContext(sb, apiDefinition, method, hasMetadata);

								GenerateOnError(sb, apiDefinition, method, cancellationTokenParam, () => {
									sb.AppendLine("using var __urlBuilder = global::Cysharp.Text.ZString.CreateStringBuilder();");
									if (method.IsMethodBaseAddress) {
										sb.AppendLine($@"__urlBuilder.Append(""{method.BaseAddress}"");");
									} else {
										sb.AppendLine("global::RestFetch.ThrowHelper.ThrowBaseAddressIsNullOrEmpty(Client.BaseAddress ?? CompileTimeBaseAddress);");
										// sb.AppendLine($@"global::System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(Client.BaseAddress ?? CompileTimeBaseAddress), ""The BaseAddress is null or empty."");");
										sb.AppendLine("__urlBuilder.Append(Client.BaseAddress ?? CompileTimeBaseAddress);");
									}


									GenerateParams(sb, method.PathSegments?.ToArray(), parameters);
									GenerateQueryString(sb, parameters);
									// sb.AppendLine($"var __draft = new global::RestFetch.RequestDraft(__urlBuilder.ToString(), {(hasHeaders ? "__headers" : "null")});");
									sb.AppendLine($"__context.Url = __urlBuilder.ToString();");
									sb.AppendLine($"__context.Headers = {(hasHeaders ? "__headers" : "null")};");
									// GenerateRequestBodySerializer(sb, bodyParam, cancellationTokenParam);
									// GenerateBeforeSend(sb, method, bodyParam, cancellationTokenParam);
									// GenerateSendAsync(sb, method, bodyParam, cancellationTokenParam, hasHeaders);
									// GenerateAfterResponse(sb, method, cancellationTokenParam);
									GenerateResilienceBoundary(sb, method, bodyParam, cancellationTokenParam);
									GenerateExceptionFactory(sb, method, cancellationTokenParam);
									GenerateResponseDeserializer(sb, method, cancellationTokenParam);
									GenerateOnSuccess(sb, method, cancellationTokenParam);

									if (!method.ReturnsVoid) {
										if (method.ReturnsRawResponse) {
											// NOTE: 转移所有权之前清理一遍,如果异常则可以正常释放__resp
											// sb.AppendLine("await __resourceManager.DisposeAsync().ConfigureAwait(false);");
											// NOTE: 转移resp所有权避免finally中Dispose()掉resp
											// sb.AppendLine("var ___resp = __resp;");
											// sb.AppendLine("__resp = null!;");
											// NOTE: 前面没有异常则归还所有权
											sb.AppendLine("__resourceManager.Absolve(__resp);")
											.AppendLine("return __resp;");
										} else if (method.ReturnsStream) {
											// NOTE: 转移所有权之前清理一遍,如果异常则可以正常释放__resp
											// sb.AppendLine("await __resourceManager.DisposeAsync().ConfigureAwait(false);");
											// NOTE: 转移resp所有权避免finally中Dispose()掉resp
											// sb.AppendLine("__resp = null!;")
											// NOTE: 前面没有异常则归还所有权
											sb.AppendLine("__resourceManager.Absolve(__resp);")
												.AppendLine("return __respBody;");
										} else if (method.ReturnsIAsyncEnumerableT) {
											if (method.OnError is null) {
												// sb.AppendLine("try {")
													sb.CreateIndent(sb => {
														// NOTE: 这里不加WithCancellation()了,因为即便加了也是使用反序列化器相同的CancellationToken对象
														// 而这个token在反序列化器中已经处理过了,我们必须假定反序列化器的实现者会正确处理这个token
														sb.AppendLine("await foreach (var item in __respBody.ConfigureAwait(false)) {")
														.CreateIndent(sb => {
															sb.AppendLine("switch (item) {")
																.AppendLine($"case global::RestFetch.SseEventResult<{method.ExpectedReturnType!.FullyQualifiedNameWithGlobal}>.Ok(var __sseData): yield return __sseData; continue;")
																.AppendLine($"case global::RestFetch.SseEventResult<{method.ExpectedReturnType.FullyQualifiedNameWithGlobal}>.Err(var __e, true): continue;")
																.AppendLine($"case global::RestFetch.SseEventResult<{method.ExpectedReturnType.FullyQualifiedNameWithGlobal}>.Err(var __e, false): global::System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(__e).Throw(); break;")
																.AppendLine("}\n");
														})
														.AppendLine("}\n");
													});
													// .AppendLine("} finally {")
													// .AppendLine("\t__resp?.Dispose();")
													// .AppendLine("}");
											}
											// if (method.OnError is null) {
											// 	sb.AppendLine("await foreach (var item in __respBody.ConfigureAwait(false)) {")
											// 	.CreateIndent(sb => {
											// 		sb.AppendLine("yield return item;");
											// 	})
											// 	.AppendLine("}\n");
											// }
										} else {
											sb.AppendLine($"return __respBody{(!method.ReturnDataType!.IsNullable ? "!" : "")};");
										}
									}
								});

								sb.AppendLine("} finally {");

								// if (method.ReturnsRawResponse || method.ReturnsStream) {
								// 	sb.AppendLine("try {")
								// 	.AppendLine("__resp?.Dispose();")
								// 	.AppendLine("} finally {")
								// 	.AppendLine("await __resourceManager.DisposeAsync().ConfigureAwait(false);")
								// 	.AppendLine("}");
								// } else {
									sb.AppendLine("await __resourceManager.DisposeAsync().ConfigureAwait(false);");
								// }
								sb.AppendLine("}");
							});
						});

						sb.AppendLine( $$"""
						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						private ValueTask<{{apiDefinition.TResponse}}> {{method.MethodInfo.Name}}ResilienceBoundary(
							{{(bodyParam is null
							? "object"
							: bodyParam.Value.Item1.Type.FullyQualifiedNameWithGlobal.TrimEnd('?')
							)}}? body,
							global::Polly.ResilienceContext resilienceContext,
							global::RestFetch.ApiContext context,
							global::RestFetch.ResourceManager resourceManager,
							global::System.Text.Json.Serialization.JsonSerializerContext? jsonContext
						) {
						""");
						sb.CreateIndent(sb => {
							sb.Append($$"""
								static async ValueTask<{{apiDefinition.TResponse}}> ExecBody(
								global::Polly.ResilienceContext resilienceContext,
								(
									{{(bodyParam is null ? "object" : bodyParam.Value.Item1.Type.FullyQualifiedNameWithGlobal.TrimEnd('?'))}}? body,
									global::RestFetch.ApiContext context,
									global::RestFetch.ResourceManager resourceManager,
									global::System.Text.Json.Serialization.JsonSerializerContext? jsonContext,
									global::RestFetch.IHttpClientProxyService<{{apiDefinition.TResponse}}> client
									) state) {
									var (body, __context, __resourceManager, jsonContext, client) = state;

									{{GenerateRequestBodySerializerString(bodyParam)}}
									{{GenerateBeforeSendString(method, bodyParam)}}
									{{GenerateSendAsyncString(method, bodyParam)}}
									{{GenerateAfterResponseString(method)}}
									return __resp;
								}

								global::RestFetch.RetryOptions<{{apiDefinition.TResponse}}>? retryOptions = default;
								var methodLvRetryOptions = _methodRetryOptions is null ? null : _methodRetryOptions.TryGetValue(context.MethodName, out var rtOptions) ? rtOptions : null;
								if (methodLvRetryOptions is null && _retryOptions is global::RestFetch.RetryOptions<{{apiDefinition.TResponse}}> globalRetryOptions) {
									retryOptions = globalRetryOptions;
								} else if (methodLvRetryOptions is global::RestFetch.RetryOptions<{{apiDefinition.TResponse}}> {
									Ignore: false
								} mLvRetryOptions) {
									retryOptions = mLvRetryOptions;
								}

								if (retryOptions?.ValidateRequestBody is not null) {
									retryOptions?.ValidateRequestBody?.Invoke(body, retryOptions, context);
								} else {
									{{method.RetryOptions?.Predicate ?? "global::RestFetch.DefaultRetryPredicate"}}.ValidateRequestBody(body, retryOptions, context);
								}

								return GetPipeline<{{(bodyParam is null
							? "object"
							: bodyParam.Value.Item1.Type.FullyQualifiedNameWithGlobal.TrimEnd('?')
							)}}, {{method.RetryOptions?.Predicate ?? "global::RestFetch.DefaultRetryPredicate"}}
							, {{method.RetryOptions?.OnRetry ?? "global::RestFetch.DefaultOnRetry"}}>(context.MethodName, body, context.HttpMethod).ExecuteAsync(ExecBody, resilienceContext, (body, context, resourceManager, jsonContext, Client));
							""");
						});
						sb.AppendLine("}\n");

					}

					sb.AppendLine($"private global::Polly.ResiliencePipeline<{apiDefinition.TResponse}> GetPipeline<TRequestBody, TRetryPredicate, TOnRetry>(string methodName, TRequestBody? body, global::System.Net.Http.HttpMethod httpMethod) where TRetryPredicate: IRetryPredicate<{apiDefinition.TResponse}> where TOnRetry: IOnRetry<{apiDefinition.TResponse}> {{");
					sb.CreateIndent(sb => {
						sb.AppendLine($$"""
								if (_methodPipelines.TryGetValue(methodName, out var pipeline)) {
									return pipeline;
								}

								object? retryOptions = default;
								var ignoreRetry = false;
								var methodLvRetryOptions = _methodRetryOptions is null ? null : _methodRetryOptions.TryGetValue(methodName, out var rtOptions) ? rtOptions : null;
								if (methodLvRetryOptions is null) {
									if (_methodRetryOptions is not null) {
										ignoreRetry = true;
									}
									if (_retryOptions is null) {
										retryOptions = null;
									} else {
										retryOptions = _retryOptions;
									}
								} else {
									if (methodLvRetryOptions is global::RestFetch.RetryOptions<{{apiDefinition.TResponse}}> {
										Ignore: true
									}) {
										ignoreRetry = true;
										retryOptions = null;
									} else {
										retryOptions = methodLvRetryOptions;
									}
								}

								object? timeoutOptions = default;
								var ignoreTimeout = false;
								var methodLvTimeoutOptions = _methodTimeoutOptions is null ? null : _methodTimeoutOptions.TryGetValue(methodName, out var tmoOptions) ? tmoOptions : null;
								if (methodLvTimeoutOptions is null) {
									if (_methodTimeoutOptions is not null) {
										ignoreTimeout = true;
									}
									if (_timeoutOptions is null) {
										timeoutOptions = null;
									} else {
										timeoutOptions = _timeoutOptions;
									}
								} else {
									if (methodLvTimeoutOptions is global::RestFetch.TimeoutOptions {
										Ignore: true
									}) {
										ignoreTimeout = true;
										timeoutOptions = null;
									} else {
										timeoutOptions = methodLvTimeoutOptions;
									}
								}

								var builder = new global::Polly.ResiliencePipelineBuilder<{{apiDefinition.TResponse}}>();
								if (ignoreRetry && ignoreTimeout) {
									pipeline = builder.Build();
									AddResiliencePipelineFor(methodName, pipeline);
									return pipeline;
								} else if (_pipeline is not null) {
									AddResiliencePipelineFor(methodName, _pipeline);
									return _pipeline;
								}

								global::Polly.Retry.RetryStrategyOptions<{{apiDefinition.TResponse}}>? pollyRetryOptions = default;
								if (retryOptions is global::RestFetch.RetryOptions<{{apiDefinition.TResponse}}> rOptions) {
									pollyRetryOptions = new global::Polly.Retry.RetryStrategyOptions<{{apiDefinition.TResponse}}>() {
										MaxRetryAttempts = rOptions.MaxRetryAttempts ?? 3,
										Delay = global::System.TimeSpan.FromSeconds(rOptions.Delay ?? 2),
										MaxDelay = rOptions.MaxDelay is int maxDelay? global::System.TimeSpan.FromSeconds(maxDelay) : null,
										UseJitter = rOptions.UseJitter ?? false,
										BackoffType = rOptions.BackoffType,
										ShouldHandle = rOptions.CreateShouldHandle?.Invoke(body, rOptions, httpMethod) ?? TRetryPredicate.CreateShouldHandle(body, rOptions, httpMethod),
										OnRetry = rOptions.OnRetry ?? TOnRetry.OnRetry
									};
								} else if (retryOptions is global::Polly.Retry.RetryStrategyOptions<{{apiDefinition.TResponse}}> rtyOptions) {
									pollyRetryOptions = rtyOptions;
								}

								global::Polly.Timeout.TimeoutStrategyOptions? totalTimeoutOptions = default;
								global::Polly.Timeout.TimeoutStrategyOptions? attemptTimeoutOptions = default;
								if (timeoutOptions is global::RestFetch.TimeoutOptions tOptions) {
									totalTimeoutOptions = tOptions.TotalTimeout is int tTimeout ? new() {
										Timeout = global::System.TimeSpan.FromSeconds(tTimeout)
									} : null;

									attemptTimeoutOptions = tOptions.AttemptTimeout is int aTimeout ? new() {
										Timeout = global::System.TimeSpan.FromSeconds(aTimeout)
									} : null;
								} else if (timeoutOptions is global::Polly.Timeout.TimeoutStrategyOptions ttTimeout) {
									totalTimeoutOptions = ttTimeout;
								}

								if (totalTimeoutOptions is not null) {
									builder.AddTimeout(totalTimeoutOptions);
								}

								if (pollyRetryOptions is not null) {
									builder.AddRetry(pollyRetryOptions);
								}

								if (attemptTimeoutOptions is not null) {
									builder.AddTimeout(attemptTimeoutOptions);
								}

								pipeline = builder.Build();
								AddResiliencePipelineFor(methodName, pipeline);
								return pipeline;
						""");

					});
					sb.AppendLine("}\n");
				}), withContainingType, apiDefinition.TypeInfo.IsInterface ? typeName : null,
				apiDefinition.ImplIRestFetchApiFullName is null
					? "global::System.IDisposable"
					: $"global::System.IDisposable, {apiDefinition.ImplIRestFetchApiFullName}", false);
			}
			var typeName = apiDefinition.TypeInfo.IsInterface ? "Impl" : apiDefinition.TypeInfo.Name;
			var typeNameFullyQualifiedNameWithGlobal =
			apiDefinition.TypeInfo.IsInterface
			? $"{apiDefinition.TypeInfo.FullyQualifiedNameWithGlobal}.{typeName}"
			: apiDefinition.TypeInfo.FullyQualifiedNameWithGlobal;
			sb.CreateUsing("System.Runtime.CompilerServices");
			sb.CreateUsing("System.Collections.Frozen");
			sb.CreateUsing("Cysharp.Text");
			sb.CreateUsing("Polly");
			sb.CreateUsing("Polly.Retry");
			sb.CreateUsing("Polly.Timeout");
			sb.CreateUsing("RestFetch");
			sb.CreateNamespace(apiDefinition.TypeInfo.ContainingNamespace);

			if (apiDefinition.TypeInfo.IsInterface) {
				sb.CreateType(RestFetchSourceGenerator.RestFetchNamespaceName, new("1.0.0"),
					apiDefinition.TypeInfo, sb => {
						// sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						// 	.AppendLine($@"public new static {typeNameFullyQualifiedNameWithGlobal} Create(string baseAddress) => new(true, {apiDefinition.HttpClientProxy.FullyQualifiedNameWithGlobal}.Create(baseAddress));")
						// 	.AppendLine("")
						// 	.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						// 	.AppendLine($@"public new static {typeNameFullyQualifiedNameWithGlobal} Create(IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> client) => new(true, client);")
						// 	.AppendLine("")
						// 	.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						// 	.AppendLine($@"public new static {typeNameFullyQualifiedNameWithGlobal} Create(bool sharedClient, IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> client) => new(sharedClient, client);")
						// 	.AppendLine("");
						GenerateType(sb, apiDefinition, false, typeNameFullyQualifiedNameWithGlobal, typeName);
					}, true);
			} else {
				GenerateType(sb, apiDefinition, true, typeNameFullyQualifiedNameWithGlobal, typeName);
			}
		});
		return sourceBuilder.ToString();
	}

	public static void GenerateOnError(SourceBuilder sb, ApiDefinitionTypeInfo apiDefinition, ApiDefinitionMethodInfo method, (ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam, Action fill) {
		if (method.OnError is HookInfo onerror) {
			sb.AppendLine("try {")
				.CreateIndent(_ => fill?.Invoke())
				.AppendLine("} catch(Exception e) {")
				.CreateIndent(sb => {
					sb.Append($"await {onerror.FullyQualifiedNameWithGlobal}.OnError(e, __context, {(cancellationTokenParam is {} token
						? token.paramInfo.Name
						: "global::System.Threading.CancellationToken.None")})", true);
					if (!onerror.ConfigureAwait) {
						sb.Append($".ConfigureAwait(false)");
					}
					sb.Append(";\n");
					sb.AppendLine("throw;");
				});

			sb.AppendLine("}");
			if (method.ReturnsIAsyncEnumerableT) {
				// NOTE: 这里不加WithCancellation()了,因为即便加了也是使用反序列化器相同的CancellationToken对象
				// 而这个token在反序列化器中已经处理过了,我们必须假定反序列化器的实现者会正确处理这个token
				sb.AppendLine("await foreach (var item in __respBody.ConfigureAwait(false)) {")
				.CreateIndent(sb => {
					sb.AppendLine("switch (item) {")
						.AppendLine($"case global::RestFetch.SseEventResult<{method.ExpectedReturnType!.FullyQualifiedNameWithGlobal}>.Ok(var __sseData): yield return __sseData; continue;")
						.AppendLine($"case global::RestFetch.SseEventResult<{method.ExpectedReturnType.FullyQualifiedNameWithGlobal}>.Err(var __e, true): continue;")
						.AppendLine($"case global::RestFetch.SseEventResult<{method.ExpectedReturnType.FullyQualifiedNameWithGlobal}>.Err(var __e, false): await {onerror.FullyQualifiedNameWithGlobal}.OnError(__e, __context, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")}){(onerror.ConfigureAwait ? "" : ".ConfigureAwait(false)")}; global::System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(__e).Throw();break;")
						.AppendLine("}\n");
				})
				.AppendLine("}\n");
			}
		} else {
			fill?.Invoke();
		}
	}

	public static bool GenerateMetadata(SourceBuilder sb, ApiDefinitionMethodInfo method, IEnumerable<(ParameterInfo paramInfo, ApiParameterExtraInfo extraInfo)> parameters) {
		var dynamicMetadata = parameters
			.Where(static param => param.extraInfo is MetadataParameter)
			.Select(static param => (((MetadataParameter)param.extraInfo).Key, param.paramInfo.Name, param.paramInfo.Type.IsNullable))
			.ToArray();
		var nullableMetadata = dynamicMetadata.Where(static p => p.IsNullable).ToArray();
		var notNullableMetadata = dynamicMetadata.Where(static p => !p.IsNullable).ToArray();
		var capacity = dynamicMetadata.Length;

		if (method.StaticMetadata.IsEmpty && dynamicMetadata.Length == 0) {
			return false;
		}

		capacity += method.StaticMetadata.Length;
		sb.AppendLine($"var __metaData = new Dictionary<string, object>({capacity}) {{")
			.CreateIndent(sb => {
			if (!method.StaticMetadata.IsEmpty) {
				foreach (var (key, value) in method.StaticMetadata) {
					sb.AppendLine($@"{{ ""{key}"", ""{value}"" }},");
				}
			}
			foreach (var (key, value, _) in notNullableMetadata) {
				sb.AppendLine($@"{{ ""{key}"", {value} }},");
			}
		});
		sb.AppendLine("};\n");
		foreach (var (key, value, _) in nullableMetadata) {
			sb.AppendLine($"if ({value} is {{}} __meta{value}) {{");
			sb.AppendLine($@"__metaData[""{key}""] = __meta{value};");
			sb.AppendLine("};\n");
		}
		return true;
	}

	public static bool GenerateQueryString(SourceBuilder sb, IEnumerable<(ParameterInfo paramInfo, ApiParameterExtraInfo extraInfo)> parameters) {
		var querystring = parameters
			.Where(static p => p.extraInfo is QueryParameter)
			.Select(static p =>(
				Key: ((QueryParameter)p.extraInfo).Alias,
				Value: p.paramInfo.Name,
				IsNullable: p.paramInfo.Type.IsNullable || p.paramInfo.Type.NullableAnnotation == NullableAnnotation.Annotated,
				ValueOnly: ((QueryParameter)p.extraInfo).ValueOnly,
				Serializer: ((QueryParameter)p.extraInfo).Serializer
			)).ToArray();

		if (querystring.Length > 0) {
			for (var i = 0; i < querystring.Length; ++i) {
				var (key, value, isNullable, ValueOnly, serializer) = querystring[i];
				if (!ValueOnly) {
					sb.AppendLine($@"__urlBuilder.Append({
						serializer.FullyQualifiedNameWithGlobal
					}.Serialize(""{key}"", {value}));");
				} else {
					sb.AppendLine($@"__urlBuilder.Append({
						serializer.FullyQualifiedNameWithGlobal
					}.Serialize({value}));");
				}

				if (i < querystring.Length - 1) {
					sb.AppendLine("__urlBuilder.Append('&');");
				}
			}

			return true;
		} else {
			return false;
		}
	}

	public static void GenerateParams(SourceBuilder sb, PathSegment[]? pathSegments, IEnumerable<(ParameterInfo paramInfo, ApiParameterExtraInfo extraInfo)> parameters) {
		if (pathSegments is null) {
			return;
		}
		var @params = parameters
			.Where(static p => p.extraInfo is ParamParameter)
			.Select(static p =>(
				Key: ((ParamParameter)p.extraInfo).Alias,
				Value: p.extraInfo.Name,
				UrlEncode: ((ParamParameter)p.extraInfo).UrlEncode,
				Serializer: ((ParamParameter)p.extraInfo).Serializer
			)).ToArray();

		foreach (var seg in pathSegments) {
			if (seg is LiteralSegment literal) {
				if (literal.Text.Length == 1) {
					sb.AppendLine($@"__urlBuilder.Append('{literal.Text}');");
				} else {
					sb.AppendLine($@"__urlBuilder.Append(""{literal.Text}"");");
				}
			} else if (seg is ParamSegment param) {
				foreach (var (key, value, shouldEncode, serializer) in @params) {
					if (key == param.RootParamName) {
						sb.AppendLine($@"__urlBuilder.Append({serializer.FullyQualifiedNameWithGlobal}.{(shouldEncode ? "Encode(" : "Serialize(")}""{key}"", {(param.Expr.Contains('.') ? param.Expr : value)}));");
					}
				}
			} else {
				return;
			}
		}
	}

	public static bool GenerateHeaders(SourceBuilder sb, ApiDefinitionMethodInfo method, IEnumerable<(ParameterInfo paramInfo, ApiParameterExtraInfo extraInfo)> parameters) {
		var dynamicHeader = parameters
			.Where(static param => param.extraInfo is HeaderParameter)
			.Select(static param => (((HeaderParameter)param.extraInfo).Key,
				Value: param.paramInfo.Name, param.paramInfo.Type.SpecialType,
				IsEnumerable: param.paramInfo.Type.MinimallyQualifiedName.Contains("IEnumerable<KeyValuePair<string"),
				IsNullable: param.paramInfo.Type.IsNullable
			))
			.ToArray();
		var singleHeaders = dynamicHeader.Where(static h => h.SpecialType == SpecialType.System_String);
		var capacity = dynamicHeader.Length + 4;

		if (method.StaticHeaders.IsEmpty && dynamicHeader.Length == 0) {
			return false;
		}

		if (!method.StaticHeaders.IsEmpty) {
			capacity += method.StaticHeaders.Length;
		}

		var nullableSingleHeaders = singleHeaders.Where(static h => h.IsNullable).ToArray();
		var notNullableSingleHeaders = singleHeaders.Where(static h => !h.IsNullable).ToArray();

		sb.AppendLine($"var __headers = new List<KeyValuePair<string, string>>({capacity}) {{");
		sb.CreateIndent(sb => {
			if (!method.StaticHeaders.IsEmpty) {
				foreach (var (key, value) in method.StaticHeaders) {
					sb.AppendLine($@"new(""{key}"", ""{value}""),");
				}
			}
			foreach (var (key, value, _, _, _) in notNullableSingleHeaders) {
				sb.AppendLine($@"new(""{key}"", {value}),");
			}
		});
		sb.AppendLine("};\n");

		foreach (var (key, value, _, _, _) in nullableSingleHeaders) {
			sb.AppendLine($"if ({value} is not null) {{");
			sb.AppendLine($@"__headers.Add(""{key}"", {value});");
			sb.AppendLine("};\n");
		}

		var headerCollections = dynamicHeader.Where(static h => h.IsEnumerable);
		foreach (var (_, value, _, _, isNullable) in headerCollections) {
			if (isNullable) {
				sb.AppendLine($"if ({value} is not null) {{");
				sb.CreateIndent(sb => {
					sb.AppendLine($"__headers.AddRange({value});");
				});
				sb.AppendLine("}\n");
			} else {
				sb.AppendLine($"__headers.AddRange({value});");
			}
		}
		return true;
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateApiContext(SourceBuilder sb,
		ApiDefinitionTypeInfo apiDefinition,
		ApiDefinitionMethodInfo method,
		bool hasMetadata) {

		var returnDataTypeKind = method.ReturnDataTypeKind switch {
			ReturnTypeKind.Other => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.Other",
			ReturnTypeKind.SystemString => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.SystemString",
			ReturnTypeKind.RawResponse => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.RawResponse",
			ReturnTypeKind.Stream => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.Stream",
			ReturnTypeKind.JsonNode => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.JsonNode",
			ReturnTypeKind.JsonDocument => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.JsonDocument",
			ReturnTypeKind.SseEvent => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.SseEvent",
			ReturnTypeKind.SseEventT => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.SseEventT",
			ReturnTypeKind.SseEventString => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.SseEventString",
			ReturnTypeKind.SseEventJsonNode => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.SseEventJsonNode",
			ReturnTypeKind.SseEventJsonDocument => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.SseEventJsonDocument",
			ReturnTypeKind.SseItemT => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.SseItemT",
			ReturnTypeKind.SseItemString => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.SseItemString",
			ReturnTypeKind.SseItemJsonNode => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.SseItemJsonNode",
			ReturnTypeKind.SseItemJsonDocument => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.SseItemJsonDocument",
			ReturnTypeKind.ApiResponseT => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.ApiResponseT",
			ReturnTypeKind.ApiResponseString => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.ApiResponseString",
			ReturnTypeKind.ApiResponseJsonNode => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.ApiResponseJsonNode",
			ReturnTypeKind.ApiResponseJsonDocument => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.ApiResponseJsonDocument",
			ReturnTypeKind.Void => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.Void",
			_ => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeKind.Other",
		};
		var returnTypeWrapperKind = method.AsyncReturnTypeKind switch {
			ReturnTypeWrapperKind.CustomTask => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeWrapperKind.CustomTask",
			ReturnTypeWrapperKind.IAsyncEnumerableT => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeWrapperKind.IAsyncEnumerableT",
			ReturnTypeWrapperKind.IObservableT => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeWrapperKind.IObservableT",
			ReturnTypeWrapperKind.NotAsync => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeWrapperKind.NotAsync",
			ReturnTypeWrapperKind.Task => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeWrapperKind.Task",
			ReturnTypeWrapperKind.TaskT => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeWrapperKind.TaskT",
			ReturnTypeWrapperKind.ValueTask => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeWrapperKind.ValueTask",
			ReturnTypeWrapperKind.ValueTaskT => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeWrapperKind.ValueTaskT",
			ReturnTypeWrapperKind.Void => $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeWrapperKind.Void",
			_ => ""
		};

		sb.AppendLine($"var __context = new global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ApiContext(");
		sb.CreateIndent(sb => {
			sb.AppendLine("id: __requestId,");
			sb.AppendLine("client: Client,");
			sb.AppendLine("options: Client.Options,");
			sb.AppendLine($"returnsAsyncEnumerable: {(method.ReturnsIAsyncEnumerableT ? "true" : "false")},");
			sb.AppendLine($@"methodName: ""{method.MethodInfo.Name}"",");
			sb.AppendLine($@"className: ""{apiDefinition.TypeInfo.FullyQualifiedName}"",");
			// sb.AppendLine($@"fullName: ""{apiDefinition.TypeInfo.FullyQualifiedName}"",");
			sb.AppendLine($"httpMethod: {method.HttpMethod},");
			if (!method.ReturnsVoid && !method.ReturnsRawResponse) {
				sb.AppendLine($@"returnTypeInfo: new global::RestFetch.ReturnTypeInfo(typeof({(method.ReturnDataType!.IsNullable
					? method.ReturnDataType.FullyQualifiedNameWithGlobal[..^1] : method.ReturnDataType!.FullyQualifiedNameWithGlobal)}),
					{(method.ReturnDataType.IsNullable ? "true" : "false")},
					{(method.ReturnDataType.IsValueType ? "true" : "false")},
					{(method.ResponseBodyDeserializer.SkipNullableCheck ? "true" : "false")},
					{returnDataTypeKind}, {returnTypeWrapperKind}),");
			} else {
				sb.AppendLine($@"returnTypeInfo: new(null),");
			}
			sb.AppendLine($"metadata: {(hasMetadata ? "__metaData" : "null")}");
		});
		sb.AppendLine(");\n");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateRequestBodySerializer(SourceBuilder sb,
		(ParameterInfo paramInfo, BodyParameter extraInfo)? bodyParam,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		if (bodyParam is {} body) {
			sb.Append($"var __body = await {body.extraInfo.Serializer.FullyQualifiedNameWithGlobal}.Serialize({body.paramInfo.Name}, {(body.extraInfo.JsonContext is null ? "null" : $"{body.extraInfo.JsonContext}.Default")}, __context, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!body.extraInfo.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
			// HttpContent,Stream参数不会获取默认的序列化器返回值的所有权
			if (body.extraInfo.TakeOwnershipOfSerializeResult) {
				sb.AppendLine("if (__body is global::System.IDisposable __bd) __resourceManager.TakeOwnership(__bd);");
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GenerateRequestBodySerializerString(
		(ParameterInfo paramInfo, BodyParameter extraInfo)? bodyParam
		) {
		if (bodyParam is {} body) {
			using var sb = ZString.CreateStringBuilder();
			sb.Append($"var __body = await {body.extraInfo.Serializer.FullyQualifiedNameWithGlobal}.Serialize(body, jsonContext, __context, resilienceContext.CancellationToken)");
			if (!body.extraInfo.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
			// HttpContent,Stream参数不会获取默认的序列化器返回值的所有权
			if (body.extraInfo.TakeOwnershipOfSerializeResult) {
				sb.AppendLine("if (__body is global::System.IDisposable __bd) __resourceManager.TakeOwnership(__bd);");
			}
			return sb.ToString();
		} else {
			return "";
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateSendAsync(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, BodyParameter extraInfo)? bodyParam,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam,
		bool hasHeaders
		) {
		sb.AppendLine($"__resp = await Client.SendAsync{(bodyParam is null ? "<global::System.Net.Http.HttpContent>" : "")}({(bodyParam is null ? "null" : "__body")}, __context, __resourceManager, {(cancellationTokenParam is {} token
			? token.paramInfo.Name
			: "global::System.Threading.CancellationToken.None")})");
		if (!method.HttpClientProxy.ConfigureAwait) {
			sb.Append($".ConfigureAwait(false)");
		}
		sb.Append(";\n");

		// if (!method.ReturnsRawResponse && !method.ReturnsStream) {
		sb.AppendLine("__resourceManager.TakeOwnership(__resp);");
		// }
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateResilienceBoundary(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, BodyParameter extraInfo)? bodyParam,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		sb.AppendLine($"var __resilienceContext = global::Polly.ResilienceContextPool.Shared.Get({(cancellationTokenParam is {} ctoken
			? ctoken.paramInfo.Name
			: "global::System.Threading.CancellationToken.None")});");
		sb.AppendLine("try {");
		sb.CreateIndent(sb => {
			sb.AppendLine("__resilienceContext.Properties.Set(ApiContextKey, __context);");
			sb.AppendLine($"__resp = await {method.MethodInfo.Name}ResilienceBoundary({(bodyParam is null ? "null" : bodyParam.Value.paramInfo.Name)}, __resilienceContext, __context, __resourceManager, {(bodyParam?.extraInfo.JsonContext is string jsonContext ? $"{jsonContext}.Default" : "null")})");
			if (!method.HttpClientProxy.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
		});
		sb.AppendLine("} finally { global::Polly.ResilienceContextPool.Shared.Return(__resilienceContext); }");

		// if (!method.ReturnsRawResponse && !method.ReturnsStream) {
		// sb.AppendLine("__resourceManager.TakeOwnership(__resp);");
		// }
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GenerateSendAsyncString(
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, BodyParameter extraInfo)? bodyParam
		) {
		using var sb = ZString.CreateStringBuilder();
		sb.AppendLine($"var __resp = await client.SendAsync{(bodyParam is null ? "<global::System.Net.Http.HttpContent>" : "")}({(bodyParam is null ? "null" : "__body")}, __context, __resourceManager, resilienceContext.CancellationToken)");
		if (!method.HttpClientProxy.ConfigureAwait) {
			sb.Append($".ConfigureAwait(false)");
		}
		sb.Append(";\n");

		// if (!method.ReturnsRawResponse && !method.ReturnsStream) {
		sb.AppendLine("__resourceManager.TakeOwnership(__resp);");
		// }
		return sb.ToString();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateBeforeSend(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, BodyParameter extraInfo)? bodyParam,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		if (method.BeforeSendHook is HookInfo beforeSend) {
			sb.Append($"await {beforeSend.FullyQualifiedNameWithGlobal}.BeforeSend({(bodyParam is not null ? "__body" : "null")}, __context, __resourceManager, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!beforeSend.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GenerateBeforeSendString(
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, BodyParameter extraInfo)? bodyParam
		) {
		if (method.BeforeSendHook is HookInfo beforeSend) {
			using var sb = ZString.CreateStringBuilder();
			sb.Append($"await {beforeSend.FullyQualifiedNameWithGlobal}.BeforeSend({(bodyParam is not null ? "__body" : "null")}, __context, __resourceManager, resilienceContext.CancellationToken)");
			if (!beforeSend.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
			return sb.ToString();
		} else {
			return "";
		}

	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateAfterResponse(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		if (method.AfterResponseHook is HookInfo afterResponse) {
			sb.Append($"await {afterResponse.FullyQualifiedNameWithGlobal}.AfterResponse(__resp, __context, __resourceManager, {(cancellationTokenParam	is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!afterResponse.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GenerateAfterResponseString(
		ApiDefinitionMethodInfo method
		) {
		if (method.AfterResponseHook is HookInfo afterResponse) {
			using var sb = ZString.CreateStringBuilder();
			sb.Append($"await {afterResponse.FullyQualifiedNameWithGlobal}.AfterResponse(__resp, __context, __resourceManager, resilienceContext.CancellationToken)");
			if (!afterResponse.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
			return sb.ToString();
		} else {
			return "";
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateExceptionFactory(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		if (method.ExceptionFactory is HookInfo exceptionFactory) {
			sb.Append($"var __exception = await {exceptionFactory.FullyQualifiedNameWithGlobal}.CreateException(__resp, __context, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!exceptionFactory.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
			sb.AppendLine("if (__exception is not null) {")
			.AppendLine("global::System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(__exception).Throw();")
			.AppendLine("}\n");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateResponseDeserializer(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		if (method.ReturnsVoid || method.ReturnsRawResponse) {
			return;
		}


		if (method.ReturnsIAsyncEnumerableT) {
			sb.AppendLine($@"__respBody = {
				method.ResponseBodyDeserializer.FullyQualifiedNameWithGlobal
				}.Parse<{(method.ResponseBodyDeserializer.CallWithGenericArgs ? $"{method.ReturnDataType!.FullyQualifiedNameWithGlobal}, " : "")}{method.OnSseEventError?.FullyQualifiedNameWithGlobal ?? $"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.DefaultSseEventErrorHandler"}>
					(__resp, __context, {(method.ResponseBodyDeserializer.JsonContext is null
					? "null" : $"{method.ResponseBodyDeserializer.JsonContext}.Default")}, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")});")
				.AppendLine("");
		} else {
			if (method.ResponseBodyDeserializer.SkipNullableCheck) {
				sb.AppendLine("#pragma warning disable CS8600");
			}
			sb.Append($@"__respBody = await {
				method.ResponseBodyDeserializer.FullyQualifiedNameWithGlobal
				}.Parse{(method.ResponseBodyDeserializer.CallWithGenericArgs ? $"<{
					method.ReturnDataType!.FullyQualifiedNameWithGlobal
					}>" : "")}
					(__resp, __context, {(method.ResponseBodyDeserializer.JsonContext is null
					? "null" : $"{method.ResponseBodyDeserializer.JsonContext}.Default")}, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!method.ResponseBodyDeserializer.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");

			if (method.ResponseBodyDeserializer.SkipNullableCheck) {
				sb.AppendLine("#pragma warning restore CS8600");
			}

			if (method.ReturnDataType!.NullableAnnotation != NullableAnnotation.Annotated
				&& !method.ReturnDataType.IsNullable && !method.ResponseBodyDeserializer.SkipNullableCheck) {
				sb.AppendLine("if (__respBody is null) {");
				sb.CreateIndent(sb => {
					sb.AppendLine($"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeException.Throw(__context.MethodName, __context.ClassName);");
				});
				sb.AppendLine("}\n");
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateOnSuccess(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		if (method.OnSuccess is HookInfo onSuccess) {
			sb.Append($"await {onSuccess.FullyQualifiedNameWithGlobal}.OnSuccess{(method.ReturnsVoid
				|| method.ReturnsRawResponse
				|| method.ReturnsStream
				|| method.ReturnsIAsyncEnumerableT ? $"<object>" : "")}({(method.ReturnsVoid
				|| method.ReturnsRawResponse
				|| method.ReturnsStream
				|| method.ReturnsIAsyncEnumerableT ? "null" : "__respBody")}, __context, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!onSuccess.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
		}
	}
}