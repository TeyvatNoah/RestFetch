# RestFetch

RestFetch is a C# HTTP client library that uses a source generator to turn annotated API definitions into request clients at compile time. It aims to keep API declarations concise while providing control over request and response processing.

## Highlights

- Compile-time generated clients, designed to work well with trimming and Native AOT.
- Attribute-based definitions for HTTP methods, paths, query strings, headers, metadata, and request bodies.
- Customizable serializers, deserializers, transport proxies, and lifecycle hooks.
- Method- and API-group-level retry and timeout configuration.
- Support for `Task<T>`, `ValueTask<T>`, raw responses, streams, JSON, and server-sent events.
- Optional `IHttpClientFactory` integration.

## Installation

Install the core package:

```shell
dotnet add package RestFetch.Core
```

For dependency-injection integration with `IHttpClientFactory`, also install:

```shell
dotnet add package RestFetch.Extensions
```

The repository currently targets `net11.0`; use a .NET SDK and target framework compatible with the package version you install. The core package provides the runtime API and source-generator integration.

## Quick start

Declare a `partial` API class that implements `IRestFetchApi<TSelf>`. Mark each request method with an HTTP method attribute and declare how its parameters map to the request:

```csharp
using RestFetch;

public sealed record User(int Id, string Name);

[BaseAddress("https://api.example.com")]
public sealed partial class UsersApi : IRestFetchApi<UsersApi>
{
    [Get("/users/{id}")]
    public partial Task<User?> GetUser(
        [Param] int id,
        [Query("include")] string? include,
        CancellationToken cancellationToken);
}
```

Create and build the generated client, then call it like an ordinary asynchronous C# method:

```csharp
using var api = RestFetchService.For<UsersApi>().Build();

User? user = await api.GetUser(42, "profile", CancellationToken.None);
```

`BaseAddress` can also be supplied at runtime with `RestFetchService.For<UsersApi>(baseAddress)`. A method-level `[BaseAddress]` takes precedence over the runtime value, which in turn takes precedence over the type-level value.

An API can also be declared as an interface. The generator creates an `Impl` type; use that generated type when creating the client:

```csharp
[BaseAddress("https://api.example.com")]
public partial interface IUsersApi : IRestFetchApi<IUsersApi.Impl>
{
    [Get("/users/{id}")]
    Task<User?> GetUser([Param] int id);
}

using var api = RestFetchService.For<IUsersApi.Impl>().Build();
```

## Map parameters to the request

RestFetch separates the main parts of an HTTP request into attributes:

```csharp
[Header("Accept", "application/json")]
public sealed partial class UsersApi : IRestFetchApi<UsersApi>
{
    [Get("/users/{id}")]
    public partial Task<User?> GetUser(
        [Param] int id,
        [Query("include")] string? include,
        [Header("Authorization")] string authorization,
        [Metadata("traceId")] string traceId,
        CancellationToken cancellationToken);

    [Post("/users")]
    public partial Task<User?> Create([Body] User user);
}
```

- `[Param]` fills a `{placeholder}` in the path. By default, the parameter name must match; `[Param("userId")]` specifies an alias. Values are URL-encoded unless `UrlEncode = false`.
- `[Query]` creates query-string values. It uses the C# parameter name as the key; use `[Query("q")]` to provide another key. `ValueOnly = true` omits the key.
- `[Header]` supports a named `string` header or a collection of header key/value pairs. Type- and method-level headers are static.
- `[Metadata]` stores request-scoped values for hooks; metadata is not sent as an HTTP header.
- `[Body]` marks the request body. Built-in serializers are selected for common types, with JSON as the fallback.
- `CancellationToken` is accepted without an attribute.

An argument can have only one RestFetch parameter attribute. A method can have only one `[Body]` parameter.

## Customize serialization

Choose a built-in body serializer with `[Body<TSerializer>]`:

```csharp
[Post("/users")]
public partial Task<User?> CreateBuffered(
    [Body<RequestBodySerializer.JsonBuffered>] User user);
```

For a custom body format, implement `IRequestBodySerializer<TBody, TContent>` and select it on the parameter:

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

### Buffer JSON for retries with `JsonBody<T>`

Use `JsonBody<T>` when a request may be retried and you want to avoid serializing the same object on every attempt. It is also accepted by the ordinary `[Body]` attribute:

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

`JsonBody<T>` implicitly converts from `T`, so the call can pass a `User` directly. The JSON is buffered on the first attempt and reused by retries. The request method takes ownership of a `JsonBody<T>` argument by default and disposes it when the request finishes; do not wrap it in `using` or reuse an explicit wrapper after passing it to a request. For explicit construction, `new JsonBody<User>(update, initialCapacity: 32 * 1024)` lets you set the initial buffer capacity. You can also set `[Body(JsonContext = typeof(UserJsonContext))]` to use source-generated JSON metadata.

Implement `IParamSerializer<TValue, TResult>` or `IParamEncodeSerializer<TValue, TResult>` for path values. Implement `IQueryStringSerializer<TValue>` for value-only query serialization or `IAliasQueryStringSerializer<TValue>` when the serializer also needs the query key.

## Choose and customize response deserialization

The return type selects a built-in response deserializer:

```csharp
[Get("/users/{id}")]
public partial Task<User?> GetUser([Param] int id);

[Get("/users/{id}")]
public partial Task<ApiResponse<User?>> GetUserWithStatus([Param] int id);

[Get("/users/{id}")]
public partial Task<HttpResponseMessage> GetRawResponse([Param] int id);
```

Built-in response types include JSON objects, `string`, `Stream`, `JsonNode`, `JsonDocument`, `HttpResponseMessage`, `ApiResponse<T>`, and `IAsyncEnumerable<T>`. Use a nullable result such as `Task<User?>` when an empty response is possible. The caller must dispose returned streams, raw responses, and `JsonDocument` instances.

To supply a custom response parser, implement `IResponseBodyDeserializer<TResponse, TResult>`:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class PlainTextDeserializer
    : IResponseBodyDeserializer<HttpResponseMessage, string>
{
    public static async Task<string?> Parse(
        HttpResponseMessage response,
        ApiContext context,
        JsonSerializerContext? jsonContext,
        CancellationToken cancellationToken)
    {
        return await response.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

[Response<PlainTextDeserializer>]
[Get("/status")]
public partial Task<string?> GetStatus();
```

The open-generic `IResponseBodyDeserializer<TResponse>` form is available when one parser supports multiple result types. For a streaming response, implement `IResponseAsyncEnumerableDeserializer<TResponse>` or its result-specific form; those parsers yield `SseEventResult<T>` values so per-event failures can be handled without conflating them with request-level failures.

For System.Text.Json source generation, pass a `JsonSerializerContext` through `JsonContext`:

```csharp
using System.Text.Json.Serialization;

[JsonSerializable(typeof(User))]
internal sealed partial class ApiJsonContext : JsonSerializerContext { }

[Post("/users")]
public partial Task<User?> Create(
    [Body(JsonContext = typeof(ApiJsonContext))] User user);

[Get("/users/{id}")]
[Response(JsonContext = typeof(ApiJsonContext))]
public partial Task<User?> GetUser([Param] int id);
```

## Add request and response hooks

Hooks are static implementations of the hook interfaces. They can be attached at API-group or method scope; method-level configuration overrides group-level configuration where both apply.

```csharp
[BeforeSend<RequestHooks>]
[AfterResponse<RequestHooks>]
[OnSuccess<RequestHooks>]
[OnError<RequestHooks>]
public sealed partial class UsersApi : IRestFetchApi<UsersApi>
{
    [Get("/users/{id}")]
    public partial Task<User?> GetUser([Param] int id);
}

public sealed class RequestHooks :
    IBeforeSendHook,
    IAfterResponseHook,
    IOnSuccess,
    IOnError
{
    public static Task BeforeSend(
        HttpContent? body,
        ApiContext context,
        ResourceManager resourceManager,
        CancellationToken token)
    {
        Console.WriteLine($"Sending {context.MethodName}: {context.Url}");
        return Task.CompletedTask;
    }

    public static Task AfterResponse(
        HttpResponseMessage response,
        ApiContext context,
        ResourceManager resourceManager,
        CancellationToken token)
    {
        Console.WriteLine((int)response.StatusCode);
        return Task.CompletedTask;
    }

    public static Task OnSuccess<T>(
        T? body,
        ApiContext context,
        CancellationToken token)
    {
        Console.WriteLine($"Completed {context.MethodName}");
        return Task.CompletedTask;
    }

    public static Task OnError(
        Exception? error,
        ApiContext context,
        CancellationToken token)
    {
        Console.Error.WriteLine(error);
        return Task.CompletedTask;
    }
}
```

Use `[ExceptionFactory<T>]` to map a response to an exception. Return `null` to continue without throwing; the default factory throws `RestFetchApiException` for unsuccessful status codes. Use `[OnSseEventError<T>]` to decide whether to skip a failed SSE event (`true`) or stop and route the exception to request-level error handling (`false`).

`[IdGenerator<T>]` selects a custom `IRequestIdGenerator`. Hooks receive the request's `ApiContext`; register disposable intermediate resources with the `ResourceManager` argument when they must live until request cleanup.

## Response streams and SSE

`IAsyncEnumerable<T>` can represent a JSON array or a server-sent event stream; the response `Content-Type` determines which built-in parser is used. To receive SSE metadata as well as `data`, use `RestFetch.WebUtilities.SseEvent<T>`:

```csharp
using RestFetch.WebUtilities;

[Get("/events")]
public partial IAsyncEnumerable<SseEvent<Notification>> Watch(
    [Query] int userId,
    CancellationToken cancellationToken);
```

The request begins when enumeration starts. `SseEvent<T>` exposes `Id`, `Event`, `Retry`, and `Data`; the .NET `SseItem<T>` type is also supported.

## Configure retry and timeout

Compile-time settings can be placed on the API group or an individual method:

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

Runtime options are configured before `Build()`:

```csharp
using var api = RestFetchService.For<UsersApi>()
    .AddRetry(new RetryOptions<HttpResponseMessage>(MaxRetryAttempts: 5))
    .AddTimeout(new TimeoutOptions(AttemptTimeout: 3, TotalTimeout: 15))
    .Build();
```

The default retry predicate handles HTTP 408, 429, 500, 502, 503, and 504 responses, plus selected transport and timeout failures. It rejects `POST`, `PATCH`, and `CONNECT` unless `AllowUnsafeHttpMethod` is enabled. It also checks body replayability: use `JsonBody<T>` or `IStreamFactory` for replayable content. A non-seekable `Stream` remains unsafe even when `AllowUnsafeRequestBody` is enabled. Method-level runtime configuration is available through `AddRetryFor` and `AddTimeoutFor`.

Use `OnRetry` to observe each retry attempt:

```csharp
using Polly.Retry;

[Retry(4, OnRetry = typeof(RetryLogger))]
public sealed partial class UsersApi : IRestFetchApi<UsersApi>
{
    [Get("/users/{id}")]
    public partial Task<User?> GetUser([Param] int id);
}

public sealed class RetryLogger : IOnRetry
{
    public static ValueTask OnRetry(
        OnRetryArguments<HttpResponseMessage> arguments)
    {
        if (arguments.Context.Properties.TryGetValue(
                UsersApi.ApiContextKey,
                out var context))
        {
            Console.WriteLine(context.MethodName);
        }

        return ValueTask.CompletedTask;
    }
}
```

## Use another HTTP transport

Configure the built-in proxy with a custom handler:

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

To replace `HttpClient`, implement `IHttpClientProxy<TSelf, TResponse>` and apply `[HttpClientProxy<TProxy>]` to the API group. The response type is part of `IRestFetchApi<TSelf, TResponse>`, so custom transports can use a response type other than `HttpResponseMessage`.

When passing an `IHttpClientProxyService<TResponse>` to `RestFetchService.For`, the API treats it as shared and does not dispose it. Dispose an externally created proxy separately, as in the example.

For ASP.NET Core dependency injection, add `RestFetch.Extensions`:

```csharp
using RestFetch.Extensions;

services.AddRestFetchApi<UsersApi>("https://api.example.com");
```

The returned `IHttpClientBuilder` can be used to configure the named `HttpClient`. The extension package also provides interface and keyed-service registration overloads.

## Learn more

The [API guide](API.md) contains the attribute reference, exact serializer and hook contracts, retry behavior, transport options, utility types, and a public-type index. Working examples are in [`src/RestFetch.Examples`](src/RestFetch.Examples).