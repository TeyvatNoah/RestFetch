using System.Text.Json.Serialization;

namespace RestFetch;

public interface IRequestBodySerializer<TRequestBodyContent> {
	static abstract ValueTask<TRequestBodyContent> Serialize<TRequestBody>(TRequestBody? value, JsonSerializerContext? jsonContext, ApiContext context, CancellationToken cancellationToken);
	// static abstract ReadOnlySpan<byte> SerializeUTF8String(T? value);;
}

public interface IRequestBodySerializer<in TRequestBody, TRequestBodyContent> {
	static abstract ValueTask<TRequestBodyContent> Serialize(TRequestBody? value, JsonSerializerContext? jsonContext, ApiContext context, CancellationToken cancellationToken);
	// static abstract ReadOnlySpan<byte> SerializeUTF8String(T? value);;
}

public interface IRequestBodySerializer: IRequestBodySerializer<HttpContent>;

// public interface IRequestBodyAsyncSerializer<in T> {
// 	#pragma warning disable CA1000
// 	static abstract ValueTask<HttpContent> SerializeAsync(T value, HttpRequestMessage req, ApiContext info, CancellationToken cancellationToken, out object? bufferedData);
// 	// static abstract ReadOnlySpan<byte> SerializeUTF8String(T? value);
// 	#pragma warning restore CA1000
// }
