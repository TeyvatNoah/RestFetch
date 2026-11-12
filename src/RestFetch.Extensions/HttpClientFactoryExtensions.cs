using System.Runtime.CompilerServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RestFetch.Extensions;

public static class HttpClientFactoryExtensions {
	private static long _httpClientProxyCounter;

	private static string CreateHttpClientProxyKey() => $"RestFetch.HttpClientProxy.{Interlocked.Increment(ref _httpClientProxyCounter)}";

	private static long _httpClientNameCounter;

	private static string CreateHttpClientKey() => $"RestFetch.HttpClient.{Interlocked.Increment(ref _httpClientNameCounter)}";

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static HttpClientProxy CreateHttpClientProxy(
		IServiceProvider provider,
		string httpClientName,
		string? baseAddress = default,
		HttpClientProxyOptions.HttpClientProxyCommonOptions? options = default) {

		var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

		return new(
			new HttpClientProxyOptions.HttpClientProxyFactoryOptions(() => httpClientFactory.CreateClient(httpClientName)) {
				SerializerOptions = options?.SerializerOptions,
				DeserializerOptions = options?.DeserializerOptions
			}) {
			BaseAddress = baseAddress
		};
	}

	extension(IServiceCollection services) {
		public IHttpClientBuilder AddRestFetchApi<TApiImpl>(string? baseAddress = default, string? httpClientName = default, Action<TApiImpl, IHttpClientProxyService<HttpResponseMessage>>? configureFn = default)
			where TApiImpl: class, IRestFetchApi<TApiImpl, HttpResponseMessage> {

			var proxyKey = CreateHttpClientProxyKey();

			services.AddSingleton(provider => {
				var clientProxy = provider.GetRequiredKeyedService<IHttpClientProxyService<HttpResponseMessage>>(proxyKey);
				var instance = TApiImpl.Create(true, clientProxy);
				configureFn?.Invoke(instance, clientProxy);
				return instance.Build();
			});

			return services.AddHttpClientProxyService(proxyKey, baseAddress, httpClientName);
		}

		public IServiceCollection AddRestFetchApi<TApiImpl, TProxyKey>(TProxyKey proxyKey, Action<TApiImpl>? configureFn = default)
			where TProxyKey: notnull
			where TApiImpl: class, IRestFetchApi<TApiImpl, HttpResponseMessage> {

			services.AddSingleton(provider => {
				var clientProxy = provider.GetRequiredKeyedService<IHttpClientProxyService<HttpResponseMessage>>(proxyKey);
				var instance = TApiImpl.Create(true, clientProxy);
				configureFn?.Invoke(instance);
				return instance.Build();
			});

			return services;
		}

		public IHttpClientBuilder AddRestFetchApi<TApi, TApiImpl>(string? baseAddress = default, string? httpClientName = default, Action<TApiImpl, IHttpClientProxyService<HttpResponseMessage>>? configureFn = default)
			where TApi: class
			where TApiImpl: class, TApi, IRestFetchApi<TApiImpl, HttpResponseMessage> {

			var proxyKey = CreateHttpClientProxyKey();

			services.AddSingleton<TApi, TApiImpl>(provider => {
				var clientProxy = provider.GetRequiredKeyedService<IHttpClientProxyService<HttpResponseMessage>>(proxyKey);
				var instance = TApiImpl.Create(true, clientProxy);
				configureFn?.Invoke(instance, clientProxy);
				return instance.Build();
			});

			return services.AddHttpClientProxyService(proxyKey, baseAddress, httpClientName);
		}

		public IServiceCollection AddRestFetchApi<TApi, TApiImpl, TProxyKey>(TProxyKey proxyKey, Action<TApiImpl>? configureFn = default)
			where TProxyKey: notnull
			where TApi: class
			where TApiImpl: class, TApi, IRestFetchApi<TApiImpl, HttpResponseMessage> {


			services.AddSingleton<TApi, TApiImpl>(provider => {
				var clientProxy = provider.GetRequiredKeyedService<IHttpClientProxyService<HttpResponseMessage>>(proxyKey);
				var instance = TApiImpl.Create(true, clientProxy);
				configureFn?.Invoke(instance);
				return instance.Build();
			});

			return services;
		}

		public IServiceCollection AddKeyedRestFetchApi<TKey, TProxyKey, TApiImpl>(TKey key, TProxyKey proxyKey, Action<TApiImpl>? configureFn = default)
			where TProxyKey: notnull
			where TKey: notnull
			where TApiImpl: class, IRestFetchApi<TApiImpl, HttpResponseMessage> {

			services.AddKeyedSingleton(key, (provider, _) => {
				var clientProxy = provider.GetRequiredKeyedService<IHttpClientProxyService<HttpResponseMessage>>(proxyKey);
				var instance = TApiImpl.Create(true, clientProxy);
				configureFn?.Invoke(instance);
				return instance.Build();
			});

			return services;
		}

		public IHttpClientBuilder AddKeyedRestFetchApi<TKey, TApi, TApiImpl>(TKey key, string? baseAddress = default, string? httpClientName = default, Action<TApiImpl, IHttpClientProxyService<HttpResponseMessage>>? configureFn = default)
			where TKey: notnull
			where TApi: class
			where TApiImpl: class, TApi, IRestFetchApi<TApiImpl, HttpResponseMessage> {

			services.AddKeyedSingleton<TApi, TApiImpl>(key, (provider, _) => {
				var clientProxy = provider.GetRequiredKeyedService<IHttpClientProxyService<HttpResponseMessage>>(key);
				var instance = TApiImpl.Create(true, clientProxy);
				configureFn?.Invoke(instance, clientProxy);
				return instance.Build();
			});

			return services.AddHttpClientProxyService(key, baseAddress, httpClientName);
		}

		public IServiceCollection AddKeyedRestFetchApi<TKey, TProxyKey, TApi, TApiImpl>(TKey key, TProxyKey proxyKey, Action<TApiImpl>? configureFn = default)
			where TKey: notnull
			where TProxyKey: notnull
			where TApi: class
			where TApiImpl: class, TApi, IRestFetchApi<TApiImpl, HttpResponseMessage> {

			services.AddKeyedSingleton<TApi, TApiImpl>(key, (provider, _) => {
				var clientProxy = provider.GetRequiredKeyedService<IHttpClientProxyService<HttpResponseMessage>>(proxyKey);
				var instance = TApiImpl.Create(true, clientProxy);
				configureFn?.Invoke(instance);
				return instance.Build();
			});

			return services;
		}




		public IHttpClientBuilder TryAddDefaultHttpClientProxyService(string? httpClientName = default) {
			var clientName = httpClientName ?? "";

			services.TryAddKeyedSingleton<IHttpClientProxyService<HttpResponseMessage>>(KeyedService.AnyKey, (provider, _)
				=> CreateHttpClientProxy(provider, clientName));

			return services.AddHttpClient(clientName);
		}

		public IHttpClientBuilder AddHttpClientProxyService<TKey>(
			TKey key,
			string? baseAddress = default,
			string? httpClientName = default,
			HttpClientProxyOptions.HttpClientProxyCommonOptions? options = default)
			where TKey: notnull {

			var clientName = httpClientName ?? CreateHttpClientKey();

			services.AddKeyedSingleton<IHttpClientProxyService<HttpResponseMessage>, HttpClientProxy>(key, (provider, _)
				=> CreateHttpClientProxy(provider, clientName, baseAddress, options));

			return services.AddHttpClient(clientName);
		}

	}
}