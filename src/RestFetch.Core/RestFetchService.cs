using System.Runtime.CompilerServices;

namespace RestFetch;

public static class RestFetchService<THttpClient, TResponse>
	where THttpClient: IDisposable
	where TResponse: IDisposable {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TApi For<TApi>() where TApi: class, IRestFetchApi<TApi, TResponse>
		=> TApi.Create();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TApi For<TApi>(string baseAddress) where TApi: class, IRestFetchApi<TApi, TResponse>
		=> TApi.Create(baseAddress);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TApi For<TApi>(IHttpClientProxyService<TResponse> httpClientProxy) where TApi: class, IRestFetchApi<TApi, TResponse>
		=> TApi.Create(httpClientProxy);
}


public static class RestFetchService {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TApi For<TApi>() where TApi: class, IRestFetchApi<TApi>
		=> TApi.Create();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TApi For<TApi>(string baseAddress) where TApi: class, IRestFetchApi<TApi>
		=> TApi.Create(baseAddress);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TApi For<TApi>(IHttpClientProxyService<HttpResponseMessage> httpClientProxy) where TApi: class, IRestFetchApi<TApi>
		=> TApi.Create(httpClientProxy);
}