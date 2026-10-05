using System.Text.Json.Serialization;

namespace RestFetch;

public interface IRequestBodySerializer<TRequestBodyContent, TResponse, TProxy>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TResponse, TProxy> {
	#pragma warning disable CA1000
	static abstract ValueTask<TRequestBodyContent> Serialize<TRequestBody>(TRequestBody? value, JsonSerializerContext? jsonContext, ref readonly APIContext<TResponse, TProxy> context, CancellationToken cancellationToken);
	// static abstract ReadOnlySpan<byte> SerializeUTF8String(T? value);;
	#pragma warning restore CA1000
}

public interface IRequestBodySerializer<TRequestBody, TRequestBodyContent, TResponse, TProxy>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TResponse, TProxy> {
	#pragma warning disable CA1000
	static abstract ValueTask<TRequestBodyContent> Serialize(TRequestBody? value, JsonSerializerContext? jsonContext, ref readonly APIContext<TResponse, TProxy> context, CancellationToken cancellationToken);
	// static abstract ReadOnlySpan<byte> SerializeUTF8String(T? value);;
	#pragma warning restore CA1000
}

public interface IRequestBodySerializer: IRequestBodySerializer<HttpContent, HttpResponseMessage, HttpClientProxy>;

// public interface IRequestBodyAsyncSerializer<in T> {
// 	#pragma warning disable CA1000
// 	static abstract ValueTask<HttpContent> SerializeAsync(T value, HttpRequestMessage req, APIContext info, CancellationToken cancellationToken, out object? bufferedData);
// 	// static abstract ReadOnlySpan<byte> SerializeUTF8String(T? value);
// 	#pragma warning restore CA1000
// }
