using System.Net;
using System.Net.Http.Headers;

namespace RestFetch;

public sealed record ApiResponse<T>(
	HttpStatusCode StatusCode,
	string? ReasonPhrase,
	string Url,
	T? Content,
	IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Headers
	// 选这两个类型是否会和HttpClient耦合太多了些?
	// HttpContentHeaders? ContentHeaders,
	// HttpResponseHeaders? ResponseHeaders
);

