using System.Runtime.CompilerServices;

namespace RestFetch;


public struct ApiContext(
	RequestId id,
	object client,
	string methodName,
	string className,
	bool returnsAsyncEnumerable,
	HttpMethod httpMethod,
	object? options,
	Dictionary<string, object>? metadata
	) {
	// 请求id
	public readonly RequestId Id = id;
	// 请求url
	public string Url = default!;
	// 内部使用的IHttpClientProxy实例
	public readonly object Client = client;
	// 构建IHttpClientProxy实例的选项
	public readonly object? Options = options;
	// API 方法名
	public readonly string MethodName = methodName;
	// API 所属类型名,完全限定名
	public readonly string ClassName = className;
	// HTTP Method
	public readonly HttpMethod HttpMethod = httpMethod;
	// Metadata
	public readonly Dictionary<string, object>? Metadata = metadata;

	// 请求方法是否返回IAsyncEnumerable<T>
	// IHttpClientProxy.SendAsync()可以根据此字段决定HttpCompletionOption.ResponseHeadersRead
	public readonly bool ReturnsAsyncEnumerable = returnsAsyncEnumerable;
}

