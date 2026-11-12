using System.Runtime.CompilerServices;

namespace RestFetch;

public class DefaultExceptionFactory: IExceptionFactory {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static async ValueTask<Exception?> CreateException(HttpResponseMessage resp, ApiContext context, CancellationToken token) {
		if (resp.IsSuccessStatusCode) {
			return null;
		} else {
			var body = await resp.Content.ReadAsStringAsync(token).ConfigureAwait(false);
			return new RestFetchApiException(
				context.MethodName,
				context.Id,
				resp.Version,
				$"Method: {context.MethodName}, Request Id: {context.Id}, URL: {resp.RequestMessage?.RequestUri?.ToString() ?? "<missing>"}, HTTP Status Code: {(int)resp.StatusCode}, HTTP Message: {resp.ReasonPhrase}") {
				ResponseBody = body
			};
		}
	}
}
