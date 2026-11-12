using System.Net;

namespace RestFetch;

public sealed class RestFetchApiException(string methodName, RequestId id, Version version, string? msg): Exception(msg) {
	public readonly RequestId Id = id;

	public readonly string MethodName = methodName;

	public readonly HttpStatusCode StatusCode;

	public readonly string? ReasonPhrase = msg;

	public readonly Version Version = version;

	public string? ResponseBody { get; init; }
}