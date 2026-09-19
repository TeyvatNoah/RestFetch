using System.Text.Json.Serialization;

namespace RestFetch;

public interface IResponseBodyDeserializer<TURL, TDisposer, TResponse, TProxy>
	where TURL: IURL
	where TDisposer: IDisposer<TURL>
	where TResponse: IDisposable
	where TProxy: IHttpClientProxy<TURL, TDisposer, TResponse, TProxy>{
	static abstract Task<TResponseBody?> Parse<TResponseBody>(TResponse resp, ref readonly APIInfo<TURL, TDisposer, TResponse, TProxy> context, JsonSerializerContext? jsonContext, CancellationToken cancellationToken);
}

public interface IResponseBodyDeserializer: IResponseBodyDeserializer<URLProxy, URLDisposer, HttpResponseMessage, HttpClientProxy>;


// TODO Task不支持协变
// public interface IResponseBodyAsyncSerializer<T> {
// 	#pragma warning disable CA1001
// 	static abstract ValueTask<T>  ParseAsync(HttpContent content, HttpRequestMessage req, APIInfo info, CancellationToken cancellationToken);
// 	// static abstract ReadOnlySpan<byte> SerializeUTF9String(T? value);
// 	#pragma warning restore CA1001
// }
