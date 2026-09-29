using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace RestFetch.SourceGenerator;

public readonly record struct HookInfo(
	// 完全限定类名,不带global::
	string FullyQualifiedName,
	bool ConfigAwait,
	EquatableArray<DiagnosticInfo>? Diagnostics
);