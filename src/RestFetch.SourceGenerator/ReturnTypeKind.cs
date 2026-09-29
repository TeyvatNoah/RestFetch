namespace RestFetch.SourceGenerator;

public enum ReturnTypeKind {
	NotAsync,
	Void,
	Task,
	TaskT,
	ValueTask,
	ValueTaskT,
	IAsyncEnumerableT,
	CustomTask
}