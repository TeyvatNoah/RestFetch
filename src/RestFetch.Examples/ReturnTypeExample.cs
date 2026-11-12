using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

using RestFetch.WebUtilities;
namespace RestFetch.Examples;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
[OnError<ErrorHk>]
[OnSuccess<OnsuccessHkk>]
[BaseAddress("http://127.0.0.1")]
public sealed partial class C19: IRestFetchApi<C19> {
	[Get("/user")]
	public partial Task GetUserById0([Query]int id);

	[Get("/user")]
	[Response(SkipNullableCheck = true)]
	public partial Task<User> GetUserById1([Query]int id);

	[Get("/user")]
	public partial Task<string> GetUserById2([Query]int id);

	[Get("/user")]
	public partial Task<Stream> GetUserById3([Query]int id);

	[Get("/user")]
	public partial Task<HttpResponseMessage> GetUserById4([Query]int id);

	[Get("/user")]
	public partial Task<JsonNode> GetUserById5([Query]int id);

	[Get("/user")]
	public partial Task<JsonDocument> GetUserById6([Query]int id);

	[Get("/user")]
	public partial Task<ApiResponse<JsonNode?>> GetUserById7([Query]int id);

	[Get("/user")]
	// [Response<ResponseBodyDeserializer.ApiResponse<string>>(SkipNullableCheck = true)]
	public partial Task<ApiResponse<string?>> GetUserById8([Query]int id);

	[Get("/user")]
	public partial Task<ApiResponse<User?>> GetUserById9([Query]int id);

	[Get("/user")]
	public partial ValueTask GetUserById10([Query]int id);

	[Response<ResponseBodyDeserializer.Json>]
	[Get("/user")]
	public partial ValueTask<User?> GetUserById11([Query]int id);

	[Get("/user")]
	public partial ValueTask<string> GetUserById12([Query]int id);

	[Get("/user")]
	public partial ValueTask<Stream> GetUserById13([Query]int id);

	[Get("/user")]
	public partial ValueTask<HttpResponseMessage> GetUserById14([Query]int id);

	[Get("/user")]
	public partial ValueTask<JsonNode> GetUserById15([Query]int id);

	[Get("/user")]
	public partial ValueTask<JsonDocument> GetUserById16([Query]int id);

	[Get("/user")]
	public partial ValueTask<ApiResponse<JsonNode?>> GetUserById17([Query]int id);

	[Get("/user")]
	// [Response<ResponseBodyDeserializer.ApiResponse<string>>(SkipNullableCheck = true)]
	public partial ValueTask<ApiResponse<string?>> GetUserById18([Query]int id);

	[Get("/user")]
	public partial ValueTask<ApiResponse<User?>> GetUserById19([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<string> GetUsers0([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<JsonNode?> GetUsers1([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<JsonDocument> GetUsers2([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<SseEvent> GetUsers3([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<SseEvent<string?>> GetUsers4([Query]int id);
	

	[Get("/users")]
	public partial IAsyncEnumerable<SseEvent<JsonNode?>> GetUsers5([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<SseEvent<JsonDocument?>> GetUsers6([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<SseItem<string?>> GetUsers7([Query]int id);
	
	[Get("/users")]
	public partial IAsyncEnumerable<SseItem<JsonNode?>> GetUsers8([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<SseItem<JsonDocument?>> GetUsers9([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<SseEvent<User?>> GetUsers10([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<SseItem<User?>> GetUsers11([Query]int id);

	[Get("/users")]
	public partial IAsyncEnumerable<User?> GetUsers12([Query]int id);

	[Get("/usersarray")]
	public partial IAsyncEnumerable<User?> GetUsers13([Query]int id);

	[OnSseEventError<SseEventErrHook>]
	[Get("/userserr")]
	public partial IAsyncEnumerable<User?> GetUsers14([Query]int id);












	






	
}

public sealed class OnsuccessHkk: IOnSuccess {
	public static Task OnSuccess<TResponseBody>(TResponseBody? body, ApiContext context, CancellationToken token) {
		Console.WriteLine("success");
		return Task.CompletedTask;
	}
}

public sealed class ErrorHk: IOnError {
	public static Task OnError(Exception? e, ApiContext context, CancellationToken token) {
		Console.WriteLine("on error");
		return Task.CompletedTask;
	}
}

public sealed class SseEventErrHook: IOnSseEventError {
	public static Task<bool> OnSseEventError(Exception exp, ApiContext context, CancellationToken token) {
		Console.WriteLine("sse event error");
		return Task.FromResult(true);
	}
}


public static class ReturnTypeExample {
	public static async Task Main() {
		using var client = RestFetchService.For<C19>().Build();
		// await client.GetUserById0(114514);
		// var u1 = await client.GetUserById1(114514);
		// Console.WriteLine(u1.Name);
		// var u2 = await client.GetUserById2(114514);
		// Console.WriteLine(u2);
		// using var u3 = await client.GetUserById3(114514);
		// using var reader = new StreamReader(u3);
		// Console.WriteLine(await reader.ReadToEndAsync());
		using var u4 = await client.GetUserById4(114514);
		Console.WriteLine(await u4.Content.ReadAsStringAsync());
		var u5 = await client.GetUserById5(114514);
		Console.WriteLine(u5["name"]);
		using var u6 = await client.GetUserById6(114514);
		Console.WriteLine(u6.RootElement.GetProperty("name"));
		var u7 = await client.GetUserById7(114514);
		Console.WriteLine(u7.Content?["name"]);
		var u8 = await client.GetUserById8(114514);
		Console.WriteLine(u8);
		var u9 = await client.GetUserById9(114514);
		Console.WriteLine(u9.Content?.Name);

		// await client.GetUserById10(114514);
		// var u11 = await client.GetUserById11(114514);
		// Console.WriteLine(u11.Name);
		// var u12 = await client.GetUserById12(114514);
		// Console.WriteLine(u12);
		// using var u13 = await client.GetUserById13(114514);
		// using var reader = new StreamReader(u13);
		// Console.WriteLine(await reader.ReadToEndAsync());
		// using var u14 = await client.GetUserById14(114514);
		// Console.WriteLine(await u14.Content.ReadAsStringAsync());
		// var u15 = await client.GetUserById15(114514);
		// Console.WriteLine(u15["name"]);
		// using var u16 = await client.GetUserById16(114514);
		// Console.WriteLine(u16.RootElement.GetProperty("name"));
		// var u17 = await client.GetUserById17(114514);
		// Console.WriteLine(u17.Content?["name"]);
		// var u18 = await client.GetUserById18(114514);
		// Console.WriteLine(u18);
		// var u19 = await client.GetUserById19(114514);
		// Console.WriteLine(u19.Content?.Name);
		// await foreach (var item in client.GetUsers0(114514)) {
		// 	Console.WriteLine(item);
		// }

		// await foreach (var item in client.GetUsers1(114514)) {
		// 	Console.WriteLine(item["name"]);
		// }

		// await foreach (var item in client.GetUsers2(114514)) {
		// 	Console.WriteLine(item.RootElement.GetProperty("name"));
		// }

		// await foreach (var item in client.GetUsers3(114514)) {
		// 	Console.WriteLine(item.Data);
		// }

		// await foreach (var item in client.GetUsers4(114514)) {
		// 	Console.WriteLine(item.Data);
		// }

		// await foreach (var item in client.GetUsers5(114514)) {
		// 	Console.WriteLine(item.Data["name"]);
		// }

		// await foreach (var item in client.GetUsers6(114514)) {
		// 	Console.WriteLine(item.Data.RootElement.GetProperty("name"));
		// }

		// await foreach (var item in client.GetUsers7(114514)) {
		// 	Console.WriteLine(item.Data);
		// }

		// await foreach (var item in client.GetUsers8(114514)) {
		// 	Console.WriteLine(item.Data["name"]);
		// }

		// await foreach (var item in client.GetUsers9(114514)) {
		// 	Console.WriteLine(item.Data.RootElement.GetProperty("name"));
		// }

		// await foreach (var item in client.GetUsers10(114514)) {
		// 	Console.WriteLine(item.Data.Name);
		// }

		// await foreach (var item in client.GetUsers11(114514)) {
		// 	Console.WriteLine(item.Data.Name);
		// }

		// await foreach (var item in client.GetUsers12(114514)) {
		// 	Console.WriteLine(item.Name);
		// }

		// await foreach (var item in client.GetUsers13(114514)) {
		// 	Console.WriteLine(item.Name);
		// }

		// await foreach (var item in client.GetUsers14(114514)) {
		// 				Console.WriteLine(item.Name);
		// 		}
		
	}
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.