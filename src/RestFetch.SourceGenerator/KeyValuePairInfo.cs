using System.Runtime.CompilerServices;


namespace RestFetch.SourceGenerator;

public record KeyValuePairInfo (
	string Key,
	string? Value,
	EquatableArray<DiagnosticInfo>? Diagnostics
) {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out string key, out string? value) {
		key = Key;
		value = Value;
	}
};
