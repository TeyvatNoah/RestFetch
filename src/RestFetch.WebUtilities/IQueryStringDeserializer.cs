using System.Diagnostics.CodeAnalysis;

namespace RestFetch.WebUtilities;

// TODO out参数不支持协变,如果有天支持了记得加上
// public interface IQueryStringDeserializer<out T> {
public interface IQueryStringDeserializer<T> where T: allows ref struct {
	static abstract T Parse(string? str);
	static abstract bool TryParse(string? str, [NotNullWhen(true)]out T? retval);
	// static abstract T ParseUTF8String(byte[]? str);
	// static abstract bool TryParseUTF8String(byte[]? str, out T retval);
}