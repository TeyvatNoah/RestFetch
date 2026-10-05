using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

using RestFetch.WebUtilities;

namespace RestFetch;

public sealed class RequestBodySerializer {
	// JsonContent默认UTF8
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
	[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
	private static readonly JsonSerializerOptions _Options = new() {
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

	private static readonly JsonSerializerOptions _FastPathOptions = new() {
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
			ref readonly APIContext<HttpResponseMessage, HttpClientProxy> context,
			CancellationToken cancellationToken) {
			var isFastPath = jsonContext is not null;
			_FastPathOptions.TypeInfoResolver = jsonContext;
			return ValueTask.FromResult<HttpContent>(JsonContent.Create(value, new("application/json"), isFastPath ? _FastPathOptions : _Options));
		}

	}

	internal sealed class JsonBuffered: IRequestBodySerializer {
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsonContent))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent> Serialize<TRequestBody>(TRequestBody? value,
		JsonSerializerContext? jsonContext,
		ref readonly APIContext<HttpResponseMessage, HttpClientProxy> context,
		CancellationToken cancellationToken) {
			var isFastPath = jsonContext is not null;
			_FastPathOptions.TypeInfoResolver = jsonContext;
			var bytes = JsonSerializer.SerializeToUtf8Bytes(value, isFastPath ? _FastPathOptions : _Options);
			var content = new ByteArrayContent(bytes);
			content.Headers.ContentType = new("application/json");
			return ValueTask.FromResult<HttpContent>(content);
		}
	}

	public sealed class Form: IRequestBodySerializer {
		// NOTE: 仅允许IEnumerable<KeyValuePair<string, string?>>类型
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<HttpContent> Serialize<TRequestBody>(TRequestBody? value,
			JsonSerializerContext? jsonContext,
			ref readonly APIContext<HttpResponseMessage, HttpClientProxy> context,
			CancellationToken cancellationToken) {
			var str = value is null ? "" : QueryString<FormEncoder>.Serialize((IEnumerable<KeyValuePair<string, string?>>)value);
			return ValueTask.FromResult<HttpContent>(new StringContent(str, Encoding.ASCII, new MediaTypeHeaderValue("application/x-www-form-urlencoded")));
		}
	}
}

