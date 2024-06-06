namespace RestFetch.WebUtilities;

public interface IURLDecoder {
	static abstract string DecodeURIComponent(string? str);

	static abstract string DecodeURIComponent(ReadOnlySpan<char> str);
}