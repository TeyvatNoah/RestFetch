using System.Reflection;

using Cysharp.Text;

using Microsoft.CodeAnalysis;

using static RestFetch.SourceGenerator.Util;


namespace RestFetch.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class RestFetchSourceGenerator: IIncrementalGenerator {
	// private static readonly string AssemblyNameSpaceName = nameof(RestFetch);

	private static readonly Assembly RestFetchSourceGeneratorAssembly = typeof(RestFetchSourceGenerator).Assembly;
	
	private static readonly string RestFetchNamespaceName = nameof(RestFetch);

	private static readonly string[] EmbedFiles = [
		"RestFetch.AfterResponseAttribute.cs",
		"RestFetch.BaseAddressAttribute.cs",
		"RestFetch.BeforeSendAttribute.cs",
		"RestFetch.BodyAttribute.cs",
		"RestFetch.ErrorFactoryAttribute.cs",
		"RestFetch.HeaderAttribute.cs",
		"RestFetch.HttpClientProxyAttribute.cs",
		"RestFetch.HttpMethodAttribute.cs",
		"RestFetch.IdGeneratorAttribute.cs",
		"RestFetch.MetaDataAttribute.cs",
		"RestFetch.OnErrorAttribute.cs",
		"RestFetch.OnSuccessAttribute.cs",
		"RestFetch.ParamAttribute.cs",
		"RestFetch.QueryAttribute.cs",
		"RestFetch.ResponseAttribute.cs",
	];

	private static readonly DiagnosticDescriptor Rule0 = new("TAGGED0001",
		"Multiple attributes are not allowed",
		"Multiple [Tagged] attributes are not allowed",
		"SourceGenerator", DiagnosticSeverity.Error, true,
		"Simultaneous use of [Tagged] and [Tagged<T>] attributes is not allowed.");


	public void Initialize(IncrementalGeneratorInitializationContext context) {
		context.RegisterPostInitializationOutput(static i => {
			i.AddEmbeddedAttributeDefinition();
			foreach (var file in EmbedFiles) {
				i.AddSource(file, ReadFileFromAssembly(RestFetchSourceGeneratorAssembly, file));
			}
		});

		string[] typeLevelAttrName = [
			$"{RestFetchNamespaceName}.AfterResponseAttribute`1",
			$"{RestFetchNamespaceName}.BaseAddressAttribute",
			$"{RestFetchNamespaceName}.BeforeSendAttribute`1",
			$"{RestFetchNamespaceName}.ErrorFactoryAttribute`1",
			$"{RestFetchNamespaceName}.HeaderAttribute",
			$"{RestFetchNamespaceName}.HttpClientProxyAttribute`1",
			$"{RestFetchNamespaceName}.IdGeneratorAttribute`1",
			$"{RestFetchNamespaceName}.OnErrorAttribute`1",
			$"{RestFetchNamespaceName}.OnSuccessAttribute`1",
			$"{RestFetchNamespaceName}.ResponseAttribute`1",
		];
		string[] methodLevelAttrName = [
			$"{RestFetchNamespaceName}.AfterResponseAttribute`1",
			$"{RestFetchNamespaceName}.BaseAddressAttribute",
			$"{RestFetchNamespaceName}.BeforeSendAttribute`1",
			$"{RestFetchNamespaceName}.ErrorFactoryAttribute`1",
			$"{RestFetchNamespaceName}.HeaderAttribute",
			$"{RestFetchNamespaceName}.GetAttribute",
			$"{RestFetchNamespaceName}.PostAttribute",
			$"{RestFetchNamespaceName}.PutAttribute",
			$"{RestFetchNamespaceName}.PatchAttribute",
			$"{RestFetchNamespaceName}.DeleteAttribute",
			$"{RestFetchNamespaceName}.HeadAttribute",
			$"{RestFetchNamespaceName}.OptionsAttribute",
			$"{RestFetchNamespaceName}.ConnectAttribute",
			$"{RestFetchNamespaceName}.TraceAttribute",
			$"{RestFetchNamespaceName}.IdGeneratorAttribute`1",
			$"{RestFetchNamespaceName}.MetaDataAttribute`1",
			$"{RestFetchNamespaceName}.MetaDataAttribute",
			$"{RestFetchNamespaceName}.OnErrorAttribute`1",
			$"{RestFetchNamespaceName}.OnSuccessAttribute`1",
			$"{RestFetchNamespaceName}.ResponseAttribute`1",
		];

		string[] httpAttrNames = [
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
		foreach (var httpAttr in httpAttrNames) {

		}
		var target = context.SyntaxProvider.ForAttributeWithMetadataName();
		context.RegisterSourceOutput(target, )

		using var builder = ZString.CreateStringBuilder();
		builder.Append("test");
		builder.ToString();
		// Console.WriteLine("test");
	}
}

