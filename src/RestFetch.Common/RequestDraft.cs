namespace RestFetch;

public record struct RequestDraft(
	// 待构造的URL
	string Url,
	// 待构造的Header
	IEnumerable<KeyValuePair<string, string>>? Headers,
	// 当前尝试次数
	int Attempts = 0,
	// 是否配置了Retry供序列化器决定是否需要buffered
	bool Retry = false
);