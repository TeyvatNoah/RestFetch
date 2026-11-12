
namespace RestFetch;

public class DefaultExceptionFactory: IExceptionFactory {
	public static ValueTask<Exception?> CreateException(HttpResponseMessage resp, ApiContext context, CancellationToken token) {
		return CreateExceptionInternal($"{context.ClassName}.{context.MethodName}", context.Id, resp, token);

		static async ValueTask<Exception?> CreateExceptionInternal(string methodName, RequestId id, HttpResponseMessage resp, CancellationToken token) {
			if (resp.IsSuccessStatusCode) {
				return null;
			} else {
				var body = await resp.Content.ReadAsStringAsync(token).ConfigureAwait(false);
				return new RestFetchApiException(
					methodName,
					id,
					resp.Version,
					$"Method: {methodName}, Request Id: {id}, URL: {resp.RequestMessage?.RequestUri?.ToString() ?? "<missing>"}, HTTP Status Code: {(int)resp.StatusCode}, HTTP Message: {resp.ReasonPhrase}") {
					ResponseBody = body
				};
			}
		}
	}
}
