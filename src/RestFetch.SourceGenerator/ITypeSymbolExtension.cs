using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public static class ITypeSymbolExtension {
	public static bool ImplementsInterface(this ITypeSymbol self, INamedTypeSymbol interfaceSymbol) {
		if (self is null || interfaceSymbol is null) return false;

		// 提取其开放泛型
		var targetDefinition = interfaceSymbol.IsGenericType
		? interfaceSymbol.OriginalDefinition
		: interfaceSymbol;

		if (IsMatch(self, targetDefinition)) {
			return true;
		}

		foreach (var iface in self.AllInterfaces) {
			if (IsMatch(iface, targetDefinition)) {
				return true;
			}
		}

		return false;

		static bool IsMatch(ITypeSymbol current, INamedTypeSymbol target) {
			var currentDefinition = current is INamedTypeSymbol { IsGenericType: true } named
			? named.OriginalDefinition
			: current;

			return SymbolEqualityComparer.Default.Equals(currentDefinition, target);
		}
	}
}