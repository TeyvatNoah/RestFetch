
namespace RestFetch;

public class DefaultExceptionFactory: IExceptionFactory {
	public static ValueTask<Exception?> CreateException(HttpResponseMessage resp, ref readonly ApiContext context, CancellationToken token) {
		return CreateExceptionInternal(context.Id, resp, token);

		static async ValueTask<Exception?> CreateExceptionInternal(RequestId id, HttpResponseMessage resp, CancellationToken token) {
			if (resp.IsSuccessStatusCode) {
				return null;
			} else {
				var body = await resp.Content.ReadAsStringAsync(token).ConfigureAwait(false);
				return new RestFetchApiException(id, $"Request Id: {id}, URL: {resp.RequestMessage?.RequestUri?.ToString() ?? "<missing>"}, HTTP Status Code: {(int)resp.StatusCode}, HTTP Message: {resp.ReasonPhrase}", resp.Version) {
					ResponseBody = body
				};
			}
		}
	}
}
