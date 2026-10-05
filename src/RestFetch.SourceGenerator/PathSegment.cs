using System.Diagnostics.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public abstract record PathSegment {
	public static bool TryParse(string template, [NotNullWhen(true)]out List<PathSegment>? result) {
		var segments = new List<PathSegment>();
		var span = template.AsSpan();
		var i = 0;

		while (i < span.Length) {
			var openBrace = span[i..].IndexOf('{');
			if (openBrace == -1) {
				segments.Add(new LiteralSegment(span[i..].ToString()));
				break;
			}

			var absoluteOpen = i + openBrace;
			if (absoluteOpen > i) {
				segments.Add(new LiteralSegment(span[i..absoluteOpen].ToString()));
			}

			var closeBrace = span[absoluteOpen..].IndexOf('}');
			if (closeBrace == -1) {
				result = null;
				return false;
			}

			var absoluteClose = absoluteOpen + closeBrace;
			var rawExpr = span[(absoluteOpen + 1)..absoluteClose].Trim().ToString();
			var dotIndex = rawExpr.IndexOf('.');
			var rootParam = dotIndex == -1 ? rawExpr : rawExpr[0..dotIndex];
			var propPath = dotIndex == -1 ? null : rawExpr[(dotIndex + 1)..];

			segments.Add(new ParamSegment(rawExpr, rootParam, propPath));
			i = absoluteClose + 1;
		}
		result = segments;
		return true;
	}
}

public sealed record LiteralSegment(string Text): PathSegment;

public sealed record ParamSegment(string Expr, string RootParamName, string? PropertyPath): PathSegment;