namespace RestFetch;

// TODO 实现为对象池
// NOTE: 因为会跨await边界传递,所以会跟着状态机上堆
public sealed class ApiContext(
	RequestId id,
	object client,
	string methodName,
	string className,
	bool returnsAsyncEnumerable,
	HttpMethod httpMethod,
	object? options,
	Dictionary<string, object>? metadata,
	ReturnTypeInfo returnTypeInfo
	) {
	// 请求id
	public readonly RequestId Id = id;
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
	public Dictionary<string, object>? Metadata = metadata;

	// 请求方法是否返回IAsyncEnumerable<T>
	// IHttpClientProxy.SendAsync()可以根据此字段决定HttpCompletionOption.ResponseHeadersRead
	public readonly bool ReturnsAsyncEnumerable = returnsAsyncEnumerable;

	// 待构造的URL
	public string Url = default!;

	// 待构造的Header
	public IEnumerable<KeyValuePair<string, string>>? Headers;

	// 当前尝试次数
	// public int Attempts;

	public readonly ReturnTypeInfo ReturnTypeInfo = returnTypeInfo;
	// 是否配置了Retry供序列化器决定是否需要buffered
	// public bool Retry = false
}

