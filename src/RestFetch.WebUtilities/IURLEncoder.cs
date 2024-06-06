namespace RestFetch.WebUtilities;

public interface IURLEncoder {
	static abstract string EncodeURIComponent(string? str);

	static abstract string EncodeURIComponent(ReadOnlySpan<char> str);
}