using System.Runtime.CompilerServices;

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
					// sb.AppendLine($@"public string BaseAddress = ""{apiDefinition.BaseAddress}"";");
					sb.AppendLine($@"public readonly global::{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> Client;")
					.Append('\n');
					sb.AppendLine($"private readonly bool _ownedClient = true;")
					.Append('\n');
					sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
					.AppendLine("public void Dispose() {")
					.AppendLine("\tif (_ownedClient) Client.Dispose();")
					.AppendLine("}\n");

					sb.AppendLine($"public {typeName}(bool sharedClient, global::{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> client) {{")
					.AppendLine("\t_ownedClient = !sharedClient;")
					.AppendLine("\tClient = client;")
					.AppendLine("}\n");

					sb.AppendLine($"public {typeName}(global::{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> client): this(true, client) {{}}\n");

					sb.AppendLine($@"public {typeName}(): this(true, {apiDefinition.HttpClientProxy.FullyQualifiedNameWithGlobal}.Create(""{apiDefinition.BaseAddress}"")) {{}}")
					.Append('\n');

					sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create() => new();")
						.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create(string baseAddress) => new(true, {apiDefinition.HttpClientProxy.FullyQualifiedNameWithGlobal}.Create(baseAddress));")
						.AppendLine("")
						.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create(global::{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> client) => new(true, client);")
						.AppendLine("")
						.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create(bool sharedClient, global::{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> client) => new(sharedClient, client);")
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
								sb.AppendLine($"var __requestId = {method.IdGenerator}.NextId();");

								var hasMetadata = GenerateMetadata(sb, method, parameters);

								var hasHeaders = GenerateHeaders(sb, method, parameters);

								GenerateApiContext(sb, apiDefinition, method, hasMetadata);

								GenerateOnError(sb, apiDefinition, method, cancellationTokenParam, () => {
									sb.AppendLine("using var __urlBuilder = global::Cysharp.Text.ZString.CreateStringBuilder();");
									if (method.IsMethodBaseAddress) {
										sb.AppendLine($@"__urlBuilder.Append(""{method.BaseAddress}"");");
									} else {
										sb.AppendLine("__urlBuilder.Append(Client.BaseAddress);");
									}


									GenerateParams(sb, method.PathSegments?.ToArray(), parameters);
									GenerateQueryString(sb, parameters);
									sb.AppendLine("__context.Url = __urlBuilder.ToString();");
									GenerateRequestBodySerializer(sb, bodyParam, cancellationTokenParam);
									GenerateBeforeSend(sb, method, bodyParam, cancellationTokenParam);
									GenerateSendAsync(sb, method, bodyParam, cancellationTokenParam, hasHeaders);
									GenerateAfterResponse(sb, method, cancellationTokenParam);
									GenerateExceptionFactory(sb, method, cancellationTokenParam);
									GenerateResponseDeserializer(sb, method, cancellationTokenParam);
									GenerateOnSuccess(sb, method, cancellationTokenParam);

									if (!method.ReturnsVoid) {
										if (method.ReturnsRawResponse) {
											sb.AppendLine("return __resp;");
										} else if (method.ReturnsStream) {
											// NOTE: 转移resp所有权避免finally中Dispose()掉resp
											sb.AppendLine("__resp = null!;") 
												.AppendLine("return __respBody;");
										} else if (method.ReturnsIAsyncEnumerableT) {
											if (method.OnError is null) {
												sb.AppendLine("try {")
													.CreateIndent(sb => {
														sb.AppendLine("await foreach (var item in __respBody.ConfigureAwait(false)) {")
														.CreateIndent(sb => {
															sb.AppendLine("switch (item) {")
																.AppendLine($"case global::RestFetch.SseEventResult<{method.ExceptedReturnType!.FullyQualifiedNameWithGlobal}>.Ok(var __sseData): yield return __sseData; continue;")
																.AppendLine($"case global::RestFetch.SseEventResult<{method.ExceptedReturnType.FullyQualifiedNameWithGlobal}>.Err(var __e, true): continue;")
																.AppendLine($"case global::RestFetch.SseEventResult<{method.ExceptedReturnType.FullyQualifiedNameWithGlobal}>.Err(var __e, false): global::System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(__e).Throw();")
																.AppendLine("}\n");
														})
														.AppendLine("}\n");
													})
													.AppendLine("} finally {")
													.AppendLine("\t__resp?.Dispose();")
													.AppendLine("}");
											}
											// if (method.OnError is null) {
											// 	sb.AppendLine("await foreach (var item in __respBody.ConfigureAwait(false)) {")
											// 	.CreateIndent(sb => {
											// 		sb.AppendLine("yield return item;");
											// 	})
											// 	.AppendLine("}\n");
											// }
										} else {
											sb.AppendLine("return __respBody;");
										}
									}
								});
							});
						});
					}
				}), withContainingType, apiDefinition.TypeInfo.IsInterface ? typeName : null, "global::System.IDisposable");
			}
			var typeName = apiDefinition.TypeInfo.IsInterface ? $"Impl" : apiDefinition.TypeInfo.Name;
			var typeNameFullyQualifiedNameWithGlobal = apiDefinition.TypeInfo.IsInterface ? $"{apiDefinition.TypeInfo.FullyQualifiedNameWithGlobal}.{(
				apiDefinition.TypeInfo.IsGenericType && !apiDefinition.TypeInfo.TypeParameters.IsEmpty
				? $"{typeName}<{string.Join(", ", apiDefinition.TypeInfo.TypeArguments.Select(static t => t.Name))}>"
				: typeName)}" : apiDefinition.TypeInfo.FullyQualifiedNameWithGlobal;
			sb.CreateUsing("System.Runtime.CompilerServices");
			sb.CreateUsing("Cysharp.Text");
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
			sb.AppendLine($"{apiDefinition.TResponse} __resp = default!;");
			if (method.ReturnsIAsyncEnumerableT) {
				sb.AppendLine($"{method.DeserializedResponseTypeName} __respBody = default!;");
			}
			sb.AppendLine("try {")
				.CreateIndent(_ => fill?.Invoke())
				.AppendLine("} catch(Exception e) {")
				.CreateIndent(sb => {
					sb.AppendLine("try {")
						.Append($"await {onerror.FullyQualifiedNameWithGlobal}.OnError(e, in __context, {(cancellationTokenParam is {} token
						? token.paramInfo.Name
						: "global::System.Threading.CancellationToken.None")})", true);
					if (!onerror.ConfigureAwait) {
						sb.Append($".ConfigureAwait(false)");
					}
					sb.Append(";\n")
						.AppendLine("} finally {")
						.AppendLine("\t__resp?.Dispose();")
						.AppendLine("}")
						.AppendLine("throw;");
				});
			// NOTE: 对于Stream,仅在异常的时候Dispose()掉resp,正常情况下前面会把resp置为null从而这里不会Dispose()
			// 避免Stream被返回后而resp被Dispose()
			if (!method.ReturnsIAsyncEnumerableT && !method.ReturnsRawResponse) {
				sb.AppendLine("} finally {")
					.AppendLine("\t__resp?.Dispose();");
			}
			sb.AppendLine("}");
			if (method.ReturnsIAsyncEnumerableT) {
				sb.AppendLine("try {")
					.CreateIndent(sb => {
						sb.AppendLine("await foreach (var item in __respBody.ConfigureAwait(false)) {")
						.CreateIndent(sb => {
							sb.AppendLine("switch (item) {")
								.AppendLine($"case global::RestFetch.SseEventResult<{method.ExceptedReturnType!.FullyQualifiedNameWithGlobal}>.Ok(var __sseData): yield return __sseData; continue;")
								.AppendLine($"case global::RestFetch.SseEventResult<{method.ExceptedReturnType.FullyQualifiedNameWithGlobal}>.Err(var __e, true): continue;")
								.AppendLine($"case global::RestFetch.SseEventResult<{method.ExceptedReturnType.FullyQualifiedNameWithGlobal}>.Err(var __e, false): await {onerror.FullyQualifiedNameWithGlobal}.OnError(__e, in __context, {(cancellationTokenParam is {} token
						? token.paramInfo.Name
						: "global::System.Threading.CancellationToken.None")}); throw __e;")
								.AppendLine("}\n");
						})
						.AppendLine("}\n");
					})
					.AppendLine("} finally {")
					.AppendLine("\t__resp?.Dispose();")
					.AppendLine("}");
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
				AutoAlias: ((QueryParameter)p.extraInfo).AutoAlias,
				Serializer: ((QueryParameter)p.extraInfo).Serializer
			)).ToArray();

		if (querystring.Length > 0) {
			for (var i = 0; i < querystring.Length; ++i) {
				var (key, value, isNullable, autoAlias, serializer) = querystring[i];
				if (autoAlias) {
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

		sb.AppendLine($"var __headers = new Dictionary<string, string>({capacity}, StringComparer.OrdinalIgnoreCase) {{");
		sb.CreateIndent(sb => {
			if (!method.StaticHeaders.IsEmpty) {
				foreach (var (key, value) in method.StaticHeaders) {
					sb.AppendLine($@"{{ ""{key}"", ""{value}"" }},");
				}
			}
			foreach (var (key, value, _, _, _) in notNullableSingleHeaders) {
				sb.AppendLine($@"{{ ""{key}"", {value} }},");
			}
		});
		sb.AppendLine("};\n");

		foreach (var (key, value, _, _, _) in nullableSingleHeaders) {
			sb.AppendLine($"if ({value} is not null) {{");
			sb.AppendLine($@"__headers[""{key}""] = {value};");
			sb.AppendLine("};\n");
		}

		var headerCollections = dynamicHeader.Where(static h => h.IsEnumerable);
		foreach (var (_, value, _, _, isNullable) in headerCollections) {
			if (isNullable) {
				sb.AppendLine($"if ({value} is not null) {{");
				sb.CreateIndent(sb => {
					sb.AppendLine($"foreach (var (__key, __value) in {value}) {{");
					sb.CreateIndent(sb => {
						sb.AppendLine($"__headers[__key] = __value;");
					});
					sb.AppendLine("}\n");
				});
				sb.AppendLine("}\n");
			} else {
				sb.AppendLine($"foreach (var (__key, __value) in {value}) {{");
				sb.CreateIndent(sb => {
					sb.AppendLine($"__headers[__key] = __value;");
				});
				sb.AppendLine("}\n");
			}
		}
		return true;
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateApiContext(SourceBuilder sb,
		ApiDefinitionTypeInfo apiDefinition,
		ApiDefinitionMethodInfo method,
		bool hasMetadata) {
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
			sb.Append($"var __body = await {body.extraInfo.Serializer.FullyQualifiedNameWithGlobal}.Serialize({body.paramInfo.Name}, {(body.extraInfo.JsonContext is null ? "null" : $"{body.extraInfo.JsonContext}.Default")}, in __context, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!body.extraInfo.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateSendAsync(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, BodyParameter extraInfo)? bodyParam,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam,
		bool hasHeaders
		) {
		var lineStart = true;
		if (method.OnError is null) {
			lineStart = false;
			if (method.ReturnsRawResponse || method.ReturnsStream) {
				sb.Append("var ", true);
			} else {
				sb.Append("using var ", true);
			}
		}
		sb.Append($"__resp = await Client.SendAsync{(bodyParam is null ? "<global::System.Net.Http.HttpContent>" : "")}(in __context, {(hasHeaders ? "__headers" : "null")}, {(bodyParam is null ? "null" : "__body")}, {(cancellationTokenParam is {} token
			? token.paramInfo.Name
			: "global::System.Threading.CancellationToken.None")})", lineStart);
		if (!method.HttpClientProxy.ConfigureAwait) {
			sb.Append($".ConfigureAwait(false)");
		}
		sb.Append(";\n");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateBeforeSend(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, BodyParameter extraInfo)? bodyParam,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		if (method.BeforeSendHook is HookInfo beforeSend) {
			sb.Append($"await {beforeSend.FullyQualifiedNameWithGlobal}.BeforeSend({(bodyParam is not null ? "__body" : "null")}, in __context, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!beforeSend.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateAfterResponse(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		if (method.AfterResponseHook is HookInfo afterResponse) {
			sb.Append($"await {afterResponse.FullyQualifiedNameWithGlobal}.AfterResponse(__resp, in __context, {(cancellationTokenParam	is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!afterResponse.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateExceptionFactory(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		if (method.ExceptionFactory is HookInfo exceptionFactory) {
			sb.Append($"var __exception = await {exceptionFactory.FullyQualifiedNameWithGlobal}.CreateException(__resp, in __context, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!exceptionFactory.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
			sb.AppendLine("if (__exception is not null) {")
			.AppendLine("global::RestFetch.ThrowHelper.Throw(__exception);")
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
		sb.AppendLine($@"var __returnTypeInfo = new global::RestFetch.ReturnTypeInfo(typeof({(method.ReturnDataType!.IsNullable
			? method.ReturnDataType!.FullyQualifiedNameWithGlobal[..^1] : method.ReturnDataType!.FullyQualifiedNameWithGlobal)}),
			{(method.ReturnDataType.IsNullable ? "true" : "false")},
			{(method.ReturnDataType.IsValueType ? "true" : "false")},
			{(method.ResponseBodyDeserializer.SkipNullableCheck ? "true" : "false")},
			{returnDataTypeKind}, {returnTypeWrapperKind});");
		if (method.ReturnsIAsyncEnumerableT) {
			sb.AppendLine($@"{(method.OnError is null ? "var " : "")}__respBody = {
				method.ResponseBodyDeserializer.FullyQualifiedNameWithGlobal
				}.Parse{(method.ResponseBodyDeserializer.CallWithGenericArgs ? $"<{
					method.ReturnDataType!.FullyQualifiedNameWithGlobal
					}>" : "")}
					(__resp, in __returnTypeInfo, in __context, {(method.ResponseBodyDeserializer.JsonContext is null
					? "null" : $"{method.ResponseBodyDeserializer.JsonContext}.Default")}, {(method.OnSseEventError is null
					? "null" : $"{method.OnSseEventError.Value.FullyQualifiedNameWithGlobal}.OnSseEventError")}, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")});")
				.AppendLine("");
		} else {
			sb.Append($@"var __respBody = await {
				method.ResponseBodyDeserializer.FullyQualifiedNameWithGlobal
				}.Parse{(method.ResponseBodyDeserializer.CallWithGenericArgs ? $"<{
					method.ReturnDataType!.FullyQualifiedNameWithGlobal
					}>" : "")}
					(__resp, in __returnTypeInfo, in __context, {(method.ResponseBodyDeserializer.JsonContext is null
					? "null" : $"{method.ResponseBodyDeserializer.JsonContext}.Default")}, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!method.ResponseBodyDeserializer.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");

			if (method.ReturnDataType.NullableAnnotation != NullableAnnotation.Annotated
				&& !method.ReturnDataType.IsNullable && !method.ResponseBodyDeserializer.SkipNullableCheck) {
				sb.AppendLine("if (__respBody is null) {");
				sb.CreateIndent(sb => {
					sb.AppendLine($"global::{RestFetchSourceGenerator.RestFetchNamespaceName}.ReturnTypeException.Throw();");
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
				|| method.ReturnsIAsyncEnumerableT ? "null" : "__respBody")}, in __context, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "global::System.Threading.CancellationToken.None")})", true);
			if (!onSuccess.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
		}
	}
}