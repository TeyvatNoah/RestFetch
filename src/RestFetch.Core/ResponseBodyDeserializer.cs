using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestFetch;

public sealed class ResponseBodyDeserializer {
	// JsonContent默认UTF8
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
	[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HttpContentJsonExtensions))]
	private static readonly JsonSerializerOptions _Options = new() {
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

		public sealed class Json : IResponseBodyDeserializer {
			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026")]
			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050")]
			[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HttpContentJsonExtensions))]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Task<TResponseBody?> Parse<TResponseBody>(HttpResponseMessage resp,
				ref readonly APIContext<HttpResponseMessage, HttpClientProxy> context,
				JsonSerializerContext? jsonContext,
				CancellationToken cancellationToken) {
				if (resp.StatusCode == HttpStatusCode.NoContent || resp.Content.Headers.ContentLength == 0) {
					return Task.FromResult<TResponseBody?>(default);
				}
				_Options.TypeInfoResolver = jsonContext;
				return resp.Content.ReadFromJsonAsync<TResponseBody>(_Options, cancellationToken);
			}
		}
}

