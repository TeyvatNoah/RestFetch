using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace RestFetch.SourceGenerator;

public record KeyValuePairInfo (
	string Key,
	string? Value,
	EquatableArray<DiagnosticInfo> Diagnostics
);
