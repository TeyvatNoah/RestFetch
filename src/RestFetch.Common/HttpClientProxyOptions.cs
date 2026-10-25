using System.Text.Json;

namespace RestFetch;

public sealed record HttpClientProxyOptions(
	HttpMessageHandler Handler,
	JsonSerializerOptions? SerializerOptions = default,
	JsonSerializerOptions? DeserializerOptions = default,
	bool SharedHandler = false
);