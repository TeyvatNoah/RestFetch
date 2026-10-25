using System.Text.Json;

namespace RestFetch;

public sealed record HttpClientProxyOptions(
	HttpMessageHandler Handler,
	JsonSerializerOptions? SerializerOptions,
	JsonSerializerOptions? DeserializerOptions,
	bool SharedHandler = false
);