using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

using RestFetch;
using RestFetch.WebUtilities;
namespace RestFetch.Examples;

public sealed partial class C11: IRestFetchApi<C11> {
	[Post("/user")]
	public partial Task<User> CreateUser0([Body]User? user);

	[Post("/user")]
	public partial Task<User> CreateUser1([Body<RequestBodySerializer.JsonBuffered>]User user);

	[Post("/user")]
	public partial Task<User> CreateUser2([Body<RequestBodySerializer.Form>]IEnumerable<KeyValuePair<string, string?>> user);

	[Post("/user")]
	[Response<ResponseBodyDeserializer.Json>(JsonContext = typeof(UserContext))]
	public partial Task<User> CreateUser3([Body(JsonContext = typeof(UserContext))]User user);

	[Post("/user")]
	public partial Task<User> CreateUser4([Body(ConfigureAwait = true)]User user);

	[Post("/user")]
	public partial Task<User> CreateUser5([Body]string user);

	[Retry(8)]
	[Post("/user")]
	public partial Task<User> CreateUser6([Body]JsonBody<User> user);

	[Post("/user")]
	public partial Task<User> CreateUser7([Body]JsonBody<User> user);

	[Post("/upload")]
	public partial Task<User> CreateFile0([Body]RestFetchFileInfo[] file);

	[Post("/upload")]
	public partial Task<User> CreateFile1([Body<RequestBodySerializer.OctetStream>]Stream file);

	[Post("/upload")]
	public partial Task<User> CreateFile2([Body<RequestBodySerializer.MultipartFormData>]FileInfo[] file);

	[Post("/upload")]
	public partial Task<User> CreateFile3([Body<RequestBodySerializer.MultipartFormData>]RestFetchFileInfo[] file);

	[Post("/user")]
	public partial Task<User> CreateFile4([Body]FileStreamFactory? file);
}

public sealed class FileStreamFactory: IStreamFactory {
	public string ContentType { get; init; } = "application/octet-stream";
	public ValueTask<Stream> Create() => throw new NotImplementedException();
}

public static class BodyExample {
	public static async Task Main() {
		// var client = RestFetchService.For<C11>();
		var handler = new SocketsHttpHandler() {
			// Proxy = new WebProxy("http://127.0.0.1:2333", false),
			AllowAutoRedirect = true,
			AutomaticDecompression = DecompressionMethods.All,
			PooledConnectionLifetime = TimeSpan.FromMinutes(2),
			MaxConnectionsPerServer = 100,
			UseCookies = true,
			SslOptions = new() {
				EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
				CertificateRevocationCheckMode = X509RevocationMode.Online
			}
		};
		JsonSerializerOptions jsonOptions = new() {
			WriteIndented = false,
			IncludeFields = true,
			RespectNullableAnnotations = true, // 全局配置了这里就不用了
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			PropertyNamingPolicy = JsonNamingPolicy.PascalCase,
			DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
			ReferenceHandler = ReferenceHandler.IgnoreCycles,
			// TypeInfoResolver = UserContext.Default,
		};

		using var client = RestFetchService.For<C11>(new HttpClientProxy(new HttpClientProxyOptions.HttpClientProxyMessageHandlerOptions(handler) {
			SerializerOptions = jsonOptions
		}) {
			BaseAddress = "http://127.0.0.1"
		}).Build();
		var user = await client.CreateUser6(new(new(
			Id: 114514,
			Age: 24,
			Name: "senpai"
		)));
		// var user = await client.CreateUser2([
		// 	new("id", "114514"),
		// 	new("age", "24"),
		// 	new("name", "senpai"),
		// ]);

		// var user = await client.CreateFile2([
		// 	new("Nuget.Config"),
		// 	new("Directory.Packages.props")
		// ]);
		// var user = await client.CreateFile0([
		// 	new( "aaa", "bbb.xml", new("Nuget.Config")),
		// 	new( "ccc", "ddd.xml", new("Directory.Packages.props"))
		// ]);
		// var user = await client.CreateUser5(@"{""name"": ""aaa""}");
		Console.WriteLine(user.Name);
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(User))]
internal sealed partial class UserContext: JsonSerializerContext {}