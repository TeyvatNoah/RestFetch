using System.Text.Json;

namespace RestFetch;

public readonly union HttpClientProxyOptions(HttpClientProxyOptions.HttpClientProxyFactoryOptions, HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions) {
	public class HttpClientProxyCommonOptions {
		public JsonSerializerOptions? SerializerOptions;
		public JsonSerializerOptions? DeserializerOptions;
	}

	public sealed class HttpClientProxyMessageHandlerOptions(HttpMessageHandler handler): HttpClientProxyCommonOptions {
		public HttpMessageHandler Handler = handler;
		public bool SharedHandler;
	}

	public sealed class HttpClientProxyFactoryOptions(Func<HttpClient> factory): HttpClientProxyCommonOptions {
		public Func<HttpClient> Factory = factory;
	}
}