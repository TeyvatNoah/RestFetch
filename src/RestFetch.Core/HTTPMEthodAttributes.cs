namespace RestFetch;

public abstract class HTTPMethodAttribute: Attribute {
	public readonly string Path;

	public readonly HttpMethod Method;

	internal HTTPMethodAttribute(string path, HttpMethod method) {
		Path = path;
		Method = method;
	}
}

[AttributeUsage(
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = false
	)]
public sealed class GetAttribute(string path) : HTTPMethodAttribute(path, HttpMethod.Get) {
	public GetAttribute(): this("/") {}
}

[AttributeUsage(
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = false
	)]
public sealed class PostAttribute(string path) : HTTPMethodAttribute(path, HttpMethod.Post) {
	public PostAttribute(): this("/") {}
}

[AttributeUsage(
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = false
	)]
public sealed class DeleteAttribute(string path) : HTTPMethodAttribute(path, HttpMethod.Delete) {
	public DeleteAttribute(): this("/") {}
}

[AttributeUsage(
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = false
	)]
public sealed class PatchAttribute(string path) : HTTPMethodAttribute(path, HttpMethod.Patch) {
	public PatchAttribute(): this("/") {}
}

[AttributeUsage(
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = false
	)]
public sealed class PutAttribute(string path) : HTTPMethodAttribute(path, HttpMethod.Put) {
	public PutAttribute(): this("/") {}
}

[AttributeUsage(
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = false
	)]
public sealed class OptionsAttribute(string path) : HTTPMethodAttribute(path, HttpMethod.Options) {
	public OptionsAttribute(): this("/") {}
}

[AttributeUsage(
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = false
	)]
public sealed class HeadAttribute(string path) : HTTPMethodAttribute(path, HttpMethod.Head) {
	public HeadAttribute(): this("/") {}
}

[AttributeUsage(
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = false
	)]
public sealed class ConnectAttribute(string path) : HTTPMethodAttribute(path, HttpMethod.Connect) {
	public ConnectAttribute(): this("/") {}
}

[AttributeUsage(
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = false
	)]
public sealed class TraceAttribute(string path) : HTTPMethodAttribute(path, HttpMethod.Trace) {
	public TraceAttribute(): this("/") {}
}