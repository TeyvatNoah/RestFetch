namespace RestFetch.SourceGenerator;

public enum ReturnTypeWrapperKind {
	NotAsync,
	Void,
	Task,
	TaskT,
	ValueTask,
	ValueTaskT,
	IAsyncEnumerableT,
	IObservableT,
	CustomTask
}