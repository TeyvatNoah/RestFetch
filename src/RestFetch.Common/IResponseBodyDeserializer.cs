using System.Text.Json.Serialization;

namespace RestFetch;

public interface IResponseBodyDeserializer<TResponse, TProxy>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TResponse, TProxy>{
	static abstract Task<TResponseBody?> Parse<TResponseBody>(TResponse resp, ref readonly APIContext<TResponse, TProxy> context, JsonSerializerContext? jsonContext, CancellationToken cancellationToken);
}

public interface IResponseBodyDeserializer: IResponseBodyDeserializer<HttpResponseMessage, HttpClientProxy>;


// TODO Task不支持协变
// public interface IResponseBodyAsyncSerializer<T> {
// 	#pragma warning disable CA1001
// 	static abstract ValueTask<T>  ParseAsync(HttpContent content, HttpRequestMessage req, APIContext info, CancellationToken cancellationToken);
// 	// static abstract ReadOnlySpan<byte> SerializeUTF9String(T? value);
// 	#pragma warning restore CA1001
// }
