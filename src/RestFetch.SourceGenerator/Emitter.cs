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
					sb.AppendLine($@"public readonly IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> Client;")
					.Append('\n');
					sb.AppendLine($"private readonly bool _OwnedClient = true;")
					.Append('\n');
					sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
					.AppendLine("public void Dispose() {")
					.AppendLine("\tif (_OwnedClient) Client.Dispose();")
					.AppendLine("}\n");

					sb.AppendLine($"public {typeName}(bool sharedClient, IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> client) {{")
					.AppendLine("\t_OwnedClient = !sharedClient;")
					.AppendLine("\tClient = client;")
					.AppendLine("}\n");

					sb.AppendLine($"public {typeName}(IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> client): this(true, client) {{}}\n");

					sb.AppendLine($@"public {typeName}(): this(true, {apiDefinition.HttpClientProxy.FullyQualifiedNameWithGlobal}.Create(""{apiDefinition.BaseAddress}"")) {{}}")
					.Append('\n');

					sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create(string baseAddress) => new(true, {apiDefinition.HttpClientProxy.FullyQualifiedNameWithGlobal}.Create(baseAddress));")
						.AppendLine("")
						.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create(IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> client) => new(true, client);")
						.AppendLine("")
						.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]")
						.AppendLine($@"public static {typeNameFullyQualifiedNameWithGlobal} Create(bool sharedClient, IHttpClientProxy<{apiDefinition.TResponse}, {apiDefinition.THttpClientProxy}> client) => new(sharedClient, client);");


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

								GenerateAPIContext(sb, apiDefinition, method, hasMetadata);

								GenerateOnError(sb, method, cancellationTokenParam, () => {
									sb.AppendLine("using var __urlBuilder = ZString.CreateStringBuilder();");
									if (string.IsNullOrEmpty(method.BaseAddress)) {
										sb.AppendLine("__urlBuilder.Append(Client.BaseAddress);");
									} else {
										sb.AppendLine($@"__urlBuilder.Append(""{method.BaseAddress}"");");
									}


									GenerateParams(sb, method.PathSegments?.ToArray(), parameters);
									GenerateQueryString(sb, parameters);
									sb.AppendLine("__context.URL = __urlBuilder.ToString();");
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
											// TODO
											sb.AppendLine("return __respBody;");
										} else if (method.ReturnsIAsyncEnumerableT) {
											// TODO
										} else {
											sb.AppendLine("return __respBody;");
										}
									}
								});

							});
						});
					}
				}), withContainingType, apiDefinition.TypeInfo.IsInterface ? typeName : null, "IDisposable");
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

	public static void GenerateOnError(SourceBuilder sb, ApiDefinitionMethodInfo method, (ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam, Action fill) {
		if (method.OnError is not null) {
			sb.AppendLine("try {");
		}
		sb.CreateIndent(_ => fill?.Invoke());
		if (method.OnError is HookInfo onerror) {
			sb.AppendLine("} catch(Exception e) {");
			sb.CreateIndent(sb => {
				sb.Append($"await {onerror.FullyQualifiedNameWithGlobal}.OnError(e, in __context, {(cancellationTokenParam is {} token
					? token.paramInfo.Name
					: "CancellationToken.None")})", true);
				if (!onerror.ConfigureAwait) {
					sb.Append($".ConfigureAwait(false)");
				}
				sb.Append(";\n");
				sb.AppendLine("throw;");
			});
			sb.AppendLine('}');
		}
	}

	public static bool GenerateMetadata(SourceBuilder sb, ApiDefinitionMethodInfo method, IEnumerable<(ParameterInfo paramInfo, ApiParameterExtraInfo extraInfo)> parameters) {
		var dynamicMetadata = parameters
			.Where(static param => param.extraInfo is MetadataParameter)
			.Select(static param => (((MetadataParameter)param.extraInfo).Key, param.paramInfo.Name, param.paramInfo.Type.IsNullable))
			.ToArray();
		var capacity = dynamicMetadata.Length;

		if (method.StaticMetadata.IsEmpty && dynamicMetadata.Length == 0) {
			return false;
		}

		capacity += method.StaticMetadata.Length;
		if (dynamicMetadata.Any(static m => m.IsNullable)) {
			sb.AppendLine($"var __metaData = new Dictionary<string, object?>({capacity}) {{");
		} else {
			sb.AppendLine($"var __metaData = new Dictionary<string, object>({capacity}) {{");
		}
		sb.CreateIndent(sb => {
			if (!method.StaticMetadata.IsEmpty) {
				foreach (var (key, value) in method.StaticMetadata) {
					sb.AppendLine($@"{{ ""{key}"", ""{value}"" }},");
				}
			}
			foreach (var (key, value, _) in dynamicMetadata) {
				sb.AppendLine($@"{{ ""{key}"", {value} }},");
			}
		});
		sb.AppendLine("};\n");
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
						sb.AppendLine($@"__urlBuilder.Append({serializer.FullyQualifiedNameWithGlobal}.{(shouldEncode ? "Encode(" : "Serialize(")}""{key}"", {param.Expr}));");
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
		var nonNullableSingleHeaders = singleHeaders.Where(static h => !h.IsNullable).ToArray();

		sb.AppendLine($"var __headers = new Dictionary<string, string>({capacity}, StringComparer.OrdinalIgnoreCase) {{");
		sb.CreateIndent(sb => {
			if (!method.StaticHeaders.IsEmpty) {
				foreach (var (key, value) in method.StaticHeaders) {
					sb.AppendLine($@"{{ ""{key}"", ""{value}"" }},");
				}
			}
			foreach (var (key, value, _, _, _) in nonNullableSingleHeaders) {
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
	public static void GenerateAPIContext(SourceBuilder sb,
		ApiDefinitionTypeInfo apiDefinition,
		ApiDefinitionMethodInfo method,
		bool hasMetadata) {
		sb.AppendLine($"var __context = new APIContext(");
		sb.CreateIndent(sb => {
			sb.AppendLine($"id: __requestId,");
			sb.AppendLine($"client: Client,");
			sb.AppendLine($@"methodName: ""{method.MethodInfo.Name}"",");
			sb.AppendLine($@"className: ""{apiDefinition.TypeInfo.MinimallyQualifiedName}"",");
			sb.AppendLine($@"fullName: ""{apiDefinition.TypeInfo.FullyQualifiedName}"",");
			sb.AppendLine($"httpMethod: {method.HttpMethod},");
			// sb.AppendLine($"options: __requestId");
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
				: "CancellationToken.None")})", true);
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
		if (!method.ReturnsRawResponse && !method.ReturnsStream && !method.ReturnsVoid) {
			sb.Append($"var __resp = await Client.SendAsync{(bodyParam is null ? "<global::System.Net.Http.HttpContent>" : "")}(in __context, {(hasHeaders ? "__headers" : "null")}, {(bodyParam is null ? "null" : "__body")}, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "CancellationToken.None")})", true);
		} else {
			sb.Append($"using var __resp = await Client.SendAsync{(bodyParam is null ? "<global::System.Net.Http.HttpContent>" : "")}(in __context, {(hasHeaders ? "__headers" : "null")}, {(bodyParam is null ? "null" : "__body")}, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "CancellationToken.None")})", true);
		}
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
				: "CancellationToken.None")})", true);
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
				: "CancellationToken.None")})", true);
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
				: "CancellationToken.None")})", true);
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
		if (method.ReturnsVoid) {
			return;
		}

		sb.Append($@"var __respBody = await {
			method.ResponseBodyDeserializer.FullyQualifiedNameWithGlobal
			}.Parse<{
				method.ResponseType!.FullyQualifiedNameWithGlobal
				}>(__resp, new(""{method.ResponseType.MinimallyQualifiedName}"", ""{method.MethodInfo.ReturnType.MinimallyQualifiedName}"", {(
					method.ResponseTypeKind switch {
						ReturnTypeKind.Other => "ReturnTypeKind.Other",
						ReturnTypeKind.RawResponse => "ReturnTypeKind.RawResponse",
						ReturnTypeKind.Stream => "ReturnTypeKind.Stream",
						ReturnTypeKind.SystemString => "ReturnTypeKind.SystemString",
						ReturnTypeKind.Void => "ReturnTypeKind.Void",
						_ => "",
					})}, {(
						method.AsyncReturnTypeKind switch {
							ReturnTypeWrapperKind.CustomTask => "ReturnTypeWrapperKind.CustomTask",
							ReturnTypeWrapperKind.IAsyncEnumerableT => "ReturnTypeWrapperKind.IAsyncEnumerableT",
							ReturnTypeWrapperKind.IObservableT => "ReturnTypeWrapperKind.IObservableT",
							ReturnTypeWrapperKind.NotAsync => "ReturnTypeWrapperKind.NotAsync",
							ReturnTypeWrapperKind.Task => "ReturnTypeWrapperKind.Task",
							ReturnTypeWrapperKind.TaskT => "ReturnTypeWrapperKind.TaskT",
							ReturnTypeWrapperKind.ValueTask => "ReturnTypeWrapperKind.ValueTask",
							ReturnTypeWrapperKind.ValueTaskT => "ReturnTypeWrapperKind.ValueTaskT",
							ReturnTypeWrapperKind.Void => "ReturnTypeWrapperKind.Void",
							_ => ""
						}
					)}), in __context, {(method.ResponseBodyDeserializer.JsonContext is null
				? "null" : $"{method.ResponseBodyDeserializer.JsonContext}.Default")}, {(cancellationTokenParam is {} token
			? token.paramInfo.Name
			: "CancellationToken.None")})", true);
		if (!method.ResponseBodyDeserializer.ConfigureAwait) {
			sb.Append($".ConfigureAwait(false)");
		}
		sb.Append(";\n");

		if (method.ResponseType.NullableAnnotation != NullableAnnotation.Annotated
			&& !method.ResponseType.IsNullable) {
			sb.AppendLine("if (__respBody is null) {");
			sb.CreateIndent(sb => {
				sb.AppendLine("ResponseContentNullException.Throw();");
			});
			sb.AppendLine("}\n");
		}

	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GenerateOnSuccess(SourceBuilder sb,
		ApiDefinitionMethodInfo method,
		(ParameterInfo paramInfo, CancellationTokenParameter extraInfo)? cancellationTokenParam
		) {
		if (method.OnSuccess is HookInfo onSuccess) {
			sb.Append($"await {onSuccess.FullyQualifiedNameWithGlobal}.OnSuccess(__respBody, in __context, {(cancellationTokenParam is {} token
				? token.paramInfo.Name
				: "CancellationToken.None")})", true);
			if (!onSuccess.ConfigureAwait) {
				sb.Append($".ConfigureAwait(false)");
			}
			sb.Append(";\n");
		}
	}
}