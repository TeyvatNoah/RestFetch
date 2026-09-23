
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;
public sealed record DiagnosticInfo(
		DiagnosticDescriptor Descriptor,
		LocationInfo? Location,
		EquatableArray<string>? MessageArgs
) {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Diagnostic(DiagnosticInfo diagnosticInfo)
		=> Diagnostic.Create(diagnosticInfo.Descriptor, diagnosticInfo.Location ?? Microsoft.CodeAnalysis.Location.None, diagnosticInfo.MessageArgs?.ToArray());
}
