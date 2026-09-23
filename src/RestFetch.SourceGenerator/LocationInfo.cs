using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RestFetch.SourceGenerator;
public readonly record struct LocationInfo(
	string FilePath,
	TextSpan TextSpan,
	LinePositionSpan LinePositionSpan
) {
	public LocationInfo(Location location): this(
		location.SourceTree?.FilePath ?? "",
		location.SourceSpan,
		location.GetLineSpan().Span
	) {}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Location(LocationInfo locationInfo)
		=> Location.Create(locationInfo.FilePath, locationInfo.TextSpan, locationInfo.LinePositionSpan);
}
