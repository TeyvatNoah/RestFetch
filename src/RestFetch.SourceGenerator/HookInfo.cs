using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace RestFetch.SourceGenerator;

public sealed record HookInfo(
	// 完全限定类名,不带global::
	string FullyQualifiedName,
	EquatableArray<DiagnosticInfo> Diagnostics
);