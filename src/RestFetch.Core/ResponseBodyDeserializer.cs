using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestFetch;

public sealed class ResponseBodyDeserializer {
	// JsonContent默认UTF8
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
	[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonSerializer))]
	private static readonly JsonSerializerOptions _options = new() {
		IncludeFields = true,
		RespectRequiredConstructorParameters = true, // 全局配置了这里就不用了
		RespectNullableAnnotations = true, // 全局配置了这里就不用了
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
		UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
		Converters = {
			new JsonStringEnumConverter(JsonNamingPolicy.CamelCase), // AOT仅支持JsonStringEnumConverter<TEnum>
			new EmptyStringToBoolConverter(),
			new EmptyStringToByteConverter(),
			new EmptyStringToSByteConverter(),
			new EmptyStringToShortConverter(),
			new EmptyStringToUshortConverter(),
			new EmptyStringToIntConverter(),
			new EmptyStringToUintConverter(),
			new EmptyStringToLongConverter(),
			new EmptyStringToUlongConverter(),
			new EmptyStringToDecimalConverter(),
			new EmptyStringToFloatConverter(),
			new EmptyStringToDoubleConverter(),
		}
	};


	[DoesNotReturn]
	private static void ThrowNotSupportedException(string? mime) {
		throw new NotSupportedException($"The response content type '{mime?.ToString() ?? "<missing>"}' is not supported. Supported content types are 'application/json' and 'text/event-stream'.");
	}

	public sealed class Json: IResponseBodyDeserializer {
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HttpContentJsonExtensions))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Task<TResponseBody?> Parse<TResponseBody>(HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			CancellationToken cancellationToken) {
			if (resp.Content.Headers.ContentType?.MediaType != "application/json") {
				ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
			}

			if (resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0) {
				return Task.FromResult<TResponseBody?>(default);
			}

			var options = new JsonSerializerOptions(_options) {
				TypeInfoResolver = jsonContext
			};
			return resp.Content.ReadFromJsonAsync<TResponseBody?>(options, cancellationToken);
		}
	}

	public sealed class JsonNode: IResponseBodyDeserializer<HttpResponseMessage, System.Text.Json.Nodes.JsonNode> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Task<System.Text.Json.Nodes.JsonNode?> Parse(HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			CancellationToken cancellationToken) {
			if (resp.Content.Headers.ContentType?.MediaType != "application/json") {
				ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
			}

			return resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0
				? Task.FromResult<System.Text.Json.Nodes.JsonNode?>(default)
				: ParseInternal(resp, cancellationToken);

			static async Task<System.Text.Json.Nodes.JsonNode?> ParseInternal(HttpResponseMessage res, CancellationToken token) {
				var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
				return await System.Text.Json.Nodes.JsonNode.ParseAsync(stream, new() {
					PropertyNameCaseInsensitive = true
				}, new() {
					AllowDuplicateProperties = false,
					AllowTrailingCommas = false,
					CommentHandling = JsonCommentHandling.Skip,
					MaxDepth = 64
				}, token).ConfigureAwait(false);
			}
		}
	}

	public sealed class JsonDocument: IResponseBodyDeserializer<HttpResponseMessage, System.Text.Json.JsonDocument> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Task<System.Text.Json.JsonDocument?> Parse(HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			CancellationToken cancellationToken) {
			if (resp.Content.Headers.ContentType?.MediaType != "application/json") {
				ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
			}
			return resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0
				? Task.FromResult<System.Text.Json.JsonDocument?>(default)
				: ParseInternal(resp, cancellationToken);

			static async Task<System.Text.Json.JsonDocument?> ParseInternal(HttpResponseMessage res, CancellationToken token) {
				var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
				return await System.Text.Json.JsonDocument.ParseAsync(stream, new() {
					AllowDuplicateProperties = false,
					AllowTrailingCommas = false,
					CommentHandling = JsonCommentHandling.Skip,
					MaxDepth = 64
				}, token).ConfigureAwait(false);
			}
		}
	}

	public sealed class SystemString: IResponseBodyDeserializer<HttpResponseMessage, string> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Task<string?> Parse(HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			CancellationToken cancellationToken)
				=> resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0
					? Task.FromResult<string?>(default)
					: resp.Content.ReadAsStringAsync(cancellationToken) as Task<string?>;
	}

	public sealed class ApiResponse<T>: IResponseBodyDeserializer<HttpResponseMessage, RestFetch.ApiResponse<T>> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
		public static Task<RestFetch.ApiResponse<T?>> Parse(
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
			HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			CancellationToken cancellationToken) {
				if (resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0) {
					return Task.FromResult<RestFetch.ApiResponse<T?>>(new(
						StatusCode: resp.StatusCode,
						ReasonPhrase: resp.ReasonPhrase,
						Url: context.Url,
						Content: default
					));
				}

				if (!returnTypeInfo.SkipNullableCheck && !returnTypeInfo.Nullable && !returnTypeInfo.ValueType) {
					ReturnTypeException.Throw();
				}

				if (typeof(T) == typeof(string)) {
					return ParseAsString(resp, context.Url, cancellationToken);
				} else if (typeof(T) == typeof(System.Text.Json.Nodes.JsonNode)) {
					return ParseAsJsonNode(resp, context.Url, cancellationToken);
				} else {
					if (resp.Content.Headers.ContentType?.MediaType != "application/json") {
						ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
					}

					var opts = new JsonSerializerOptions(_options) {
						TypeInfoResolver = jsonContext
					};

					return ParseAsJson(resp, context.Url, opts, cancellationToken);
				}

				[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
				[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
				[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HttpContentJsonExtensions))]
				static async Task<RestFetch.ApiResponse<T?>> ParseAsJson(HttpResponseMessage res, string url, JsonSerializerOptions options, CancellationToken token) {
					var contentObj = await res.Content.ReadFromJsonAsync<T>(options, token).ConfigureAwait(false);
					return new(
						StatusCode: res.StatusCode,
						ReasonPhrase: res.ReasonPhrase,
						Url: url,
						Content: contentObj
					);
				}

				static async Task<RestFetch.ApiResponse<T?>> ParseAsJsonNode(HttpResponseMessage res, string url, CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					return new(
						StatusCode: res.StatusCode,
						ReasonPhrase: res.ReasonPhrase,
						Url: url,
						Content: (T?)(object?)await System.Text.Json.Nodes.JsonNode.ParseAsync(stream, new() {
							PropertyNameCaseInsensitive = true
						}, new() {
							AllowDuplicateProperties = false,
							AllowTrailingCommas = false,
							CommentHandling = JsonCommentHandling.Skip,
							MaxDepth = 64
						}, token).ConfigureAwait(false)
					);
				}

				static async Task<RestFetch.ApiResponse<T?>> ParseAsString(HttpResponseMessage res, string url, CancellationToken token) {
					var contentStr = await res.Content.ReadAsStringAsync(token).ConfigureAwait(false);
					return new(
						StatusCode: res.StatusCode,
						ReasonPhrase: res.ReasonPhrase,
						Url: url,
						// NOTE: 也可以把返回类型改为Task<RestFetch.ApiResponse<string?>>
						// 而在调用点处做类型转换,但是这里转换比调用点处转换更好,因为Task可能有快速路径优化而避免堆分配
						// Task强转object可能破坏快速路径优化,而这里string转object就没这种问题
						Content: (T)(object)contentStr
					);
				}
			}
	}

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
	public sealed class Stream: IResponseBodyDeserializer<HttpResponseMessage, System.IO.Stream> {
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Task<System.IO.Stream?> Parse(HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			CancellationToken cancellationToken) {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
				return resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0
					? Task.FromResult<System.IO.Stream?>(default)
					: ParseInternal(resp, cancellationToken);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

				static async Task<System.IO.Stream> ParseInternal(HttpResponseMessage res, CancellationToken token) {
					return new RestFetchResponseStream(res, await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false));
				}
			}
		}

	public sealed class SseOrJsonArray: IResponseAsyncEnumerableDeserializer {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IAsyncEnumerable<SseEventResult<T?>> Parse<T>(
			HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			SseEventExceptionHandler? handler,
			CancellationToken cancellationToken) {
				if (resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0) {
					return AsyncEnumerable.Empty<SseEventResult<T?>>();
				} else if (resp.Content.Headers.ContentType?.MediaType == "text/event-stream") {
					if (!returnTypeInfo.SkipNullableCheck && !returnTypeInfo.Nullable && !returnTypeInfo.ValueType) {
						ReturnTypeException.Throw();
					}

					if (typeof(T) == typeof(string)) {
						return (IAsyncEnumerable<SseEventResult<T?>>)(object)ParseAsString(resp, cancellationToken);
					} else if (typeof(T) == typeof(System.Text.Json.Nodes.JsonNode)) {
						return (IAsyncEnumerable<SseEventResult<T?>>)(object)ParseAsJsonNode(resp, new(context.Id, context.Url), handler, cancellationToken);
					} else if (typeof(T) == typeof(System.Text.Json.JsonDocument)) {
						return (IAsyncEnumerable<SseEventResult<T?>>)(object)ParseAsJsonDocument(resp, new(context.Id, context.Url), handler, cancellationToken);
					} else if (typeof(T) == typeof(WebUtilities.SseEvent)) {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
						return (IAsyncEnumerable<SseEventResult<T>>)(object)ParseAsSseEvent(resp, cancellationToken);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
					} else {
						var options = new JsonSerializerOptions(_options) {
							TypeInfoResolver = jsonContext
						};
						return ParseAsJson<T>(resp, options, new(context.Id, context.Url), handler, cancellationToken);
					}
				} else if (resp.Content.Headers.ContentType?.MediaType == "application/json") {
					var options = new JsonSerializerOptions(_options) {
						TypeInfoResolver = jsonContext
					};
					return ParseAsJsonArray(resp, options, cancellationToken);
				} else {
					ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
					return default;
				}



				[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
				[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
				[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonSerializer))]
				static async IAsyncEnumerable<SseEventResult<T?>> ParseAsJsonArray(
					HttpResponseMessage res,
					JsonSerializerOptions opts,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<T>(stream, opts, token)
						.ConfigureAwait(false)) {
							yield return new SseEventResult<T?>.Ok(item);
						}
					}
				}

				static async IAsyncEnumerable<SseEventResult<string?>> ParseAsString(
					HttpResponseMessage res,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create(stream).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return new SseEventResult<string?>.Ok(item.Data);
					}
				}

				static async IAsyncEnumerable<SseEventResult<System.Text.Json.Nodes.JsonNode?>> ParseAsJsonNode(
					HttpResponseMessage res,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create<SseEventResult<System.Text.Json.Nodes.JsonNode?>>(
						stream,
						(eventType, data) => {
								try {
									return new SseEventResult<System.Text.Json.Nodes.JsonNode?>.Ok(System.Text.Json.Nodes.JsonNode.Parse(
										data,
										new() {
											PropertyNameCaseInsensitive = true
										},
										new() {
											AllowDuplicateProperties = false,
											AllowTrailingCommas = false,
											CommentHandling = JsonCommentHandling.Skip,
											MaxDepth = 64
									}));
								} catch(JsonException e) {
									var skipErrorResponse = handler?.Invoke(e, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
									return new SseEventResult<System.Text.Json.Nodes.JsonNode?>.Err(
										Error: e,
										Skip: skipErrorResponse
									);
								}
							}).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return item.Data;
					}
				}

				static async IAsyncEnumerable<SseEventResult<System.Text.Json.JsonDocument?>> ParseAsJsonDocument(
					HttpResponseMessage res,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create<SseEventResult<System.Text.Json.JsonDocument?>>(
						stream,
						(eventType, data) => {
							try {
								return new SseEventResult<System.Text.Json.JsonDocument?>.Ok(System.Text.Json.JsonDocument.Parse(
									data.ToArray(), // 无法零堆分配和拷贝
									new() {
										AllowDuplicateProperties = false,
										AllowTrailingCommas = false,
										CommentHandling = JsonCommentHandling.Skip,
										MaxDepth = 64
									}));
							} catch(JsonException e) {
								var skipErrorResponse = handler?.Invoke(e, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<System.Text.Json.JsonDocument?>.Err(
									Error: e,
									Skip: skipErrorResponse
								);
							}
						}).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return item.Data;
					}
				}

				static async IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent>> ParseAsSseEvent(
					HttpResponseMessage res,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create(stream).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return new SseEventResult<WebUtilities.SseEvent>.Ok(new(
							id: item.EventId,
							@event: item.EventType,
							retry: item.ReconnectionInterval,
							data: item.Data
						));
					}
				}

				[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
				[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
				[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonSerializer))]
				static async IAsyncEnumerable<SseEventResult<T?>> ParseAsJson<T>(
					HttpResponseMessage res,
					JsonSerializerOptions opts,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser
						.Create<SseEventResult<T?>>(stream, (_, data) => {
							try {
								return new SseEventResult<T?>.Ok(JsonSerializer.Deserialize<T>(data, opts));
							} catch(JsonException e0) {
								var skipErrorResponse = handler?.Invoke(e0, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<T?>.Err(
									Error: e0,
									Skip: skipErrorResponse
								);
							} catch(NotSupportedException e1) {
								var skipErrorResponse = handler?.Invoke(e1, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<T?>.Err(
									Error: e1,
									Skip: skipErrorResponse
								);
							}
						})
						.EnumerateAsync(token)) {
						yield return item.Data;
					}
				}
		}

	public sealed class SseString: IResponseAsyncEnumerableDeserializer<HttpResponseMessage, string> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
		public static IAsyncEnumerable<SseEventResult<string>> Parse(
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
			HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			SseEventExceptionHandler? handler,
			CancellationToken cancellationToken) {
				if (resp.Content.Headers.ContentType?.MediaType != "text/event-stream") {
					ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
				}
				return resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0
				? AsyncEnumerable.Empty<SseEventResult<string>>()
				: ParseInternal(resp, cancellationToken);

				static async IAsyncEnumerable<SseEventResult<string>> ParseInternal(
					HttpResponseMessage res,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create(stream).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return new SseEventResult<string>.Ok(item.Data);
					}
				}
			}
	}


	public sealed class SseJsonNode: IResponseAsyncEnumerableDeserializer<HttpResponseMessage, System.Text.Json.Nodes.JsonNode?> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IAsyncEnumerable<SseEventResult<System.Text.Json.Nodes.JsonNode?>> Parse(
			HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			SseEventExceptionHandler? handler,
			CancellationToken cancellationToken) {
				if (resp.Content.Headers.ContentType?.MediaType != "text/event-stream") {
					ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
				}
				return resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0
				? AsyncEnumerable.Empty<SseEventResult<System.Text.Json.Nodes.JsonNode?>>()
				: ParseInternal(resp, new(context.Id, context.Url), handler, cancellationToken);

				static async IAsyncEnumerable<SseEventResult<System.Text.Json.Nodes.JsonNode?>> ParseInternal(
					HttpResponseMessage res,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create<SseEventResult<System.Text.Json.Nodes.JsonNode?>>(
						stream,
						(eventType, data) => {
							try {
								return new SseEventResult<System.Text.Json.Nodes.JsonNode?>.Ok(System.Text.Json.Nodes.JsonNode.Parse(
									data,
									new() {
										PropertyNameCaseInsensitive = true
									},
									new() {
										AllowDuplicateProperties = false,
										AllowTrailingCommas = false,
										CommentHandling = JsonCommentHandling.Skip,
										MaxDepth = 64
									}));
							} catch(JsonException e) {
								var skipErrorResponse = handler?.Invoke(e, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<System.Text.Json.Nodes.JsonNode?>.Err(
									Error: e,
									Skip: skipErrorResponse
								);
							}
						}).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return item.Data;
					}
				}
			}
	}

	public sealed class SseJsonDocument: IResponseAsyncEnumerableDeserializer<HttpResponseMessage, System.Text.Json.JsonDocument> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
		public static IAsyncEnumerable<SseEventResult<System.Text.Json.JsonDocument>> Parse(
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
			HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			SseEventExceptionHandler? handler,
			CancellationToken cancellationToken) {
				if (resp.Content.Headers.ContentType?.MediaType != "text/event-stream") {
					ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
				}
				return resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0
				? AsyncEnumerable.Empty<SseEventResult<System.Text.Json.JsonDocument>>()
				: ParseInternal(resp, new(context.Id, context.Url), handler, cancellationToken);

				static async IAsyncEnumerable<SseEventResult<System.Text.Json.JsonDocument>> ParseInternal(
					HttpResponseMessage res,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create<SseEventResult<System.Text.Json.JsonDocument>>(
						stream,
						(eventType, data) => {
							try {
								return new SseEventResult<System.Text.Json.JsonDocument>.Ok(System.Text.Json.JsonDocument.Parse(
									data.ToArray(), // 无法零堆分配和拷贝
									new() {
										AllowDuplicateProperties = false,
										AllowTrailingCommas = false,
										CommentHandling = JsonCommentHandling.Skip,
										MaxDepth = 64
									}));
							} catch(JsonException e) {
								var skipErrorResponse = handler?.Invoke(e, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<System.Text.Json.JsonDocument>.Err(
									Error: e,
									Skip: skipErrorResponse
								);
							}
						}).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return item.Data;
					}
				}
			}
	}

	public sealed class SseEvent: IResponseAsyncEnumerableDeserializer<HttpResponseMessage, WebUtilities.SseEvent> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent>> Parse(
			HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			SseEventExceptionHandler? handler,
			CancellationToken cancellationToken) {
				if (resp.Content.Headers.ContentType?.MediaType != "text/event-stream") {
					ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
				}
				return resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0
				? AsyncEnumerable.Empty<SseEventResult<WebUtilities.SseEvent>>()
				: ParseInternal(resp, cancellationToken);

				static async IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent>> ParseInternal(
					HttpResponseMessage res,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create(stream).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return new SseEventResult<WebUtilities.SseEvent>.Ok(new(
							id: item.EventId,
							@event: item.EventType,
							retry: item.ReconnectionInterval,
							data: item.Data
						));
					}
				}
			}
		}


	public sealed class SseEvent<T>: IResponseAsyncEnumerableDeserializer<HttpResponseMessage, WebUtilities.SseEvent<T>> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent<T>>> Parse(
			HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			SseEventExceptionHandler? handler,
			CancellationToken cancellationToken) {
				if (resp.Content.Headers.ContentType?.MediaType != "text/event-stream") {
					ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
				}

				if (!returnTypeInfo.SkipNullableCheck && !returnTypeInfo.Nullable && !returnTypeInfo.ValueType) {
					ReturnTypeException.Throw();
				}

				if (resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0) {
					return AsyncEnumerable.Empty<SseEventResult<WebUtilities.SseEvent<T>>>();
				} else if (typeof(T) == typeof(string)) {
					return (IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent<T>>>)(object)ParseAsString(resp, cancellationToken);
				} else if (typeof(T) == typeof(System.Text.Json.Nodes.JsonNode)) {
					return (IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent<T>>>)(object)ParseAsJsonNode(resp, new(context.Id, context.Url), handler, cancellationToken);
				} else if (typeof(T) == typeof(System.Text.Json.JsonDocument)) {
					return (IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent<T>>>)(object)ParseAsJsonDocument(resp, new(context.Id, context.Url), handler, cancellationToken);
				} else {
					var options = new JsonSerializerOptions(_options) {
						TypeInfoResolver = jsonContext
					};
					return (IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent<T>>>)(object)ParseAsJson(resp, options, new(context.Id, context.Url), handler, cancellationToken);
				}

				static async IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent<string>>> ParseAsString(
					HttpResponseMessage res,
					[EnumeratorCancellation]CancellationToken token) {

					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create(stream).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return new SseEventResult<WebUtilities.SseEvent<string>>.Ok(new(
							id: item.EventId,
							@event: item.EventType,
							retry: item.ReconnectionInterval,
							data: item.Data
						));
					}
				}

				static async IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent<System.Text.Json.JsonDocument?>>> ParseAsJsonDocument(
					HttpResponseMessage res,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {

					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create<SseEventResult<System.Text.Json.JsonDocument?>>(
						stream,
						(eventType, data) => {
							try {
								return new SseEventResult<System.Text.Json.JsonDocument?>.Ok(System.Text.Json.JsonDocument.Parse(
									data.ToArray(), // 无法零堆分配和拷贝
									new() {
										AllowDuplicateProperties = false,
										AllowTrailingCommas = false,
										CommentHandling = JsonCommentHandling.Skip,
										MaxDepth = 64
									}));
							} catch(JsonException e) {
								var skipErrorResponse = handler?.Invoke(e, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<System.Text.Json.JsonDocument?>.Err(
									Error: e,
									Skip: skipErrorResponse
								);
							}
						}).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return item.Data switch {
							SseEventResult<System.Text.Json.JsonDocument?>.Ok(var data) => new SseEventResult<WebUtilities.SseEvent<System.Text.Json.JsonDocument?>>.Ok(new(
								id: item.EventId,
								@event: item.EventType,
								retry: item.ReconnectionInterval,
								data: data
							)),
							SseEventResult<System.Text.Json.JsonDocument?>.Err(var e, var skip) => new SseEventResult<WebUtilities.SseEvent<System.Text.Json.JsonDocument?>>.Err(
								Error: e,
								Skip: skip
							)
						};
					}
				}

				static async IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent<System.Text.Json.Nodes.JsonNode?>>> ParseAsJsonNode(
					HttpResponseMessage res,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {

					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create<SseEventResult<System.Text.Json.Nodes.JsonNode?>>(
						stream,
						(eventType, data) => {
							try {
								return new SseEventResult<System.Text.Json.Nodes.JsonNode?>.Ok(System.Text.Json.Nodes.JsonNode.Parse(
									data,
									new() {
										PropertyNameCaseInsensitive = true
									},
									new() {
										AllowDuplicateProperties = false,
										AllowTrailingCommas = false,
										CommentHandling = JsonCommentHandling.Skip,
										MaxDepth = 64
									}));
							} catch(JsonException e) {
								var skipErrorResponse = handler?.Invoke(e, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<System.Text.Json.Nodes.JsonNode?>.Err(
									Error: e,
									Skip: skipErrorResponse
								);
							}
						}).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return item.Data switch {
							SseEventResult<System.Text.Json.Nodes.JsonNode?>.Ok(var data) => new SseEventResult<WebUtilities.SseEvent<System.Text.Json.Nodes.JsonNode?>>.Ok(new(
								id: item.EventId,
								@event: item.EventType,
								retry: item.ReconnectionInterval,
								data: data
							)),
							SseEventResult<System.Text.Json.Nodes.JsonNode?>.Err(var e, var skip) => new SseEventResult<WebUtilities.SseEvent<System.Text.Json.Nodes.JsonNode?>>.Err(e, skip),
						};
					}
				}

				[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
				[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
				[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonSerializer))]
				static async IAsyncEnumerable<SseEventResult<WebUtilities.SseEvent<T?>>> ParseAsJson(
					HttpResponseMessage res,
					JsonSerializerOptions opts,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					await foreach (var item in SseParser
						.Create<SseEventResult<T?>>(stream, (_, data) => {
							try {
								return new SseEventResult<T?>.Ok(JsonSerializer.Deserialize<T>(data, opts));
							} catch(JsonException e0) {
								var skipErrorResponse = handler?.Invoke(e0, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<T?>.Err(
									Error: e0,
									Skip: skipErrorResponse
								);
							} catch(NotSupportedException e1) {
								var skipErrorResponse = handler?.Invoke(e1, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<T?>.Err(
									Error: e1,
									Skip: true
								);
							}
						})
						.EnumerateAsync(token)
						.ConfigureAwait(false)) {
							yield return item.Data switch {
								SseEventResult<T?>.Ok(var data) => new SseEventResult<WebUtilities.SseEvent<T?>>.Ok(new(
									id: item.EventId,
									@event: item.EventType,
									retry: item.ReconnectionInterval,
									data: data
								)),
								SseEventResult<T?>.Err(var e, var skip) => new SseEventResult<WebUtilities.SseEvent<T?>>.Err(e, skip),
							};
					}
				}
			}
	}

	public sealed class SseItem<T>: IResponseAsyncEnumerableDeserializer<HttpResponseMessage, System.Net.ServerSentEvents.SseItem<T>> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IAsyncEnumerable<SseEventResult<System.Net.ServerSentEvents.SseItem<T>>> Parse(
			HttpResponseMessage resp,
			ref readonly ReturnTypeInfo returnTypeInfo,
			ref readonly ApiContext context,
			JsonSerializerContext? jsonContext,
			SseEventExceptionHandler? handler,
			CancellationToken cancellationToken) {
				if (resp.Content.Headers.ContentType?.MediaType != "text/event-stream") {
					ThrowNotSupportedException(resp.Content.Headers.ContentType?.MediaType);
				}

				if (!returnTypeInfo.SkipNullableCheck && !returnTypeInfo.Nullable && !returnTypeInfo.ValueType) {
					ReturnTypeException.Throw();
				}

				if (resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0) {
					return AsyncEnumerable.Empty<SseEventResult<System.Net.ServerSentEvents.SseItem<T>>>();
				} else if (typeof(T) == typeof(string)) {
					return (IAsyncEnumerable<SseEventResult<System.Net.ServerSentEvents.SseItem<T>>>)(object)ParseAsString(resp, cancellationToken);
				} else if (typeof(T) == typeof(System.Text.Json.Nodes.JsonNode)) {
					return (IAsyncEnumerable<SseEventResult<System.Net.ServerSentEvents.SseItem<T>>>)(object)ParseAsJsonNode(resp, new(context.Id, context.Url), handler, cancellationToken);
				} else if (typeof(T) == typeof(System.Text.Json.JsonDocument)) {
					return (IAsyncEnumerable<SseEventResult<System.Net.ServerSentEvents.SseItem<T>>>)(object)ParseAsJsonDocument(resp, new(context.Id, context.Url), handler, cancellationToken);
				} else {
					var options = new JsonSerializerOptions(_options) {
						TypeInfoResolver = jsonContext
					};
					return (IAsyncEnumerable<SseEventResult<System.Net.ServerSentEvents.SseItem<T>>>)(object)ParseAsJson(resp, options, new(context.Id, context.Url), handler, cancellationToken);
				}

				static async IAsyncEnumerable<SseEventResult<System.Net.ServerSentEvents.SseItem<string>>> ParseAsString(
					HttpResponseMessage res,
					[EnumeratorCancellation]CancellationToken token) {

					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create(stream).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return new SseEventResult<System.Net.ServerSentEvents.SseItem<string>>.Ok(item);
					}
				}

				static async IAsyncEnumerable<SseEventResult<System.Net.ServerSentEvents.SseItem<System.Text.Json.JsonDocument?>>> ParseAsJsonDocument(
					HttpResponseMessage res,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {

					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create<SseEventResult<System.Text.Json.JsonDocument?>>(
						stream,
						(eventType, data) => {
							try {
								return new SseEventResult<System.Text.Json.JsonDocument?>.Ok(System.Text.Json.JsonDocument.Parse(
									data.ToArray(), // 无法零堆分配和拷贝
									new() {
										AllowDuplicateProperties = false,
										AllowTrailingCommas = false,
										CommentHandling = JsonCommentHandling.Skip,
										MaxDepth = 64
									}));
							} catch(JsonException e) {
								var skipErrorResponse = handler?.Invoke(e, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<System.Text.Json.JsonDocument?>.Err(
									Error: e,
									Skip: skipErrorResponse
								);
							}
						}).EnumerateAsync(token).ConfigureAwait(false)) {
							yield return item.Data switch {
								SseEventResult<System.Text.Json.JsonDocument?>.Ok(var data) => new SseEventResult<System.Net.ServerSentEvents.SseItem<System.Text.Json.JsonDocument?>>.Ok(new(
									data,
									item.EventType
								) {
									EventId = item.EventId,
									ReconnectionInterval = item.ReconnectionInterval
								}),
								SseEventResult<System.Text.Json.JsonDocument?>.Err(var e, var skip) => new SseEventResult<System.Net.ServerSentEvents.SseItem<System.Text.Json.JsonDocument?>>.Err(e, skip),
							};
					}
				}

				static async IAsyncEnumerable<SseEventResult<System.Net.ServerSentEvents.SseItem<System.Text.Json.Nodes.JsonNode?>>> ParseAsJsonNode(
					HttpResponseMessage res,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {

					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					// TODO 以后换成自己实现的解析器
					await foreach (var item in SseParser.Create<SseEventResult<System.Text.Json.Nodes.JsonNode?>>(
						stream,
						(eventType, data) => {
							try {
								return new SseEventResult<System.Text.Json.Nodes.JsonNode?>.Ok(System.Text.Json.Nodes.JsonNode.Parse(
									data,
									new() {
										PropertyNameCaseInsensitive = true
									},
									new() {
										AllowDuplicateProperties = false,
										AllowTrailingCommas = false,
										CommentHandling = JsonCommentHandling.Skip,
										MaxDepth = 64
									}));
							} catch(JsonException e) {
								var skipErrorResponse = handler?.Invoke(e, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<System.Text.Json.Nodes.JsonNode?>.Err(
									Error: e,
									Skip: skipErrorResponse
								);
							}
						}).EnumerateAsync(token).ConfigureAwait(false)) {
						yield return item.Data switch {
							SseEventResult<System.Text.Json.Nodes.JsonNode?>.Ok(var data) => new SseEventResult<System.Net.ServerSentEvents.SseItem<System.Text.Json.Nodes.JsonNode?>>.Ok(new(
								data,
								item.EventType
							) {
								EventId = item.EventId,
								ReconnectionInterval = item.ReconnectionInterval
							}),
							SseEventResult<System.Text.Json.Nodes.JsonNode?>.Err(var e, var skip) => new SseEventResult<System.Net.ServerSentEvents.SseItem<System.Text.Json.Nodes.JsonNode?>>.Err(e, skip),
						};
					}
				}

				[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
				[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
				[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonSerializer))]
				static async IAsyncEnumerable<SseEventResult<System.Net.ServerSentEvents.SseItem<T?>>> ParseAsJson(
					HttpResponseMessage res,
					JsonSerializerOptions opts,
					ApiMinimalContext ctx,
					SseEventExceptionHandler? handler,
					[EnumeratorCancellation]CancellationToken token) {
					var stream = await res.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
					await foreach (var item in SseParser
						.Create<SseEventResult<T?>>(stream, (_, data) => {
							try {
								return new SseEventResult<T?>.Ok(JsonSerializer.Deserialize<T>(data, opts));
							} catch(JsonException e0) {
								var skipErrorResponse = handler?.Invoke(e0, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<T?>.Err(
									Error: e0,
									Skip: skipErrorResponse
								);
							} catch(NotSupportedException e1) {
								var skipErrorResponse = handler?.Invoke(e1, ctx).ConfigureAwait(false).GetAwaiter().GetResult() ?? true;
								return new SseEventResult<T?>.Err(
									Error: e1,
									Skip: skipErrorResponse
								);
							}
						})
						.EnumerateAsync(token)
						.ConfigureAwait(false)) {
						yield return item.Data switch {
							SseEventResult<T?>.Ok(var data) => new SseEventResult<System.Net.ServerSentEvents.SseItem<T?>>.Ok(new(
								data,
								item.EventType
							) {
								EventId = item.EventId,
								ReconnectionInterval = item.ReconnectionInterval
							}),
							SseEventResult<T?>.Err(var e, var skip) => new SseEventResult<System.Net.ServerSentEvents.SseItem<T?>>.Err(e, skip),
						};
					}
				}
			}
	}
}