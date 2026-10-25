using System.Text.Json.Serialization;

namespace RestFetch;

public interface IResponseBodyDeserializer<in TResponse> where TResponse: IDisposable {
	static abstract Task<TReturn?> Parse<TReturn>(TResponse resp, ref readonly ReturnTypeInfo returnTypeInfo, ref readonly ApiContext context, JsonSerializerContext? jsonContext, CancellationToken cancellationToken);
}

public interface IResponseBodyDeserializer<in TResponse, TReturn> where TResponse: IDisposable {
	static abstract Task<TReturn?> Parse(TResponse resp, ref readonly ReturnTypeInfo returnTypeInfo, ref readonly ApiContext context, JsonSerializerContext? jsonContext, CancellationToken cancellationToken);
}

public interface IResponseBodyDeserializer: IResponseBodyDeserializer<HttpResponseMessage>;

public interface IResponseAsyncEnumerableDeserializer<in TResponse> where TResponse: IDisposable {
	static abstract IAsyncEnumerable<SseEventResult<TReturn?>> Parse<TReturn>(TResponse resp, ref readonly ReturnTypeInfo returnTypeInfo, ref readonly ApiContext context, JsonSerializerContext? jsonContext, SseEventExceptionHandler? handler, CancellationToken cancellationToken);
}

public interface IResponseAsyncEnumerableDeserializer<in TResponse, TReturn> where TResponse: IDisposable {
	static abstract IAsyncEnumerable<SseEventResult<TReturn?>> Parse(TResponse resp, ref readonly ReturnTypeInfo returnTypeInfo, ref readonly ApiContext context, JsonSerializerContext? jsonContext, SseEventExceptionHandler? handler, CancellationToken cancellationToken);
}

public interface IResponseAsyncEnumerableDeserializer: IResponseAsyncEnumerableDeserializer<HttpResponseMessage>;

// NOTE: 不用ValueTask,因为Task<bool>有缓存优化不会有堆分配
// 受限于API,这里不使用ApiContext,仅传递最小必要信息,请求ID和URL
public delegate Task<bool> SseEventExceptionHandler(Exception error, ApiMinimalContext context);


// TODO Task不支持协变
// public interface IResponseBodyAsyncSerializer<T> {
// 	#pragma warning disable CA1001
// 	static abstract ValueTask<T>  ParseAsync(HttpContent content, HttpRequestMessage req, ApiContext info, CancellationToken cancellationToken);
// 	// static abstract ReadOnlySpan<byte> SerializeUTF9String(T? value);
// 	#pragma warning restore CA1001
// }
