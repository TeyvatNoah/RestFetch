using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;

using RestFetch.WebUtilities;

namespace RestFetch;

public sealed class RequestBodySerializer {
	// JsonContent默认UTF8
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
	[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonSerializer))]
	private static readonly JsonSerializerOptions _options = new() {
#if DEBUG
		WriteIndented = true, // 仅开发环境
#endif
		IncludeFields = true,
		Converters = {
			new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) // 枚举序列化为字符串,默认是数字,根据情况使用,AOT仅支持JsonStringEnumConverter<TEnum>
		},
		// 不支持源生成器
		Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs, UnicodeRanges.Hiragana, UnicodeRanges.Katakana, UnicodeRanges.CjkSymbolsandPunctuation),
		RespectNullableAnnotations = true, // 全局配置了这里就不用了
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
		ReferenceHandler = ReferenceHandler.IgnoreCycles,
		TypeInfoResolver = null,
	};

	private static readonly JsonSerializerOptions _fastPathOptions = new() {
#if DEBUG
		WriteIndented = true, // 仅开发环境
#endif
		IncludeFields = true,
		RespectNullableAnnotations = true, // 全局配置了这里就不用了
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
		ReferenceHandler = ReferenceHandler.IgnoreCycles,
		TypeInfoResolver = null,
	};

	public sealed class Json: IRequestBodySerializer {
		// Stream
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent> Serialize<TRequestBody>(TRequestBody? value,
			JsonSerializerContext? jsonContext,
			ref readonly ApiContext context,
			CancellationToken cancellationToken) {
			if (jsonContext is not null) {
				var jsonSerializerOptions = ((HttpClientProxyOptions?)context.Options)?.SerializerOptions ?? _fastPathOptions;
				var jsonTypeInfo = ((IJsonTypeInfoResolver)jsonContext).GetTypeInfo(typeof(TRequestBody), jsonSerializerOptions);
				if (jsonTypeInfo is not null) {
					return ValueTask.FromResult<HttpContent>(JsonContent.Create(value, jsonTypeInfo, new("application/json")));
				}
			}
			return ValueTask.FromResult<HttpContent>(JsonContent.Create(value, new("application/json"),
					((HttpClientProxyOptions?)context.Options)?.SerializerOptions ?? _options));
		}
	}

	public sealed class JsonBuffered: IRequestBodySerializer {
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent> Serialize<TRequestBody>(TRequestBody? value,
		JsonSerializerContext? jsonContext,
		ref readonly ApiContext context,
		CancellationToken cancellationToken) {
			byte[] bytes = default!;

			if (jsonContext is not null) {
				var jsonSerializerOptions = ((HttpClientProxyOptions?)context.Options)?.SerializerOptions ?? _fastPathOptions;
				var jsonTypeInfo = ((IJsonTypeInfoResolver)jsonContext).GetTypeInfo(typeof(TRequestBody), jsonSerializerOptions);
				if (jsonTypeInfo is not null) {
					bytes = JsonSerializer.SerializeToUtf8Bytes(value, jsonTypeInfo);
				} else {
					bytes = JsonSerializer.SerializeToUtf8Bytes(value, ((HttpClientProxyOptions?)context.Options)?.SerializerOptions ?? _options);
				}
			} else {
				bytes = JsonSerializer.SerializeToUtf8Bytes(value, ((HttpClientProxyOptions?)context.Options)?.SerializerOptions ?? _options);
			}

			return ValueTask.FromResult<HttpContent>(new ByteArrayContent(bytes) {
				Headers = {
					ContentType = new("application/json")
				}
			});
		}
	}

	public sealed class Form: IRequestBodySerializer<IEnumerable<KeyValuePair<string, string?>>, HttpContent> {
		// NOTE: 仅允许IEnumerable<KeyValuePair<string, string?>>类型
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent> Serialize(IEnumerable<KeyValuePair<string, string?>>? value,
			JsonSerializerContext? jsonContext,
			ref readonly ApiContext context,
			CancellationToken cancellationToken) {
			var str = value is null ? "" : QueryString<FormEncoder>.Serialize(value);
			return ValueTask.FromResult<HttpContent>(new StringContent(str, Encoding.ASCII, new MediaTypeHeaderValue("application/x-www-form-urlencoded")));
		}
	}

	public sealed class ByteArray: IRequestBodySerializer<byte[], HttpContent?> {
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent?> Serialize(byte[]? value,
			JsonSerializerContext? jsonContext,
			ref readonly ApiContext context,
			CancellationToken cancellationToken)
				=> ValueTask.FromResult<HttpContent?>(value is null ? null : new ByteArrayContent(value));
	}

	public sealed class ByteMemory: IRequestBodySerializer<ReadOnlyMemory<byte>, HttpContent> {
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent> Serialize(ReadOnlyMemory<byte> value,
			JsonSerializerContext? jsonContext,
			ref readonly ApiContext context,
			CancellationToken cancellationToken)
				=> ValueTask.FromResult<HttpContent>(new ReadOnlyMemoryContent(value));
	}

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
	public sealed class Stream: IRequestBodySerializer<System.IO.Stream, HttpContent?> {
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent?> Serialize(System.IO.Stream? value,
			JsonSerializerContext? jsonContext,
			ref readonly ApiContext context,
			CancellationToken cancellationToken)
				=> ValueTask.FromResult<HttpContent?>(value is null ? null : new StreamContent(value));
	}

	public sealed class SystemString: IRequestBodySerializer<string, HttpContent?> {
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent?> Serialize(string? value,
			JsonSerializerContext? jsonContext,
			ref readonly ApiContext context,
			CancellationToken cancellationToken)
				=> ValueTask.FromResult<HttpContent?>(value is null ? null : new StringContent(value));
	}

	public sealed class MultipartFormData: IRequestBodySerializer<HttpContent?> {
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent?> Serialize<TRequestBody>(TRequestBody? value,
			JsonSerializerContext? jsonContext,
			ref readonly ApiContext context,
			CancellationToken cancellationToken) {
			MultipartFormDataContent? content = null;
			if (value is null) {
				content = null;
				return ValueTask.FromResult<HttpContent?>(null);
			} else if (value is FileInfo file) {
				content = new() {
					{
						// NOTE: 这里value.OpenRead()的Stream会在HttpContent被Dispose()的时候跟着Dispose()
						new StreamContent(file.OpenRead()),
						Path.GetFileNameWithoutExtension(file.Name),
						file.Name
					}
				};
			} else if (value is RestFetchFileInfo fileinfo) {
				content = new() {
					{
						// NOTE: 这里value.OpenRead()的Stream会在HttpContent被Dispose()的时候跟着Dispose()
						new StreamContent(fileinfo.File.OpenRead()),
						fileinfo.Name,
						fileinfo.FileName
					}
				};
			} else if (value is IEnumerable<FileInfo> files) {
				content = [];
				foreach (var f in files) {
					content.Add(new StreamContent(f.OpenRead()), Path.GetFileNameWithoutExtension(f.Name), f.Name);
				}
				return ValueTask.FromResult<HttpContent?>(content);
			} else if (value is IEnumerable<RestFetchFileInfo> fileInfos) {
				content = [];
				foreach (var f in fileInfos) {
					content.Add(new StreamContent(f.File.OpenRead()), f.Name, f.FileName);
				}
				return ValueTask.FromResult<HttpContent?>(content);
			} else {
				ThrowArgException();
			}

			return ValueTask.FromResult<HttpContent?>(content);

			[DoesNotReturn]
			static void ThrowArgException()
				=> throw new ArgumentException("The HTTP body parameters are expected to be of type FileInfo, RestFetchFileInfo, IEnumerable<FileInfo> or IEnumerable<RestFetchFileInfo>.");
		}
	}

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
	public sealed class OctetStream: IRequestBodySerializer<System.IO.Stream, HttpContent?> {
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent?> Serialize(System.IO.Stream? value,
			JsonSerializerContext? jsonContext,
			ref readonly ApiContext context,
			CancellationToken cancellationToken)
				=> ValueTask.FromResult<HttpContent?>(value is null
				? null
				// NOTE: 这里value.OpenRead()的Stream会在HttpContent被Dispose()的时候跟着Dispose()
				: new StreamContent(value) {
					Headers = {
						ContentType = new("application/octet-stream")
					}
				});
	}

	public sealed class Raw: IRequestBodySerializer<HttpContent, HttpContent?> {
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent?> Serialize(HttpContent? value,
			JsonSerializerContext? jsonContext,
			ref readonly ApiContext context,
			CancellationToken cancellationToken) => ValueTask.FromResult(value);
	}
}

