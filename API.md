# RestFetch API Guide

This guide covers the public usage patterns of RestFetch. For runnable examples, see [`src/RestFetch.Examples`](src/RestFetch.Examples).

## 1. Define an API group

An API group is a `partial class`, `partial struct`, or `partial interface` that implements the corresponding `IRestFetchApi` interface. Classes and structs declare request methods as `partial`; interface methods are implemented by the generated implementation type.

```csharp
using RestFetch;

public sealed record User(int Id, string Name);

[BaseAddress("https://api.example.com")]
public sealed partial class UsersApi : IRestFetchApi<UsersApi>
{
    [Get("/users/{id}")]
    public partial Task<User?> GetUser([Param] int id);
}
```

The generator implements the request methods at compile time. A `partial interface` produces a nested `Impl` type:

```csharp
[BaseAddress("https://api.example.com")]
public partial interface IUsersApi : IRestFetchApi<IUsersApi.Impl>
{
    [Get("/users/{id}")]
    Task<User?> GetUser([Param] int id);
}

using var api = RestFetchService.For<IUsersApi.Impl>().Build();
```

Use classes for the usual case. Struct API groups are supported, but are generally not recommended because copying a struct can copy references to shared resources.

### Create and configure a client

`RestFetchService.For<TApi>()` creates a generated client using the default `HttpClientProxy`. You can supply a base address or an `IHttpClientProxyService<HttpResponseMessage>`:

```csharp
using var api = RestFetchService.For<UsersApi>().Build();

using var anotherApi = RestFetchService
    .For<UsersApi>("https://staging.example.com")
    .Build();
```

Call `Build()` after applying runtime configuration. Dispose the built client when finished; `using` is recommended.

The base-address precedence is:

1. `[BaseAddress]` on the request method
2. The address passed to `RestFetchService.For<TApi>(baseAddress)`
3. `[BaseAddress]` on the API group

The path supplied to an HTTP method attribute is appended to the base address. Ensure the slash boundary is correct for your address and path.

## 2. HTTP methods and paths

Use one HTTP method attribute on each request method:

`[Get]`, `[Post]`, `[Put]`, `[Patch]`, `[Delete]`, `[Head]`, `[Options]`, `[Connect]`, and `[Trace]`.

The path is optional and defaults to `/`:

```csharp
[Get("/users/{id}")]
public partial Task<User?> GetUser([Param] int id);
```

Path placeholders use parameter names by default. `[Param("name")]` maps a C# parameter to a differently named placeholder. Property paths are also supported:

```csharp
[Get("/users/{user.Id}")]
public partial Task<User?> GetUser([Param] UserKey user);

public sealed record UserKey(int Id);
```

`[Param]` parameters must be non-nullable. Values are URL-encoded by default; set `UrlEncode = false` only when the value is already encoded or encoding is otherwise not wanted. Use `[Param<TSerializer>]` to provide a custom path-parameter serializer.

## 3. Request parameters

Each request parameter must have at most one RestFetch parameter attribute, except that `CancellationToken` is recognized without an attribute.

### Query strings

`[Query]` uses the parameter name as the query key. Supply an alias to change it:

```csharp
[Get("/users")]
public partial Task<User?> Find(
    [Query("userId")] int id,
    [Query] string? search,
    CancellationToken cancellationToken);
```

By default, the serializer emits key/value pairs such as `userId=42`. Set `ValueOnly = true` when the endpoint expects values without keys:

```csharp
[Get("/users")]
public partial Task<User?> Find([Query(ValueOnly = true)] string token);
```

`[Query<TSerializer>]` selects a custom serializer. The built-in `RestFetch.WebUtilities.QueryString` serializer handles common primitive values, nullable values, and enumerable values. Collections may produce repeated keys.

The `RestFetch.WebUtilities` package also exposes `QueryString` for parsing and composing query strings, and `FastQueryString` for building query strings with less allocation.

### Headers

Static headers can be applied to an API group or a request method. Method-level values override matching group-level headers; use `null` to remove an inherited header:

```csharp
[Header("Accept", "application/json")]
public sealed partial class UsersApi : IRestFetchApi<UsersApi>
{
    [Header("X-Api-Version", "2")]
    [Get("/users")]
    public partial Task<User?> GetUser([Query] int id);
}
```

Dynamic header parameters can be either a `string` with a named header or a collection of key/value pairs:

```csharp
[Get("/users")]
public partial Task<User?> GetUser(
    [Header("Authorization")] string authorization,
    [Header] IEnumerable<KeyValuePair<string, string>> extraHeaders);
```

### Metadata

Metadata is request-scoped data stored in `ApiContext`; it is not sent as an HTTP header. Static method metadata uses a key and value, while a parameter supplies a dynamic value:

```csharp
[Metadata("source", "mobile")]
[Get("/users")]
public partial Task<User?> GetUser(
    [Query] int id,
    [Metadata("traceId")] string traceId);
```

Hooks can read dynamic and static values from `context.Metadata`.

### Request bodies

`[Body]` marks the request body. RestFetch selects a built-in serializer for recognized types and otherwise uses JSON serialization. Specify a serializer explicitly with `[Body<TSerializer>]`:

```csharp
[Post("/users")]
public partial Task<User?> Create([Body] User user);

[Post("/users")]
public partial Task<User?> CreateBuffered(
    [Body<RequestBodySerializer.JsonBuffered>] User user);
```

Built-in serializers include:

| Serializer | Typical input | Behavior |
| --- | --- | --- |
| `Json` | Arbitrary values | JSON request content; default for otherwise unrecognized types |
| `JsonBuffered` | Arbitrary values | Serializes JSON to a byte buffer |
| `RequestBodySerializer.JsonBody<T>` | `JsonBody<T>` | Reuses buffered JSON, useful when retries need replayable content |
| `SystemString` | `string` | Sends a string as content |
| `ByteArray`, `ByteMemory` | `byte[]`, `ReadOnlyMemory<byte>` | Sends binary content |
| `Stream`, `OctetStream` | `Stream` | Sends stream content; `OctetStream` sets the binary media type |
| `Raw` | `HttpContent` | Uses the supplied content directly |
| `Form` | Key/value pairs | Form URL-encoded content |
| `MultipartFormData` | File information or file collections | Multipart file content |

For multipart uploads, use `FileInfo` or `RestFetchFileInfo` (or collections of those types). For a stream that must be replayed during retries, implement `IStreamFactory` so each attempt can obtain a fresh stream.

`[Body(JsonContext = typeof(MyJsonContext))]` supplies a `JsonSerializerContext` for source-generated JSON metadata. The same option is available on `[Response]`. `ConfigureAwait` controls whether the serializer's asynchronous call captures the current context.

The body serializer interfaces are `IRequestBodySerializer<TContent>` and `IRequestBodySerializer<TBody, TContent>`. Implement one with a static `Serialize` method to create a custom serializer.

#### Buffer JSON with `JsonBody<T>`

`JsonBody<T>` is the built-in wrapper for JSON bodies that may be retried. The default `[Body]` serializer recognizes it and uses `RequestBodySerializer.JsonBody<T>`:

```csharp
[BaseAddress("https://api.example.com")]
[Retry(4)]
public sealed partial class UsersApi : IRestFetchApi<UsersApi>
{
    [Put("/users/{id}")]
    public partial Task<User?> UpdateUser(
        [Param] int id,
        [Body] JsonBody<User> user);
}

using var api = RestFetchService.For<UsersApi>().Build();
var update = new User(42, "Ada");
User? saved = await api.UpdateUser(update.Id, update);
```

`JsonBody<T>` implicitly converts from `T`, so passing `update` constructs the wrapper for the request. The first attempt serializes and buffers the JSON; subsequent attempts in that same request reuse the buffered bytes instead of serializing `T` again. Use an idempotent method such as `PUT` for automatic retries, or explicitly enable unsafe HTTP methods only when the server makes replaying them safe.

The generated request takes ownership of a `JsonBody<T>` argument by default and disposes it when the request completes. Do not manually `using` the wrapper or reuse it after passing it to a request. Each request invocation should receive a new wrapper; passing a `T` directly uses the implicit conversion to create one. To tune the initial buffer capacity, construct `new JsonBody<T>(value, initialCapacity)` explicitly; the default capacity is 102400 bytes.

`JsonBody<T>` requires non-null `T`, implements `IDisposable`, and offers `Create(JsonTypeInfo<T>)` and `Create(JsonSerializerOptions)` methods that return `ReadOnlyMemory<byte>`. The returned memory is backed by the wrapper and must not be used after the wrapper is disposed. The built-in serializer can use the JSON context selected with `[Body(JsonContext = typeof(MyJsonContext))]`; without one, it uses serializer options from the proxy or the built-in defaults.

### Cancellation

Include a non-nullable `CancellationToken` parameter to cancel sending, serialization, or response processing:

```csharp
[Get("/users")]
public partial Task<User?> GetUser(
    [Query] int id,
    CancellationToken cancellationToken);
```

## 4. Response types and deserialization

The return type determines how the response is consumed:

| Return type | Result |
| --- | --- |
| `Task` / `ValueTask` | Waits for the request without deserializing a body |
| `Task<T>` / `ValueTask<T>` | Deserializes the response as a string, stream, raw response, JSON node/document, or JSON value according to `T` |
| `Task<ApiResponse<T>>` / `ValueTask<ApiResponse<T>>` | Returns the deserialized content together with status, reason phrase, URL, and headers |
| `IAsyncEnumerable<T>` | Streams items from a JSON array or server-sent events, based on the response content type |

The built-in response deserializers support JSON, strings, streams, `JsonNode`, `JsonDocument`, `ApiResponse<T>`, and SSE forms including `SseEvent<T>` and the .NET `SseItem<T>`.

For JSON responses, `[Response(JsonContext = typeof(MyJsonContext))]` provides source-generated JSON metadata. Use `[Response<TDeserializer>]` to select a custom `IResponseBodyDeserializer` or `IResponseAsyncEnumerableDeserializer`.

Prefer nullable return types such as `Task<User?>` when an empty or no-content response is possible. If the declared return type is non-nullable and the deserializer returns `null`, RestFetch throws at runtime. `[Response(SkipNullableCheck = true)]` disables this check when the API guarantees a non-null result.

The caller owns disposable values returned as `Stream`, `HttpResponseMessage`, or `JsonDocument`. Dispose them after use. Ordinary deserialized values and the content in `ApiResponse<T>` do not require disposal by the caller.

## 5. Server-sent events

Declare an `IAsyncEnumerable<T>` return type for an SSE endpoint. The built-in deserializer uses the response content type to distinguish SSE from a JSON array:

```csharp
using RestFetch.WebUtilities;

[Get("/events")]
public partial IAsyncEnumerable<SseEvent<Notification>> Watch(
    [Query] int userId,
    CancellationToken cancellationToken);
```

Return `T` to receive each event's data, `RestFetch.WebUtilities.SseEvent<T>` to also receive the event ID, event name, and retry interval, or `System.Net.ServerSentEvents.SseItem<T>` to use the .NET SSE event type. Enumeration starts the request; cancellation and early termination are handled by the generated iterator.

Use `[OnSseEventError<THandler>]` to handle an individual event parsing/deserialization error. Its `OnSseEventError` method returns `true` to skip that event and continue, or `false` to propagate the error and stop the stream. Transport errors and request-level errors are handled by `[OnError<THandler>]`.

## 6. Hooks and exception handling

Hooks are specified with generic attributes and implemented by types with static methods. They can be applied to an API group or overridden for a method where supported.

| Attribute | Interface | When it runs |
| --- | --- | --- |
| `[BeforeSend<T>]` | `IBeforeSendHook` | Before the transport sends the request |
| `[AfterResponse<T>]` | `IAfterResponseHook` | After the transport receives the response |
| `[ExceptionFactory<T>]` | `IExceptionFactory` | Converts a response into an exception, if needed |
| `[OnSuccess<T>]` | `IOnSuccess` | After successful response processing |
| `[OnError<T>]` | `IOnError` | When request processing throws |

The default exception factory throws `RestFetchApiException` for unsuccessful HTTP responses. A custom factory returns `null` when no exception should be raised:

```csharp
[ExceptionFactory<ApiExceptionFactory>]
public sealed partial class UsersApi : IRestFetchApi<UsersApi>
{
    [Get("/users/{id}")]
    public partial Task<User?> GetUser([Param] int id);
}

public sealed class ApiExceptionFactory : IExceptionFactory
{
    public static ValueTask<Exception?> CreateException(
        HttpResponseMessage response,
        ApiContext context,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return ValueTask.FromResult<Exception?>(null);

        return ValueTask.FromResult<Exception?>(
            new HttpRequestException($"Request failed with status {(int)response.StatusCode}."));
    }
}
```

`ApiContext` carries the request ID, API and method names, URL, headers, metadata, and other request information. Hooks that create disposable intermediate resources can register them with the supplied `ResourceManager` so they are released at the end of the request.

`[IdGenerator<T>]` selects an `IRequestIdGenerator` implementation at API-group or method scope. The default generator creates numeric request IDs.

## 7. Retry and timeout

Use `[Retry]` and `[Timeout]` for compile-time configuration. Both can be applied to an API group or an individual method; method-level options take precedence:

```csharp
[Retry(4)]
[BaseAddress("https://api.example.com")]
public sealed partial class UsersApi : IRestFetchApi<UsersApi>
{
    [Get("/users/{id}")]
    [Timeout(AttemptTimeout = 2, TotalTimeout = 10)]
    public partial Task<User?> GetUser([Param] int id);
}
```

`RetryAttribute` supports `MaxRetryAttempts`, `BackoffType`, `Delay`, `MaxDelay`, `UseJitter`, `Ignore`, `AllowUnsafeHttpMethod`, `AllowUnsafeRequestBody`, `Predicate`, and `OnRetry`. The default predicate retries HTTP 408, 429, 500, 502, 503, and 504 responses, as well as `HttpRequestException`, `HttpIOException`, `SocketException`, Polly timeout rejection, and timeout cancellation failures.

The default safety check rejects `POST`, `PATCH`, and `CONNECT` unless `AllowUnsafeHttpMethod` is enabled. It also rejects `Stream`, `HttpContent`, and `HttpRequestMessage` bodies unless `AllowUnsafeRequestBody` is enabled. A non-seekable `Stream` is rejected for each invocation even when that option is enabled. Prefer `JsonBody<T>` or `IStreamFactory` when the request body must be replayed.

Use `JsonBody<T>` or `IStreamFactory` when a body must be replayable across attempts. `AllowUnsafeHttpMethod` and `AllowUnsafeRequestBody` disable their respective default checks, but a non-seekable `Stream` is still rejected on each invocation. Only relax a safety check when replaying the operation is safe for your API.

Timeouts support `AttemptTimeout` for each attempt, `TotalTimeout` for the whole operation, and `Ignore` to disable inherited settings.

Runtime configuration is available on the generated API instance:

```csharp
using var api = RestFetchService.For<UsersApi>()
    .AddRetry(new RetryOptions<HttpResponseMessage>(MaxRetryAttempts: 5))
    .AddTimeout(new TimeoutOptions(AttemptTimeout: 3, TotalTimeout: 15))
    .Build();
```

Use `AddRetryFor` and `AddTimeoutFor` with `nameof(UsersApi.GetUser)` for method-level runtime settings. Polly's `RetryStrategyOptions<TResponse>`, `TimeoutStrategyOptions`, and `ResiliencePipeline<TResponse>` can also be supplied through the corresponding overloads. Runtime settings override compile-time settings at the same scope. Call `Build()` after configuration; the built settings are frozen.

Implement `IRetryPredicate` to customize retry decisions and `IOnRetry` to observe retry attempts.

## 8. Custom transport

The default transport is `HttpClientProxy`. Pass a custom `HttpMessageHandler` when you need to configure the underlying `HttpClient`:

```csharp
var handler = new SocketsHttpHandler
{
    AutomaticDecompression = DecompressionMethods.All
};

using var proxy = new HttpClientProxy(handler)
{
    BaseAddress = "https://api.example.com"
};
using var api = RestFetchService.For<UsersApi>(proxy).Build();
```

To replace `HttpClient` entirely, implement `IHttpClientProxy<TSelf, TResponse>` (which extends `IHttpClientProxyService<TResponse>`), then select it with `[HttpClientProxy<TProxy>]`. Use `IRestFetchApi<TSelf, TResponse>` for an API group whose transport returns a response type other than `HttpResponseMessage`. Custom proxies should be paired with compatible body serializers, response deserializers, and hooks.

## 9. Dependency injection

Install `RestFetch.Extensions` and import `RestFetch.Extensions`. Register a generated API group with `IHttpClientFactory`:

```csharp
services.AddRestFetchApi<UsersApi>("https://api.example.com");
```

The registration returns an `IHttpClientBuilder`, so the named client can be configured using standard `IHttpClientFactory` APIs. The extension package also provides overloads for interface registrations and keyed services.

## 10. Related examples

- [`GettingStarted.cs`](src/RestFetch.Examples/GettingStarted.cs)
- [`QueryExample.cs`](src/RestFetch.Examples/QueryExample.cs)
- [`BodyExample.cs`](src/RestFetch.Examples/BodyExample.cs)
- [`ReturnTypeExample.cs`](src/RestFetch.Examples/ReturnTypeExample.cs)
- [`RetryExample.cs`](src/RestFetch.Examples/RetryExample.cs)
- [`HttpClientProxyExample.cs`](src/RestFetch.Examples/HttpClientProxyExample.cs)

## 11. Attribute reference

The generator embeds the attributes into the consumer compilation. No separate attribute package or runtime reflection scan is required.

### API and method attributes

| Attribute | Targets | Purpose and options |
| --- | --- | --- |
| `[BaseAddress(string)]` | API group, request method | Sets the base address. A method value overrides the runtime value passed to `For`, which overrides the API-group value. |
| `[Get(path)]`, `[Post(path)]`, `[Put(path)]`, `[Patch(path)]`, `[Delete(path)]`, `[Head(path)]`, `[Options(path)]`, `[Connect(path)]`, `[Trace(path)]` | Request method | Selects the HTTP method and optional path. |
| `[Header(name, value)]` | API group, request method | Adds static headers. A method header overrides an API-group header with the same name; a `null` value removes an inherited header. |
| `[Metadata(key, value)]` | Request method | Adds static request metadata for `ApiContext`; metadata is not serialized as an HTTP header. |
| `[HttpClientProxy<TProxy>]` | API group | Selects a custom static `IHttpClientProxy<TProxy, TResponse>` implementation. `ConfigureAwait` controls context capture for the proxy call. |
| `[IdGenerator<TGenerator>]` | API group, request method | Selects the `IRequestIdGenerator` used for request IDs. |

### Parameter attributes

| Attribute | Targets | Purpose and options |
| --- | --- | --- |
| `[Param]`, `[Param("alias")]` | Parameter | Supplies a path placeholder. `UrlEncode` defaults to `true`; `[Param<TSerializer>]` selects a custom serializer. |
| `[Query]`, `[Query("alias")]` | Parameter | Supplies query-string values. `ValueOnly = true` emits values without keys; `[Query<TSerializer>]` selects a custom serializer. |
| `[Header("name")]` | Parameter | Adds a dynamic header value from a `string`. `[Header]` accepts a sequence of header key/value pairs. |
| `[Metadata("key")]` | Parameter | Adds a dynamic value to request metadata under the specified key. |
| `[Body]`, `[Body<TSerializer>]` | Parameter | Selects the request body and serializer. Supports `JsonContext`, `ConfigureAwait`, `TakeOwnershipOfSerializeResult`, and `TakeOwnershipOfArgument`. |

`CancellationToken` is recognized without an attribute. Other parameters must have no more than one RestFetch parameter attribute. A method can have at most one body parameter.

`TakeOwnershipOfArgument` asks the request resource manager to dispose an `IDisposable` body argument after request processing. It defaults to `false` for ordinary body types, but defaults to `true` for `JsonBody<T>`. `TakeOwnershipOfSerializeResult` transfers ownership of an `IDisposable` serializer result; it defaults to `false` for ordinary serializers and direct `Stream`/`HttpContent` bodies, and is enabled automatically for the built-in `IStreamFactory` serializer. Set these options explicitly when customizing ownership.

### Response and hook attributes

| Attribute | Targets | Purpose and options |
| --- | --- | --- |
| `[Response]`, `[Response<TDeserializer>]` | Request method | Selects a response deserializer. Supports `JsonContext`, `SkipNullableCheck`, and `ConfigureAwait`. |
| `[BeforeSend<THook>]` | API group, request method | Runs before sending; hook implements `IBeforeSendHook<TBodyContent>`. |
| `[AfterResponse<THook>]` | API group, request method | Runs after receiving a response; hook implements `IAfterResponseHook<TResponse>`. |
| `[ExceptionFactory<TFactory>]` | API group, request method | Creates an exception from the response; factory implements `IExceptionFactory<TResponse>`. |
| `[OnSuccess<THook>]` | API group, request method | Runs after successful response processing; hook implements `IOnSuccess`. |
| `[OnError<THook>]` | API group, request method | Runs when request processing throws; hook implements `IOnError`. |
| `[OnSseEventError<THook>]` | API group, request method | Handles a parsing failure for one streamed SSE event; hook implements `IOnSseEventError`. |
| `[Retry]`, `[Retry(maxRetryAttempts)]` | API group, request method | Configures resilience retries. Options: `MaxRetryAttempts`, `BackoffType`, `Delay`, `MaxDelay`, `UseJitter`, `Ignore`, `AllowUnsafeHttpMethod`, `AllowUnsafeRequestBody`, `Predicate`, and `OnRetry`. |
| `[Timeout]`, `[Timeout(total)]` | API group, request method | Configures `AttemptTimeout`, `TotalTimeout`, and `Ignore`. |

Hook attributes expose `ConfigureAwait`, which defaults to `false`: generated awaits use `ConfigureAwait(false)`. Set it to `true` when the hook or extension call should capture the current context. Response attributes also default to `ConfigureAwait = false`. `[Response]` defaults `SkipNullableCheck` to `false`.

The `[Retry]` attribute defaults are `MaxRetryAttempts = 3`, constant backoff, `Delay = 2`, `MaxDelay = 10`, `UseJitter = false`, and the default retry predicate and retry observer. `[Timeout]` defaults to a 3-second attempt timeout and a 10-second total timeout. These compile-time defaults are distinct from `RetryOptions<TResponse>` and `TimeoutOptions`, whose optional values are documented below.

## 12. Custom serializer, deserializer, and hook contracts

Serializer and hook implementations use static interface members. The type passed to a generic attribute must implement the matching interface for the selected request/response types.

### Request serializers

```csharp
public interface IRequestBodySerializer<TContent>
{
    static abstract ValueTask<TContent> Serialize<TBody>(
        TBody? value,
        JsonSerializerContext? jsonContext,
        ApiContext context,
        CancellationToken cancellationToken);
}

public interface IRequestBodySerializer<in TBody, TContent>
{
    static abstract ValueTask<TContent> Serialize(
        TBody? value,
        JsonSerializerContext? jsonContext,
        ApiContext context,
        CancellationToken cancellationToken);
}
```

The open-generic form is useful when the serializer accepts multiple body types. The body-specific form is more restrictive and provides a strongly typed input. Both can return any content type accepted by the selected transport proxy.

Example:

```csharp
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class CreateUserSerializer
    : IRequestBodySerializer<User, HttpContent>
{
    public static ValueTask<HttpContent> Serialize(
        User? value,
        JsonSerializerContext? jsonContext,
        ApiContext context,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(value);
        HttpContent content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");
        return ValueTask.FromResult(content);
    }
}

[Post("/users")]
public partial Task<User?> Create(
    [Body<CreateUserSerializer>] User user);
```

Use the supplied `jsonContext` when the endpoint's `[Body(JsonContext = ...)]` specifies source-generated JSON metadata. Pass through the cancellation token for asynchronous serialization.

### Response deserializers

The response parameter type must implement `IDisposable`, because the generated request pipeline owns and disposes the transport response:

```csharp
public interface IResponseBodyDeserializer<in TResponse>
    where TResponse : IDisposable
{
    static abstract Task<TResult?> Parse<TResult>(
        TResponse response,
        ApiContext context,
        JsonSerializerContext? jsonContext,
        CancellationToken cancellationToken);
}

public interface IResponseBodyDeserializer<in TResponse, TResult>
    where TResponse : IDisposable
{
    static abstract Task<TResult?> Parse(
        TResponse response,
        ApiContext context,
        JsonSerializerContext? jsonContext,
        CancellationToken cancellationToken);
}
```

For `IAsyncEnumerable<T>` responses, implement one of these interfaces:

```csharp
public interface IResponseAsyncEnumerableDeserializer<in TResponse>
    where TResponse : IDisposable
{
    static abstract IAsyncEnumerable<SseEventResult<TResult?>> Parse<
        TResult,
        TSseEventOnError>(
        TResponse response,
        ApiContext context,
        JsonSerializerContext? jsonContext,
        CancellationToken cancellationToken)
        where TSseEventOnError : IOnSseEventError;
}

public interface IResponseAsyncEnumerableDeserializer<in TResponse, TResult>
    where TResponse : IDisposable
{
    static abstract IAsyncEnumerable<SseEventResult<TResult?>> Parse<
        TSseEventOnError>(
        TResponse response,
        ApiContext context,
        JsonSerializerContext? jsonContext,
        CancellationToken cancellationToken)
        where TSseEventOnError : IOnSseEventError;
}
```

Return `SseEventResult<T>.Ok(value)` for a parsed item or `SseEventResult<T>.Err(exception, skip)` for a failed item. `skip = true` skips the event; `skip = false` propagates the error and ends enumeration.

### Parameter serializers

`IParamSerializer<TValue, TResult>` serializes a path value; `IParamEncodeSerializer<TValue, TResult>` provides an encoding-specific serialization path. For queries, implement `IQueryStringSerializer<TValue>` for value-only serialization or `IAliasQueryStringSerializer<TValue>` when the query key is also needed. The built-in `QueryString` implements the query serialization interfaces for common primitive, nullable, and enumerable values.

```csharp
public interface IParamSerializer<in TValue, out TResult>
{
    static abstract TResult Serialize(string aliasKey, TValue value);
}

public interface IParamEncodeSerializer<in TValue, out TResult>
{
    static abstract TResult Encode(string aliasKey, TValue value);
}

public interface IQueryStringSerializer<in TValue>
    where TValue : allows ref struct
{
    static abstract string Serialize(TValue value);
}

public interface IAliasQueryStringSerializer<in TValue>
    where TValue : allows ref struct
{
    static abstract string Serialize(string aliasKey, TValue value);
}
```

`IQueryStringSerializer` (non-generic) additionally provides a generic `Serialize<T>(T? value)` method for a serializer that handles multiple value types.

### Hook interfaces

| Interface | Static member | Purpose |
| --- | --- | --- |
| `IBeforeSendHook<TBodyContent>` | `Task BeforeSend(TBodyContent? body, ApiContext context, ResourceManager resourceManager, CancellationToken token)` | Inspect or modify a serialized body before transport. |
| `IAfterResponseHook<TResponse>` | `Task AfterResponse(TResponse response, ApiContext context, ResourceManager resourceManager, CancellationToken token)` | Observe the received response before normal response processing. |
| `IExceptionFactory<TResponse>` | `ValueTask<Exception?> CreateException(TResponse response, ApiContext context, CancellationToken token)` | Return an exception to throw, or `null` to proceed. |
| `IOnSuccess` | `Task OnSuccess<TBody>(TBody? body, ApiContext context, CancellationToken token)` | Observe a successfully deserialized result. |
| `IOnError` | `Task OnError(Exception? error, ApiContext context, CancellationToken token)` | Observe a request-level failure. |
| `IOnSseEventError` | `Task<bool> OnSseEventError(Exception error, ApiContext context, CancellationToken token)` | Return `true` to skip a failed SSE event; `false` to propagate it. |
| `IOnRetry<TResponse>` | `ValueTask OnRetry(OnRetryArguments<TResponse> arguments)` | Observe each Polly retry. Its default static implementation is a no-op. |
| `IRequestIdGenerator` | `RequestId NextId()` | Provide an ID for each request. |

Hooks are type-based rather than instance-based. Register temporary disposable resources with the hook's `ResourceManager` argument; they are disposed when the request completes. `ApiContext` exposes the request's `Id`, `Client`, `Options`, `ClassName`, `MethodName`, `HttpMethod`, `Metadata`, `ReturnsAsyncEnumerable`, `Url`, `Headers`, and `ReturnTypeInfo`.

The retry predicate contract is separate from the `IOnRetry` observer:

```csharp
public interface IRetryPredicate<TResponse>
    where TResponse : IDisposable
{
    static abstract Func<RetryPredicateArguments<TResponse>, ValueTask<bool>>
        CreateShouldHandle<TBody>(
            TBody? body,
            RetryOptions<TResponse>? options,
            HttpMethod httpMethod);

    static abstract void ValidateRequestBody<TBody>(
        TBody? body,
        RetryOptions<TResponse>? options,
        ApiContext context);
}
```

`CreateShouldHandle` runs when the pipeline is built and returns its per-attempt decision function. `ValidateRequestBody` runs for every request invocation.

## 13. Retry and timeout API reference

`RetryOptions<TResponse>` is a record with these constructor parameters:

| Parameter | Type | Meaning |
| --- | --- | --- |
| `Ignore` | `bool` | Ignore inherited retry configuration at this scope. |
| `AllowUnsafeHttpMethod` | `bool` | Permit retries for methods rejected by the default safety check. |
| `AllowUnsafeRequestBody` | `bool` | Permit retrying a body rejected by the default replayability check. |
| `MaxRetryAttempts` | `int?` | Maximum number of retry attempts. |
| `BackoffType` | `DelayBackoffType` | Polly delay strategy. |
| `Delay` | `int?` | Delay value used by the strategy. |
| `MaxDelay` | `int?` | Optional maximum delay. |
| `UseJitter` | `bool?` | Whether to use jitter. |
| `CreateShouldHandle` | `Func<object?, RetryOptions<TResponse>?, HttpMethod, Func<RetryPredicateArguments<TResponse>, ValueTask<bool>>>?` | Creates the Polly retry predicate. |
| `ValidateRequestBody` | `Action<object?, RetryOptions<TResponse>?, ApiContext>?` | Validates whether the current body can be retried. |
| `OnRetry` | `Func<OnRetryArguments<TResponse>, ValueTask>?` | Callback invoked on retry. |

`TimeoutOptions` is a record with `Ignore`, `AttemptTimeout`, and `TotalTimeout` optional values (seconds). Runtime API methods also accept Polly's `RetryStrategyOptions<TResponse>`, `TimeoutStrategyOptions`, and `ResiliencePipeline<TResponse>`.

The runtime `RetryOptions<TResponse>` defaults are `Ignore = false`, both unsafe flags `false`, no explicit maximum retry count, constant backoff, a 2-second delay, no maximum delay, and no explicit jitter setting. When constructing the Polly strategy, RestFetch uses 3 retries and no jitter if those values are omitted.

`IRetryPredicate<TResponse>.CreateShouldHandle` is called while the method's resilience pipeline is constructed and returns the predicate used for later attempts. `ValidateRequestBody` runs for each method invocation; use it to inspect whether the body's declared/runtime kind is replayable, not to cache request-specific decisions in the pipeline.

Runtime configuration methods are declared by `IRestFetchApiService<TSelf, TResponse>`:

```csharp
TSelf AddRetry(RetryOptions<TResponse>? options);
TSelf AddRetry(RetryStrategyOptions<TResponse>? options);
TSelf AddRetryFor(string methodName, RetryOptions<TResponse>? options);
TSelf AddRetryFor(string methodName, RetryStrategyOptions<TResponse>? options);

TSelf AddTimeout(TimeoutOptions? options);
TSelf AddTimeout(TimeoutStrategyOptions? options);
TSelf AddTimeoutFor(string methodName, TimeoutOptions? options);
TSelf AddTimeoutFor(string methodName, TimeoutStrategyOptions? options);

TSelf AddResiliencePipeline(ResiliencePipeline<TResponse> pipeline);
TSelf AddResiliencePipelineFor(
    string methodName,
    ResiliencePipeline<TResponse> pipeline);

TSelf Build();
```

Group-level and method-level runtime configuration may be mixed. `Build()` finalizes the client configuration and returns the same generated API instance; configure the instance before building it.

## 14. Runtime API and public type index

Types below are consumer-facing runtime contracts and helpers. Source-generator implementation/model types are intentionally not included.

### API creation and transport

| Type | Use |
| --- | --- |
| `RestFetchService` | `For<TApi>()`, `For<TApi>(string baseAddress)`, and `For<TApi>(IHttpClientProxyService<HttpResponseMessage>)` factories for the default response type. |
| `RestFetchService<THttpClient, TResponse>` | Corresponding factories for custom transport response types. |
| `IRestFetchApi<TSelf>` | API contract when the proxy returns `HttpResponseMessage`; exposes static `Create` factories and runtime configuration. |
| `IRestFetchApi<TSelf, TResponse>` | API contract for a custom disposable response type. |
| `IRestFetchApiService<TSelf, TResponse>` | Runtime retry, timeout, pipeline, and `Build()` configuration. |
| `IHttpClientProxyService<TResponse>` | Transport service with `BaseAddress`, `Options`, `SendAsync`, and disposal. |
| `IHttpClientProxy<TSelf, TResponse>` | Static factory contract for an HTTP proxy implementation. |
| `HttpClientProxy` | Default `HttpClient` transport. Accepts default options or a custom `HttpMessageHandler`; exposes `BaseAddress` and `Create(string?)`. |
| `HttpClientProxyOptions` | Union of factory-based and handler-based options. |
| `HttpClientProxyOptions.HttpClientProxyCommonOptions` | Common `SerializerOptions` and `DeserializerOptions`. |
| `HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions` | Custom `Handler` and `SharedHandler` settings. |
| `HttpClientProxyOptions.HttpClientProxyFactoryOptions` | `Func<HttpClient> Factory` for creating clients. |

The generated API contract exposes these static factories:

```csharp
static abstract TSelf Create();
static abstract TSelf Create(string baseAddress);
static abstract TSelf Create(IHttpClientProxyService<TResponse> client);
static abstract TSelf Create(
    bool sharedClient,
    IHttpClientProxyService<TResponse> client);
```

`RestFetchService.For<TApi>(proxy)` passes an externally supplied proxy to the generated API as a shared client; the API does not dispose it. Dispose that proxy separately. The generated static `Create(false, proxy)` overload transfers ownership to the generated API, while `Create(true, proxy)` leaves disposal to the caller.

### Request and response data types

| Type | Use |
| --- | --- |
| `ApiContext` | Request-scoped context passed to serializers, hooks, and proxies. |
| `RequestId` | Numeric or string request identifier; implicitly constructible from `long` and `string`. |
| `RequestIdGenerator` | Default `IRequestIdGenerator`, producing incrementing numeric request IDs. |
| `ReturnTypeInfo` | Describes the generated method's return wrapper, value type, nullability, and response kind. |
| `ReturnTypeKind`, `ReturnTypeWrapperKind` | Enums used by `ReturnTypeInfo` to describe response and async wrapper categories. |
| `ApiResponse<T>` | Deserialized content together with response status, reason, URL, and headers. |
| `JsonBody<T>` | Reusable buffered JSON body; `Create(...)` returns serialized UTF-8 bytes. It is disposable and, when passed as a body, the generated client takes ownership by default. |
| `RestFetchFileInfo` | Multipart file metadata with `Name`, `FileName`, and `File` fields; implicitly converts to and from `FileInfo`. |
| `SseEventResult<T>` | Success/error union for custom async-enumerable deserializers (`Ok` or `Err`). |
| `ResourceManager` | Request-lifetime ownership manager for synchronous and asynchronous disposable resources. |
| `RetryOptions<TResponse>`, `TimeoutOptions` | Runtime resilience configuration records. |
| `IStreamFactory` | Describes a stream body through `ContentType` and `ValueTask<Stream> Create()`. |

### Built-in serializer/deserializer types

| Type | Supported built-in forms |
| --- | --- |
| `RequestBodySerializer.Json` | JSON serialization fallback for arbitrary body types. |
| `RequestBodySerializer.JsonBuffered` | Buffered JSON serialization. |
| `RequestBodySerializer.JsonBody<T>` | Serialization of the `JsonBody<T>` wrapper. |
| `ParamSerializer` | Default serializer/encoder for primitive path parameter values and strings. |
| `RequestBodySerializer.Form` | URL-encoded key/value content. |
| `RequestBodySerializer.MultipartFormData` | Multipart content. |
| `RequestBodySerializer.ByteArray`, `ByteMemory` | Byte array and `ReadOnlyMemory<byte>` content. |
| `RequestBodySerializer.Stream`, `OctetStream` | Stream content, optionally with a binary media type. |
| `RequestBodySerializer.StreamFactory<TFactory>` | Creates content from a factory, including fresh streams for retries. |
| `RequestBodySerializer.SystemString` | String content. |
| `RequestBodySerializer.Raw` | Passes through `HttpContent`. |
| `ResponseBodyDeserializer.Json` | Typed JSON values. |
| `ResponseBodyDeserializer.JsonNode`, `JsonDocument` | System.Text.Json DOM responses. |
| `ResponseBodyDeserializer.SystemString`, `Stream` | Text and stream results. |
| `ResponseBodyDeserializer.ApiResponse<T>` | Response metadata and typed content. |
| `ResponseBodyDeserializer.SseOrJsonArray` | Async enumerable selected from response content type. |
| `ResponseBodyDeserializer.SseString`, `SseJsonNode`, `SseJsonDocument` | SSE data in text, node, or document form. |
| `ResponseBodyDeserializer.SseEvent`, `SseEvent<T>` | SSE values with event metadata. |
| `ResponseBodyDeserializer.SseItem<T>` | .NET `System.Net.ServerSentEvents.SseItem<T>` values. |

### Exception and default policy types

| Type | Use |
| --- | --- |
| `RestFetchApiException` | Default unsuccessful-status exception. Exposes `Id`, `MethodName`, `StatusCode`, `ReasonPhrase`, `Version`, and optional `ResponseBody`. |
| `ReturnTypeException` | Indicates an incompatible non-nullable API return declaration and nullable deserializer result. |
| `UnsafeRetryHTTPMethodException` | Indicates a retry configuration that the default policy rejects as unsafe. |
| `UnsafeRetryHTTPRequestBodyException` | Indicates a request body rejected by the default retry replayability check. |
| `DefaultExceptionFactory` | Default exception mapping for unsuccessful HTTP responses. |
| `DefaultRetryPredicate` | Default transient-error and unsafe-retry policy. |
| `DefaultOnRetry` | Default no-op retry callback. |
| `DefaultSseEventErrorHandler` | Default SSE error handler; failed events are skipped by the interface default. |

`JsonBody<T>` provides `Create(JsonTypeInfo<T>)` and `Create(JsonSerializerOptions)` overloads, and an implicit conversion from `T`. `ApiResponse<T>` is a record containing `StatusCode`, `ReasonPhrase`, `Url`, nullable `Content`, and `Headers`.

`ResourceManager` supports `TakeOwnership` / `TryTakeOwnership` for `IDisposable`, `TakeOwnershipAsync` / `TryTakeOwnershipAsync` for `IAsyncDisposable`, `Absolve` / `AbsolveAsync` to remove previously registered resources, and synchronous or asynchronous disposal. Registered resources are disposed in reverse registration order.

## 15. Query-string and web utility APIs

The utilities live in `RestFetch.WebUtilities`.

### Query string collections and builders

| Type | Use |
| --- | --- |
| `QueryString` | Non-generic collection using the default URL encoder. Parses and composes query strings with multiple values per key. |
| `QueryString<TEncoder>` | Encoder-parameterized query collection. Provides `Parse`, `TryParse`, `Set`, `Add`, `Remove`, `Clear`, `TryGetValue`, `Keys`, `Count`, `Value`, and `ToUriComponent`. |
| `FastQueryString` | Non-generic `IDisposable` struct builder for composing query strings. |
| `FastQueryString<TEncoder>` | Encoder-parameterized builder with `Add` overloads for scalar, nullable, and enumerable values, plus `ToString`, `ToUriComponent`, and `TryFormat`. |
| `IURLEncoder` | Static URL encoding/decoding contract; use as an encoder type parameter. |
| `URLHelper` | Default encoder with `EncodeURIComponent` and `DecodeURIComponent` helpers. |
| `FormEncoder` | Encoder implementation for form-compatible query serialization. |

Use `QueryString` when parsing, looking up, or editing existing query data. Use `FastQueryString` when constructing a query string incrementally. Both support repeated keys for multiple values. `FastQueryString` owns its internal builder and should be disposed when finished.

### Query serializer contracts and event values

| Type | Use |
| --- | --- |
| `IQueryStringSerializer<T>` | Static serialization of a query value without an alias key. |
| `IAliasQueryStringSerializer<T>` | Static serialization when the alias/key is part of the call. |
| `IQueryStringDeserializer<T>` | Static parsing of a query-string representation into a value. |
| `IQueryStringConverter<T>` | Combines query serialization and deserialization. |
| `SseEvent` | SSE event data as a string, with `Id`, `Event`, and optional `Retry`. |
| `SseEvent<T>` | Typed SSE data, with `Id`, `Event`, `Retry`, and `Data`. |

`FastQueryString<TEncoder>` is a builder rather than a lookup collection. Use `QueryString<TEncoder>` for collection-style parsing and mutation.

`IQueryStringDeserializer<T>` provides static `Parse(string?)` and `TryParse(string?, out T?)` methods. `IQueryStringConverter<T>` combines serialization and deserialization. `IURLEncoder` exposes static `EncodeURIComponent` and `DecodeURIComponent` overloads for nullable strings and character spans; `URLHelper` is the default implementation.

## 16. Dependency injection registration reference

`RestFetch.Extensions.HttpClientFactoryExtensions` provides these registration families for `IServiceCollection`:

| Method family | Registration |
| --- | --- |
| `AddRestFetchApi<TApiImpl>(baseAddress, httpClientName, configureFn)` | Registers a generated implementation as a singleton and returns an `IHttpClientBuilder`. |
| `AddRestFetchApi<TApi, TApiImpl>(...)` | Registers the implementation under an API interface and returns an `IHttpClientBuilder`. |
| `AddRestFetchApi<TApiImpl, TProxyKey>(proxyKey, configureFn)` | Binds an API implementation to an already registered keyed proxy service. |
| `AddRestFetchApi<TApi, TApiImpl, TProxyKey>(proxyKey, configureFn)` | Interface registration using an existing keyed proxy. |
| `AddKeyedRestFetchApi<TKey, TProxyKey, TApiImpl>(key, proxyKey, configureFn)` | Keyed API implementation using an existing keyed proxy. |
| `AddKeyedRestFetchApi<TKey, TApi, TApiImpl>(key, ...)` | Keyed interface registration with a named `HttpClient`; returns an `IHttpClientBuilder`. |
| `AddKeyedRestFetchApi<TKey, TProxyKey, TApi, TApiImpl>(key, proxyKey, configureFn)` | Keyed interface registration using an existing keyed proxy. |
| `AddHttpClientProxyService<TKey>(key, baseAddress, httpClientName, options)` | Registers a keyed default proxy and returns an `IHttpClientBuilder`. |
| `TryAddDefaultHttpClientProxyService(httpClientName)` | Adds a fallback proxy registration for any key and returns an `IHttpClientBuilder`. |

The `configureFn` callback runs before the API's `Build()` call. Use it to configure runtime retry, timeout, or resilience pipelines. Registrations are singleton; do not capture scoped dependencies in the configuration callback.

## 17. Additional public helpers

The following lower-level helpers may be useful when extending or integrating RestFetch:

- `IStreamFactory`: supplies `ContentType` and an asynchronously created stream for replayable stream bodies.
- `IAsyncEnumerableTExtensions.Catch<T>`: wraps an async enumerable, invokes an error callback for enumeration failures, and accepts a cancellation token.
- `FixedArrayBufferWriter`, `DelegatingStream`, and `RestFetchResponseStream`: buffer and stream support used by serialization and transport integrations.
- `ThrowHelper`: shared argument/error helpers used by generated clients.
- `EmptyStringToBoolConverter`, `EmptyStringToByteConverter`, `EmptyStringToSByteConverter`, `EmptyStringToShortConverter`, `EmptyStringToUshortConverter`, `EmptyStringToIntConverter`, `EmptyStringToUintConverter`, `EmptyStringToLongConverter`, `EmptyStringToUlongConverter`, `EmptyStringToDecimalConverter`, `EmptyStringToFloatConverter`, and `EmptyStringToDoubleConverter`: nullable numeric/boolean JSON converters.

Prefer the higher-level `RequestBodySerializer`, `ResponseBodyDeserializer`, `QueryString`, and `FastQueryString` APIs unless implementing a custom integration.