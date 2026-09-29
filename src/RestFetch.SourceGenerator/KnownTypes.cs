using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public sealed class KnownTypes {
	public static readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;
	public readonly INamedTypeSymbol? Task;
	public readonly INamedTypeSymbol? TaskT;
	public readonly INamedTypeSymbol? ValueTask;
	public readonly INamedTypeSymbol? ValueTaskT;
	public readonly INamedTypeSymbol? IAsyncEnumerableT;
	public readonly INamedTypeSymbol? AsyncMethodBuilderAttribute;
	public readonly INamedTypeSymbol? MetadataAttribute;
	public readonly INamedTypeSymbol? ResponseAttribute;
	public readonly INamedTypeSymbol? ResponseBodyDeserializer;
	public readonly INamedTypeSymbol? ResponseBodyGenericDeserializer;
	public readonly INamedTypeSymbol? HttpMethodAttribute;
	public readonly INamedTypeSymbol? BaseAddress;
	public readonly INamedTypeSymbol? HeaderAttribute;
	public readonly INamedTypeSymbol? IdGeneratorAttribute;
	public readonly INamedTypeSymbol? BeforeSendAttribute;
	public readonly INamedTypeSymbol? BeforeSendHookInterface;
	public readonly INamedTypeSymbol? BeforeSendHookGenericInterface;
	public readonly INamedTypeSymbol? AfterResponseAttribute;
	public readonly INamedTypeSymbol? AfterResponseHookInterface;
	public readonly INamedTypeSymbol? AfterResponseHookGenericInterface;
	public readonly INamedTypeSymbol? ExceptionFactoryAttribute;
	public readonly INamedTypeSymbol? ExceptionFactoryInterface;
	public readonly INamedTypeSymbol? ExceptionFactoryGenericInterface;
	public readonly INamedTypeSymbol? HttpClientProxyAttribute;
	public readonly INamedTypeSymbol? HttpClientProxyInterface;
	public readonly INamedTypeSymbol? HttpClientProxyGenericInterface;
	public readonly INamedTypeSymbol? OnErrorAttribute;
	public readonly INamedTypeSymbol? OnErrorInterface;
	public readonly INamedTypeSymbol? OnErrorGenericInterface;
	public readonly INamedTypeSymbol? OnSuccessAttribute;
	public readonly INamedTypeSymbol? OnSuccessInterface;
	public readonly INamedTypeSymbol? OnSuccessGenericInterface;
	public readonly INamedTypeSymbol? CancellationToken;
	public readonly INamedTypeSymbol? RequestBodySerializer;
	public readonly INamedTypeSymbol? RequestBodySerializer5;
	public readonly INamedTypeSymbol? RequestBodySerializer6;
	public readonly INamedTypeSymbol? ParamSerializer;
	public readonly INamedTypeSymbol? ParamEncodeSerializer;
	public readonly INamedTypeSymbol? QueryString;
	public readonly INamedTypeSymbol? QueryStringT;
	public readonly INamedTypeSymbol? QueryStringSerializer;
	public readonly INamedTypeSymbol? QueryStringSerializerT;
	public readonly INamedTypeSymbol? AliasQueryStringSerializer;
	public readonly INamedTypeSymbol? IEnumerableT;
	public readonly INamedTypeSymbol? KeyValuePair;
	public readonly INamedTypeSymbol? SystemString;
	public readonly INamedTypeSymbol? BodyAttribute;
	public readonly INamedTypeSymbol? BodyAttributeT;
	public readonly INamedTypeSymbol? ParamAttribute;
	public readonly INamedTypeSymbol? ParamAttributeT;
	public readonly INamedTypeSymbol? QueryAttribute;
	public readonly INamedTypeSymbol? QueryAttributeT;
	public KnownTypes(Compilation compilation) {
		Task = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
		TaskT = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
		ValueTask = compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask");
		ValueTaskT = compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1");
		IAsyncEnumerableT = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1");
		AsyncMethodBuilderAttribute = compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.AsyncMethodBuilderAttribute");
		MetadataAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.MetadataAttribute");
		ResponseAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseAttribute`1");
		ResponseBodyDeserializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IResponseBodyDeserializer")!;
		ResponseBodyGenericDeserializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.,IResponseBodyDeserializer`4");
		HttpMethodAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.HttpMethodAttribute");
		BaseAddress = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.BaseAddressAttribute");
		HeaderAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.HeaderAttribute");
		IdGeneratorAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IdGeneratorAttribute`1");
		BeforeSendAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.BeforeSendAttribute`1");
		BeforeSendHookInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IBeforeSendHook");
		BeforeSendHookGenericInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IBeforeSendHook`5");
		AfterResponseAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.AfterResponseAttribute`1");
		AfterResponseHookInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IAfterResponseHook");
		AfterResponseHookGenericInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IAfterResponseHook`4");
		ExceptionFactoryAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.ExceptionFactoryAttribute`1");
		ExceptionFactoryInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IExceptionFactory");
		ExceptionFactoryGenericInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IExceptionFactory`4");
		HttpClientProxyAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.HttpClientProxyAttribute`1");
		HttpClientProxyInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxy");
		HttpClientProxyGenericInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxy`4");
		OnErrorAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.OnErrorAttribute`1");
		OnErrorInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IOnError");
		OnErrorGenericInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IOnError`4");
		OnSuccessAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.OnSuccessAttribute`1");
		OnSuccessInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IOnSuccess");
		OnSuccessGenericInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IOnSuccess`4");
		CancellationToken = compilation.GetTypeByMetadataName("System.Threading.CancellationToken");
		RequestBodySerializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IRequestBodySerializer");
		RequestBodySerializer5 = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IRequestBodySerializer`5");
		RequestBodySerializer6 = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IRequestBodySerializer`6");
		ParamSerializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IParamSerializer`2");
		ParamEncodeSerializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IParamEncodeSerializer`2");
		QueryString = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.QueryString");
		QueryStringT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.QueryString`1");
		QueryStringSerializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.IQueryStringSerializer");
		QueryStringSerializerT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.IQueryStringSerializer`1");
		AliasQueryStringSerializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.IAliasQueryStringSerializer`1");
		IEnumerableT = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");
		KeyValuePair = compilation.GetTypeByMetadataName("System.Collections.Generic.KeyValuePair`2");
		SystemString = compilation.GetSpecialType(SpecialType.System_String);
		BodyAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.BodyAttribute");
		BodyAttributeT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.BodyAttribute`1");
		ParamAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.ParamAttribute");
		ParamAttributeT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.ParamAttribute`1");
		QueryAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.QueryAttribute");
		QueryAttributeT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.QueryAttribute`1");
	}

	public override bool Equals(object? obj) => obj is not null && obj is KnownTypes types && Equals(types);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(KnownTypes obj) => this == obj;

	public override int GetHashCode() => (
		Task,
		TaskT,
		ValueTask,
		ValueTaskT,
		IAsyncEnumerableT,
		AsyncMethodBuilderAttribute,
		MetadataAttribute,
		ResponseAttribute,
		ResponseBodyDeserializer,
		ResponseBodyGenericDeserializer,
		HttpMethodAttribute,
		BaseAddress,
		HeaderAttribute,
		IdGeneratorAttribute,
		BeforeSendAttribute,
		BeforeSendHookInterface,
		BeforeSendHookGenericInterface,
		AfterResponseAttribute,
		AfterResponseHookInterface,
		AfterResponseHookGenericInterface,
		ExceptionFactoryAttribute,
		ExceptionFactoryInterface,
		ExceptionFactoryGenericInterface,
		HttpClientProxyAttribute,
		HttpClientProxyInterface,
		HttpClientProxyGenericInterface,
		OnErrorAttribute,
		OnErrorInterface,
		OnErrorGenericInterface,
		OnSuccessAttribute,
		OnSuccessInterface,
		OnSuccessGenericInterface,
		CancellationToken,
		RequestBodySerializer,
		RequestBodySerializer5,
		RequestBodySerializer6,
		ParamSerializer,
		ParamEncodeSerializer,
		QueryString,
		QueryStringT,
		QueryStringSerializer,
		QueryStringSerializerT,
		AliasQueryStringSerializer,
		IEnumerableT,
		KeyValuePair,
		SystemString,
		BodyAttribute,
		BodyAttributeT,
		ParamAttribute,
		ParamAttributeT,
		QueryAttribute,
		QueryAttributeT
	).GetHashCode();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(KnownTypes left, KnownTypes right) =>
		Comparer.Equals(left.Task, right.Task)
		&& Comparer.Equals(left.TaskT, right.TaskT)
		&& Comparer.Equals(left.ValueTask, right.ValueTask)
		&& Comparer.Equals(left.ValueTaskT, right.ValueTaskT)
		&& Comparer.Equals(left.IAsyncEnumerableT, right.IAsyncEnumerableT)
		&& Comparer.Equals(left.AsyncMethodBuilderAttribute, right.AsyncMethodBuilderAttribute)
		&& Comparer.Equals(left.MetadataAttribute, right.MetadataAttribute)
		&& Comparer.Equals(left.ResponseAttribute, right.ResponseAttribute)
		&& Comparer.Equals(left.ResponseBodyDeserializer, right.ResponseBodyDeserializer)
		&& Comparer.Equals(left.ResponseBodyGenericDeserializer, right.ResponseBodyGenericDeserializer)
		&& Comparer.Equals(left.HttpMethodAttribute, right.HttpMethodAttribute)
		&& Comparer.Equals(left.BaseAddress, right.BaseAddress)
		&& Comparer.Equals(left.HeaderAttribute, right.HeaderAttribute)
		&& Comparer.Equals(left.IdGeneratorAttribute, right.IdGeneratorAttribute)
		&& Comparer.Equals(left.BeforeSendAttribute, right.BeforeSendAttribute)
		&& Comparer.Equals(left.BeforeSendHookInterface, right.BeforeSendHookInterface)
		&& Comparer.Equals(left.BeforeSendHookGenericInterface, right.BeforeSendHookGenericInterface)
		&& Comparer.Equals(left.AfterResponseAttribute, right.AfterResponseAttribute)
		&& Comparer.Equals(left.AfterResponseHookInterface, right.AfterResponseHookInterface)
		&& Comparer.Equals(left.AfterResponseHookGenericInterface, right.AfterResponseHookGenericInterface)
		&& Comparer.Equals(left.ExceptionFactoryAttribute, right.ExceptionFactoryAttribute)
		&& Comparer.Equals(left.ExceptionFactoryInterface, right.ExceptionFactoryInterface)
		&& Comparer.Equals(left.ExceptionFactoryGenericInterface, right.ExceptionFactoryGenericInterface)
		&& Comparer.Equals(left.HttpClientProxyAttribute, right.HttpClientProxyAttribute)
		&& Comparer.Equals(left.HttpClientProxyInterface, right.HttpClientProxyInterface)
		&& Comparer.Equals(left.HttpClientProxyGenericInterface, right.HttpClientProxyGenericInterface)
		&& Comparer.Equals(left.OnErrorAttribute, right.OnErrorAttribute)
		&& Comparer.Equals(left.OnErrorInterface, right.OnErrorInterface)
		&& Comparer.Equals(left.OnErrorGenericInterface, right.OnErrorGenericInterface)
		&& Comparer.Equals(left.OnSuccessAttribute, right.OnSuccessAttribute)
		&& Comparer.Equals(left.OnSuccessInterface, right.OnSuccessInterface)
		&& Comparer.Equals(left.OnSuccessGenericInterface, right.OnSuccessGenericInterface)
		&& Comparer.Equals(left.CancellationToken, right.CancellationToken)
		&& Comparer.Equals(left.RequestBodySerializer, right.RequestBodySerializer)
		&& Comparer.Equals(left.RequestBodySerializer5, right.RequestBodySerializer5)
		&& Comparer.Equals(left.RequestBodySerializer6, right.RequestBodySerializer6)
		&& Comparer.Equals(left.ParamSerializer, right.ParamSerializer)
		&& Comparer.Equals(left.ParamEncodeSerializer, right.ParamEncodeSerializer)
		&& Comparer.Equals(left.QueryString, right.QueryString)
		&& Comparer.Equals(left.QueryStringT, right.QueryStringT)
		&& Comparer.Equals(left.QueryStringSerializer, right.QueryStringSerializer)
		&& Comparer.Equals(left.QueryStringSerializerT, right.QueryStringSerializerT)
		&& Comparer.Equals(left.AliasQueryStringSerializer, right.AliasQueryStringSerializer)
		&& Comparer.Equals(left.IEnumerableT, right.IEnumerableT)
		&& Comparer.Equals(left.KeyValuePair, right.KeyValuePair)
		&& Comparer.Equals(left.SystemString, right.SystemString)
		&& Comparer.Equals(left.BodyAttribute, right.BodyAttribute)
		&& Comparer.Equals(left.BodyAttributeT, right.BodyAttributeT)
		&& Comparer.Equals(left.ParamAttribute, right.ParamAttribute)
		&& Comparer.Equals(left.ParamAttributeT, right.ParamAttributeT)
		&& Comparer.Equals(left.QueryAttribute, right.QueryAttribute)
		&& Comparer.Equals(left.QueryAttributeT, right.QueryAttributeT);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(KnownTypes left, KnownTypes right) => !(left == right);
}
