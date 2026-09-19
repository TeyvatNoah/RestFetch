using System.Net;

namespace RestFetch;

public sealed class RestFetchApiException(string? msg, Version version): Exception(msg) {
	public readonly HttpStatusCode StatusCode;

	public readonly string? ReasonPhrase = msg;

	public readonly Version Version = version;

	public string? ResponseBody { get; init; }
}