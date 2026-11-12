using System.Runtime.CompilerServices;

namespace RestFetch;

public static class RestFetchService<THttpClient, TResponse>
	where THttpClient: IDisposable
	where TResponse: IDisposable {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>() where TAPI: IRestFetchApi<TAPI, THttpClient, TResponse>
		=> TAPI.Create();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(string baseAddress) where TAPI: IRestFetchApi<TAPI, THttpClient, TResponse>
		=> TAPI.Create(baseAddress);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TAPI For<TAPI>(IHttpClientProxy<THttpClient, TResponse> httpClientProxy) where TAPI: IRestFetchApi<TAPI, THttpClient, TResponse>
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
	public static TAPI For<TAPI>(IHttpClientProxy<HttpClient, HttpResponseMessage> httpClientProxy) where TAPI: IRestFetchApi<TAPI>
		=> TAPI.Create(httpClientProxy);
}