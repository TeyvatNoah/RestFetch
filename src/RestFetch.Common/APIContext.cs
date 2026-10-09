using System.Runtime.CompilerServices;

namespace RestFetch;


public struct APIContext(
	RequestId id,
	object client,
	string methodName,
	string className,
	string fullName,
	HttpMethod httpMethod,
	// object? options,
	Dictionary<string, object>? metadata
	) {
	// 请求id
	public readonly RequestId ID = id;
	public string URL = default!;
	public readonly object Client = client;
	// API 方法名 TODO 注意泛型情况
	public readonly string MethodName = methodName;
	// API 所属接口名,非完全限定 TODO 注意泛型情况
	public readonly string ClassName = className;
	// API 所属接口名,完全限定 TODO 注意泛型情况
	public readonly string FullName = fullName;
	// HTTP Method
	public readonly HttpMethod HttpMethod = httpMethod;
	// TODO RestFetch选项
	// public readonly object? Options = options;
	public readonly Dictionary<string, object>? Metadata = metadata;
}