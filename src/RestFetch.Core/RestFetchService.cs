using System.Runtime.CompilerServices;

namespace RestFetch;

public static class RestFetchService<TResponse, THttpClientProxy>
	where TResponse: IDisposable
	where THttpClientProxy: IHttpClientProxy<TResponse, THttpClientProxy> {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>() where TAPI: IRestFetchApi<TAPI, TResponse, THttpClientProxy>
		=> TAPI.Create();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(string baseAddress) where TAPI: IRestFetchApi<TAPI, TResponse, THttpClientProxy>
		=> TAPI.Create(baseAddress);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(THttpClientProxy httpClientProxy) where TAPI: IRestFetchApi<TAPI, TResponse, THttpClientProxy>
		=> TAPI.Create(httpClientProxy);
}


public static class RestFetchService {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>() where TAPI: IRestFetchApi<TAPI>
		=> TAPI.Create();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(string baseAddress) where TAPI: IRestFetchApi<TAPI>
		=> TAPI.Create(baseAddress);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(HttpClientProxy httpClientProxy) where TAPI: IRestFetchApi<TAPI>
		=> TAPI.Create(httpClientProxy);
}