using System.Net;

namespace RestFetch;

public sealed record ApiResponse<T>(
	HttpStatusCode StatusCode,
	string? ReasonPhrase,
	string Url,
	T? Content,
	IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Headers
);

