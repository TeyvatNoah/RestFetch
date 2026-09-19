
namespace RestFetch;

public class ExceptionFactory: IExceptionFactory {
	public static ValueTask<Exception?> CreateException(HttpResponseMessage resp, ref readonly APIInfo<HttpResponseMessage, HttpClientProxy> context, CancellationToken token) {
		return CreateExceptionInternal(resp, token);

		static async ValueTask<Exception?> CreateExceptionInternal(HttpResponseMessage resp, CancellationToken token) {
			if (resp.IsSuccessStatusCode) {
				return null;
			} else {
				var body = await resp.Content.ReadAsStringAsync(token).ConfigureAwait(false);
				return new RestFetchApiException(resp.ReasonPhrase, resp.Version) {
					ResponseBody = body
				};
			}
		}
	}
}
