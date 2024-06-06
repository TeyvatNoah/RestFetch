namespace RestFetch;

[AttributeUsage(
	AttributeTargets.Class |
	AttributeTargets.Struct |
	AttributeTargets.Interface |
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = false
	)]
public sealed class BaseAddressAttribute(string baseAddress): Attribute {
	public readonly string BaseAddress = baseAddress;
}