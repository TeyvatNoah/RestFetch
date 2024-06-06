namespace RestFetch;

[AttributeUsage(
	AttributeTargets.Class |
	AttributeTargets.Struct |
	AttributeTargets.Interface |
	AttributeTargets.Method,
	Inherited = true,
	AllowMultiple = true
	)]
public sealed class StaticHeaderAttribute(string name, string value): Attribute {
	public readonly string Name = name;

	public readonly string Value = value;
}