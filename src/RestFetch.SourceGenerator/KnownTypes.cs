using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

namespace RestFetch.SourceGenerator;

public sealed class KnownTypes(Compilation compilation)
{
	public static readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;
	public readonly INamedTypeSymbol? Task = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
	public readonly INamedTypeSymbol? TaskT = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
	public readonly INamedTypeSymbol? ValueTask = compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask");
	public readonly INamedTypeSymbol? ValueTaskT = compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1");
	public readonly INamedTypeSymbol? IAsyncEnumerableT = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1");
	public readonly INamedTypeSymbol? AsyncMethodBuilderAttribute = compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.AsyncMethodBuilderAttribute");
	public readonly INamedTypeSymbol? MetadataAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.MetadataAttribute");
	public readonly INamedTypeSymbol? ResponseAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseAttribute");
	public readonly INamedTypeSymbol? ResponseAttributeT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.ResponseAttribute`1");
	public readonly INamedTypeSymbol? ResponseBodyDeserializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IResponseBodyDeserializer")!;
	public readonly INamedTypeSymbol? ResponseBodyGenericDeserializer1 = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IResponseBodyDeserializer`1");
	public readonly INamedTypeSymbol? ResponseBodyGenericDeserializer2 = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IResponseBodyDeserializer`2");
	public readonly INamedTypeSymbol? ResponseAsyncEnumerableDeserializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IResponseAsyncEnumerableDeserializer")!;
	public readonly INamedTypeSymbol? ResponseAsyncEnumerableGenericDeserializer1 = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IResponseAsyncEnumerableDeserializer`1");
	public readonly INamedTypeSymbol? ResponseAsyncEnumerableGenericDeserializer2 = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IResponseAsyncEnumerableDeserializer`2");
	public readonly INamedTypeSymbol? HttpMethodAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.HttpMethodAttribute");
	public readonly INamedTypeSymbol? BaseAddress = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.BaseAddressAttribute");
	public readonly INamedTypeSymbol? HeaderAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.HeaderAttribute");
	public readonly INamedTypeSymbol? IdGeneratorAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IdGeneratorAttribute`1");
	public readonly INamedTypeSymbol? BeforeSendAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.BeforeSendAttribute`1");
	public readonly INamedTypeSymbol? BeforeSendHookInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IBeforeSendHook");
	public readonly INamedTypeSymbol? BeforeSendHookGenericInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IBeforeSendHook`1");
	public readonly INamedTypeSymbol? AfterResponseAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.AfterResponseAttribute`1");
	public readonly INamedTypeSymbol? AfterResponseHookInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IAfterResponseHook");
	public readonly INamedTypeSymbol? AfterResponseHookGenericInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IAfterResponseHook`1");
	public readonly INamedTypeSymbol? ExceptionFactoryAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.ExceptionFactoryAttribute`1");
	public readonly INamedTypeSymbol? ExceptionFactoryInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IExceptionFactory");
	public readonly INamedTypeSymbol? ExceptionFactoryGenericInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IExceptionFactory`1");
	public readonly INamedTypeSymbol? HttpClientProxyAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.HttpClientProxyAttribute`1");
	public readonly INamedTypeSymbol? HttpClientProxyGenericInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IHttpClientProxy`2");
	public readonly INamedTypeSymbol? OnErrorAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.OnErrorAttribute`1");
	public readonly INamedTypeSymbol? OnErrorInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IOnError");
	public readonly INamedTypeSymbol? OnSseEventErrorAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.OnSseEventErrorAttribute`1");
	public readonly INamedTypeSymbol? OnSseEventErrorInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IOnSseEventError");
	public readonly INamedTypeSymbol? OnSuccessAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.OnSuccessAttribute`1");
	public readonly INamedTypeSymbol? OnSuccessInterface = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IOnSuccess");
	public readonly INamedTypeSymbol? CancellationToken = compilation.GetTypeByMetadataName("System.Threading.CancellationToken");
	public readonly INamedTypeSymbol? RequestBodySerializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IRequestBodySerializer");
	public readonly INamedTypeSymbol? RequestBodySerializer5 = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IRequestBodySerializer`1");
	public readonly INamedTypeSymbol? RequestBodySerializer6 = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IRequestBodySerializer`2");
	public readonly INamedTypeSymbol? ParamSerializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IParamSerializer`2");
	public readonly INamedTypeSymbol? ParamEncodeSerializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IParamEncodeSerializer`2");
	public readonly INamedTypeSymbol? QueryString = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.QueryString");
	public readonly INamedTypeSymbol? QueryStringT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.QueryString`1");
	public readonly INamedTypeSymbol? QueryStringSerializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.IQueryStringSerializer");
	public readonly INamedTypeSymbol? QueryStringSerializerT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.IQueryStringSerializer`1");
	public readonly INamedTypeSymbol? AliasQueryStringSerializer = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.IAliasQueryStringSerializer`1");
	public readonly INamedTypeSymbol? IEnumerableT = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");
	public readonly INamedTypeSymbol? KeyValuePair = compilation.GetTypeByMetadataName("System.Collections.Generic.KeyValuePair`2");
	public readonly INamedTypeSymbol? SystemString = compilation.GetSpecialType(SpecialType.System_String);
	public readonly INamedTypeSymbol? BodyAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.BodyAttribute");
	public readonly INamedTypeSymbol? BodyAttributeT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.BodyAttribute`1");
	public readonly INamedTypeSymbol? RetryAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.RetryAttribute");
	public readonly INamedTypeSymbol? TimeoutAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.TimeoutAttribute");
	public readonly INamedTypeSymbol? ParamAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.ParamAttribute");
	public readonly INamedTypeSymbol? ParamAttributeT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.ParamAttribute`1");
	public readonly INamedTypeSymbol? QueryAttribute = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.QueryAttribute");
	public readonly INamedTypeSymbol? QueryAttributeT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.QueryAttribute`1");
	public readonly INamedTypeSymbol? JsonSerializerContext = compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonSerializerContext");
	public readonly INamedTypeSymbol? Stream = compilation.GetTypeByMetadataName("System.IO.Stream");
	public readonly INamedTypeSymbol? StreamFactoryT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IStreamFactory");
	public readonly INamedTypeSymbol? IObservable = compilation.GetTypeByMetadataName("System.IObservable`1");
	public readonly INamedTypeSymbol? HttpContent = compilation.GetTypeByMetadataName("System.Net.Http.HttpContent");
	public readonly IArrayTypeSymbol? ByteArray = compilation.CreateArrayTypeSymbol(compilation.GetTypeByMetadataName("System.Byte")!);
	public readonly INamedTypeSymbol? ByteMemory = compilation.GetTypeByMetadataName("System.ReadOnlyMemory`1")?.Construct(compilation.GetSpecialType(SpecialType.System_Byte));
	public readonly INamedTypeSymbol? FileInfo = compilation.GetTypeByMetadataName("System.IO.FileInfo");
	public readonly INamedTypeSymbol? RestFetchFileInfo = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.RestFetchFileInfo");
	public readonly IArrayTypeSymbol? FileInfoArray = compilation.CreateArrayTypeSymbol(compilation.GetTypeByMetadataName("System.IO.FileInfo")!);
	public readonly IArrayTypeSymbol? RestFetchFileInfoArray = compilation.CreateArrayTypeSymbol(compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.RestFetchFileInfo")!);
	public readonly INamedTypeSymbol? IEnumerableFileInfo = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")?.Construct(compilation.GetTypeByMetadataName("System.IO.FileInfo")!);
	public readonly INamedTypeSymbol? IEnumerableRestFetchFileInfo = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")?
		.Construct(compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.RestFetchFileInfo")!);
	public readonly INamedTypeSymbol? IEnumerableKeyValuePairString = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")?
		.Construct(compilation.GetTypeByMetadataName("System.Collections.Generic.KeyValuePair`2")!
		.Construct(compilation.GetSpecialType(SpecialType.System_String), compilation.GetSpecialType(SpecialType.System_String)));
	public readonly INamedTypeSymbol? JsonNode = compilation.GetTypeByMetadataName("System.Text.Json.Nodes.JsonNode");
	public readonly INamedTypeSymbol? JsonDocument = compilation.GetTypeByMetadataName("System.Text.Json.JsonDocument");
	public readonly INamedTypeSymbol? SseEvent = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.SseEvent");
	public readonly INamedTypeSymbol? SseEventT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.WebUtilities.SseEvent`1");
	public readonly INamedTypeSymbol? SseEventResultT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.SseEventResult`1");
	public readonly INamedTypeSymbol? SseItemT = compilation.GetTypeByMetadataName("System.Net.ServerSentEvents.SseItem`1");
	public readonly INamedTypeSymbol? ApiResponseT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.ApiResponse`1");
	public readonly INamedTypeSymbol? IRestFetchApiT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IRestFetchApi`2");
	public readonly INamedTypeSymbol? IRestFetchApi = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IRestFetchApi`1");
	public readonly INamedTypeSymbol? JsonBodyT = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.JsonBody`1");
	public readonly INamedTypeSymbol? RetryPredicate = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IRetryPredicate`1");
	public readonly INamedTypeSymbol? OnRetry = compilation.GetTypeByMetadataName($"{RestFetchSourceGenerator.RestFetchNamespaceName}.IOnRetry`1");

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
		ResponseAttributeT,
		ResponseBodyDeserializer,
		ResponseBodyGenericDeserializer1,
		ResponseBodyGenericDeserializer2,
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
		HttpClientProxyGenericInterface,
		OnErrorAttribute,
		OnErrorInterface,
		OnSseEventErrorAttribute,
		OnSseEventErrorInterface,
		OnSuccessAttribute,
		OnSuccessInterface,
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
		RetryAttribute,
		TimeoutAttribute,
		ParamAttribute,
		ParamAttributeT,
		QueryAttribute,
		QueryAttributeT,
		JsonSerializerContext,
		Stream,
		IObservable,
		HttpContent,
		ByteArray,
		ByteMemory,
		FileInfo,
		RestFetchFileInfo,
		IEnumerableFileInfo,
		IEnumerableRestFetchFileInfo,
		IEnumerableKeyValuePairString,
		JsonNode,
		JsonDocument,
		SseEvent,
		SseEventT,
		SseEventResultT,
		SseItemT,
		ApiResponseT,
		IRestFetchApi,
		IRestFetchApiT,
		StreamFactoryT,
		JsonBodyT,
		RetryPredicate,
		OnRetry
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
		&& Comparer.Equals(left.ResponseAttributeT, right.ResponseAttributeT)
		&& Comparer.Equals(left.ResponseBodyDeserializer, right.ResponseBodyDeserializer)
		&& Comparer.Equals(left.ResponseBodyGenericDeserializer1, right.ResponseBodyGenericDeserializer1)
		&& Comparer.Equals(left.ResponseBodyGenericDeserializer2, right.ResponseBodyGenericDeserializer2)
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
		&& Comparer.Equals(left.HttpClientProxyGenericInterface, right.HttpClientProxyGenericInterface)
		&& Comparer.Equals(left.OnErrorAttribute, right.OnErrorAttribute)
		&& Comparer.Equals(left.OnErrorInterface, right.OnErrorInterface)
		&& Comparer.Equals(left.OnSseEventErrorAttribute, right.OnSseEventErrorAttribute)
		&& Comparer.Equals(left.OnSseEventErrorInterface, right.OnSseEventErrorInterface)
		&& Comparer.Equals(left.OnSuccessAttribute, right.OnSuccessAttribute)
		&& Comparer.Equals(left.OnSuccessInterface, right.OnSuccessInterface)
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
		&& Comparer.Equals(left.RetryAttribute, right.RetryAttribute)
		&& Comparer.Equals(left.TimeoutAttribute, right.TimeoutAttribute)
		&& Comparer.Equals(left.ParamAttribute, right.ParamAttribute)
		&& Comparer.Equals(left.ParamAttributeT, right.ParamAttributeT)
		&& Comparer.Equals(left.QueryAttribute, right.QueryAttribute)
		&& Comparer.Equals(left.QueryAttributeT, right.QueryAttributeT)
		&& Comparer.Equals(left.JsonSerializerContext, right.JsonSerializerContext)
		&& Comparer.Equals(left.Stream, right.Stream)
		&& Comparer.Equals(left.IObservable, right.IObservable)
		&& Comparer.Equals(left.HttpContent, right.HttpContent)
		&& Comparer.Equals(left.ByteArray, right.ByteArray)
		&& Comparer.Equals(left.ByteMemory, right.ByteMemory)
		&& Comparer.Equals(left.FileInfo, right.FileInfo)
		&& Comparer.Equals(left.RestFetchFileInfo, right.RestFetchFileInfo)
		&& Comparer.Equals(left.IEnumerableFileInfo, right.IEnumerableFileInfo)
		&& Comparer.Equals(left.IEnumerableRestFetchFileInfo, right.IEnumerableRestFetchFileInfo)
		&& Comparer.Equals(left.IEnumerableKeyValuePairString, right.IEnumerableKeyValuePairString)
		&& Comparer.Equals(left.JsonNode, right.JsonNode)
		&& Comparer.Equals(left.JsonDocument, right.JsonDocument)
		&& Comparer.Equals(left.SseEvent, right.SseEvent)
		&& Comparer.Equals(left.SseEventT, right.SseEventT)
		&& Comparer.Equals(left.SseEventResultT, right.SseEventResultT)
		&& Comparer.Equals(left.SseItemT, right.SseItemT)
		&& Comparer.Equals(left.IRestFetchApi, right.IRestFetchApi)
		&& Comparer.Equals(left.IRestFetchApiT, right.IRestFetchApiT)
		&& Comparer.Equals(left.ApiResponseT, right.ApiResponseT)
		&& Comparer.Equals(left.StreamFactoryT, right.StreamFactoryT)
		&& Comparer.Equals(left.JsonBodyT, right.JsonBodyT)
		&& Comparer.Equals(left.RetryPredicate, right.RetryPredicate)
		&& Comparer.Equals(left.OnRetry, right.OnRetry);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(KnownTypes left, KnownTypes right) => !(left == right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static INamedTypeSymbol? CreateIEnumerableT(KnownTypes knownTypes, ITypeSymbol tSymbol) => knownTypes.IEnumerableT?.Construct(tSymbol);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static INamedTypeSymbol? CreateKeyValuePair(KnownTypes knownTypes, ITypeSymbol tSymbol0, ITypeSymbol tSymbol1) => knownTypes.KeyValuePair?.Construct(tSymbol0, tSymbol1);
}
