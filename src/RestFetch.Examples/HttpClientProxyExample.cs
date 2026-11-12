namespace RestFetch.Examples;

[HttpClientProxy<HttpClientProxyDemo>]
public sealed partial class C12: IRestFetchApi<C12, HttpResponseMessage> {
	[Get("/user")]
	public partial Task GetUserById([Query]int id);

}

public sealed class HttpClientProxyDemo(string? baseAddress) : IHttpClientProxy<HttpClientProxyDemo, HttpResponseMessage> {
	public string? BaseAddress { get; set; } = baseAddress;
	public object? Options { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

	public static HttpClientProxyDemo Create(string? baseAddress) => new HttpClientProxyDemo(baseAddress);
	public static HttpClientProxyDemo Create(HttpClient client) => new HttpClientProxyDemo("");
		// static IHttpClientProxy<HttpClient, HttpResponseMessage> IHttpClientProxy<HttpClient, HttpResponseMessage>.Create(string baseAddress) => this.Create(baseAddress);
		// static IHttpClientProxy<HttpClient, HttpResponseMessage> IHttpClientProxy<HttpClient, HttpResponseMessage>.Create(HttpClient client) => this.Create(client);

		public void Dispose() {}
	public Task<HttpResponseMessage> SendAsync<TRequestBodyContent>(TRequestBodyContent? body, ApiContext context, ResourceManager resourceManager, CancellationToken token) {
		Console.WriteLine("send request");
		return Task.FromResult<HttpResponseMessage>(new());
	}
}

public static class HttpClientProxyExample {
	public static async Task Main() {
		using var client = RestFetchService<HttpClient, HttpResponseMessage>.For<C12>(new HttpClientProxyDemo("http://127.0.0.1")).Build();
		await client.GetUserById(114514);
		Console.WriteLine("completed.");
	}
}
