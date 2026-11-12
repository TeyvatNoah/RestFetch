namespace RestFetch;
public interface IStreamFactory {
	string ContentType { get; }
	ValueTask<Stream> Create();
}