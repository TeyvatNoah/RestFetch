namespace RestFetch.SourceGenerator;

public enum ApiParameterKind {
	None,
	Body,
	Header,
	QueryString,
	Param,
	Metadata,
	Cancellation
}