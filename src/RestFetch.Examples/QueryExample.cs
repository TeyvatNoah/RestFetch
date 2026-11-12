using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using RestFetch;
using RestFetch.WebUtilities;
namespace RestFetch.Examples;

public sealed partial class C10: IRestFetchApi<C10> {
	[Get("/user")]
	public partial Task<User> GetUserById0([Query]int id);

	[Get("/user")]
	public partial Task<User> GetUserById1([Query("uid")]int id);

	[Get("/user")]
	public partial Task<User> GetUserById2([Query]int? id);

	[Get("/user")]
	public partial Task<User> GetUserById3([Query<QueryString>]int? id);

	[Get("/user")]
	public partial Task<User> GetUserById4([Query(ValueOnly = false)]int id);

	[Get("/user")]
	public partial Task<User> GetUserById5([Query(ValueOnly = false)]string name);
}

public static class QueryExample {
	public static async Task Main() {
		var handler = new SocketsHttpHandler() {
			Proxy = new WebProxy("http://127.0.0.1:2333", false),
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
		using var client = RestFetchService.For<C10>(new HttpClientProxy(handler) {
			BaseAddress = "http://127.0.0.1"
		})
		.Build();
		// var client = RestFetchService.For<C10>();
		// var client = RestFetchService.For<C10>("http://127.0.0.1");
		var user = await client.GetUserById5("adfasdf");
		Console.WriteLine(user.Name);
	}
}