using System.Text.Json;

namespace RestFetch;

public readonly union HttpClientProxyOptions(HttpClientProxyOptions.HttpClientProxyClientOptions, HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions) {
	public sealed record HttpClientProxyMessageHandlerOptions(
		HttpMessageHandler Handler,
		JsonSerializerOptions? SerializerOptions = default,
		JsonSerializerOptions? DeserializerOptions = default,
		bool SharedHandler = false
	);

	public sealed record HttpClientProxyClientOptions(
		HttpClient Client,
		JsonSerializerOptions? SerializerOptions = default,
		JsonSerializerOptions? DeserializerOptions = default,
		bool SharedClient = false
	);
}