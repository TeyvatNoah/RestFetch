using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text;

using Cysharp.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using static RestFetch.SourceGenerator.Util;


namespace RestFetch.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class RestFetchSourceGenerator: IIncrementalGenerator {
	// private static readonly string AssemblyNameSpaceName = nameof(RestFetch);

	public static readonly string RestFetchNamespaceName = nameof(RestFetch);

	private static readonly Assembly RestFetchSourceGeneratorAssembly = typeof(RestFetchSourceGenerator).Assembly;


	private static readonly string[] EmbedFiles = [
		"RestFetch.AfterResponseAttribute.cs",
		"RestFetch.BaseAddressAttribute.cs",
		"RestFetch.BeforeSendAttribute.cs",
		"RestFetch.BodyAttribute.cs",
		"RestFetch.ExceptionFactoryAttribute.cs",
		"RestFetch.HeaderAttribute.cs",
		"RestFetch.HttpClientProxyAttribute.cs",
		"RestFetch.HttpMethodAttribute.cs",
		"RestFetch.IdGeneratorAttribute.cs",
		"RestFetch.MetadataAttribute.cs",
		"RestFetch.OnErrorAttribute.cs",
		"RestFetch.OnSseEventErrorAttribute.cs",
		"RestFetch.OnSuccessAttribute.cs",
		"RestFetch.ParamAttribute.cs",
		"RestFetch.QueryAttribute.cs",
		"RestFetch.ResponseAttribute.cs",
	];

	private static readonly string[] HttpAttributeMetadataNames = [
		$"{RestFetchNamespaceName}.GetAttribute",
		$"{RestFetchNamespaceName}.PostAttribute",
		$"{RestFetchNamespaceName}.PutAttribute",
		$"{RestFetchNamespaceName}.PatchAttribute",
		$"{RestFetchNamespaceName}.DeleteAttribute",
		$"{RestFetchNamespaceName}.HeadAttribute",
		$"{RestFetchNamespaceName}.OptionsAttribute",
		$"{RestFetchNamespaceName}.ConnectAttribute",
		$"{RestFetchNamespaceName}.TraceAttribute",
	];



	public void Initialize(IncrementalGeneratorInitializationContext context) {
		context.RegisterPostInitializationOutput(static i => {
			i.AddEmbeddedAttributeDefinition();
			foreach (var file in EmbedFiles) {
				i.AddSource(file, ReadFileFromAssembly(RestFetchSourceGeneratorAssembly, file));
			}
		});

		// if (!Debugger.IsAttached){

		// 	Debugger.Launch();
		// }

		// 取得所有[Get]等http attribute修饰的方法所属类
		var apiDefinitionSymbolProviders = HttpAttributeMetadataNames.Select(attrName =>
			context.SyntaxProvider.ForAttributeWithMetadataName(
				attrName,
				static (node, _) => node is MethodDeclarationSyntax,
				static (ctx, _) => ctx.TargetSymbol.ContainingType)
				.Where(NotNullRefType)
				.WithComparer(SymbolEqualityComparer.Default)
				.Collect());
		var apiDefinitionSymbolProvider = apiDefinitionSymbolProviders
			.Aggregate(static (acc, next)
				=> acc.Combine(next)
					.Select(static (pair, _) => pair.Left.AddRange(pair.Right)));
		// 去重
		var candidateApiDefinitionSymbols = apiDefinitionSymbolProvider
			.SelectMany(static (item, _) => item.Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default))
			.WithComparer(SymbolEqualityComparer.Default);

		var knownTypesProvider = context.CompilationProvider.Select(static (compilation, _) => new KnownTypes(compilation));
		var apiDefinitionInfoProvider = candidateApiDefinitionSymbols
			.Combine(knownTypesProvider)
			.Select(static (pair, _) => {
				var symbol = pair.Left;
				var knownTypes = pair.Right;
				var attr = symbol.GetAttributes();
				return ApiDefinitionTypeInfo.CreateFromSymbol(symbol, knownTypes);
			});

		var validApiDefinitionProvider = apiDefinitionInfoProvider.Where(static e => e.IsValid);
		var partialTypeProvider = validApiDefinitionProvider.Where(static e => e.TypeInfo.IsPartial
			&& (e.TypeInfo.TypeKind == TypeKind.Class || e.TypeInfo.TypeKind == TypeKind.Struct));
		var interfaceProvider = validApiDefinitionProvider.Where(static e => e.TypeInfo.IsPartial && e.TypeInfo.IsInterface);

		var diagnosticProvider = apiDefinitionInfoProvider
			.Where(static apiDefinitionInfo => apiDefinitionInfo.Diagnostics is not null)
			.SelectMany(static (apiDefinitionInfo, _) => apiDefinitionInfo.Diagnostics ?? Array.Empty<DiagnosticInfo>().ToImmutableArray());

		context.RegisterImplementationSourceOutput(partialTypeProvider, static (ctx, apiDefinition) => {
			ctx.AddSource($"{apiDefinition.TypeInfo.FullyQualifiedName.Replace("<", null).Replace(">", null).Replace(",", null)}.cs", SourceText.From(Emitter.GenerateSource(apiDefinition), Encoding.UTF8));
		});
		context.RegisterSourceOutput(interfaceProvider, static (ctx, apiDefinition) => {
			ctx.AddSource($"{apiDefinition.TypeInfo.FullyQualifiedName.Replace("<", null).Replace(">", null).Replace(",", null)}.cs", SourceText.From(Emitter.GenerateSource(apiDefinition), Encoding.UTF8));
		});

		context.RegisterSourceOutput(diagnosticProvider, static (ctx, data) =>
			// if (!Debugger.IsAttached){
			// 	Debugger.Launch();
			// }
			ctx.ReportDiagnostic(data)
		);
	}
}

