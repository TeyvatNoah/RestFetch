using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

using Polly.Retry;
using Polly.Timeout;

namespace RestFetch;

public sealed class DefaultRetryPredicate: IRetryPredicate {
	public static Func<RetryPredicateArguments<HttpResponseMessage>, ValueTask<bool>> CreateShouldHandle<TRequestBody>(TRequestBody? body, RetryOptions<HttpResponseMessage>? retryOptions, HttpMethod httpMethod) {
		if (!(retryOptions?.AllowUnsafeHttpMethod ?? false)) {
			if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Patch || httpMethod == HttpMethod.Connect) {
				ThrowUnsafeHTTPMethodException(httpMethod);
			}
		}

		if (!(retryOptions?.AllowUnsafeRequestBody ?? false)) {
			if (body is Stream or HttpContent or HttpRequestMessage) {
				ThrowUnsafeHTTPBodyException("Request body type Stream/HttpContent/HttpRequestMessage is insecure when configured with a Retry strategy. Use IStreamFactory instead.");
			}
		}

		return static args => {
			if (args.Context.CancellationToken.IsCancellationRequested) {
				return ValueTask.FromResult(false);
			}

			var ex = args.Outcome.Exception;
			var isRetryException = ex is HttpRequestException
				or HttpIOException
				or SocketException
				or TimeoutRejectedException
				or OperationCanceledException {
					InnerException: TimeoutException
				}
				or TaskCanceledException {
					InnerException: TimeoutException
				};

			var statusCode = args.Outcome.Result?.StatusCode;
			var isRetryStatusCode = statusCode is not null && (statusCode == System.Net.HttpStatusCode.TooManyRequests
				|| statusCode == System.Net.HttpStatusCode.BadGateway
				|| statusCode == System.Net.HttpStatusCode.ServiceUnavailable
				|| statusCode == System.Net.HttpStatusCode.GatewayTimeout
				|| statusCode == System.Net.HttpStatusCode.InternalServerError
				|| statusCode == System.Net.HttpStatusCode.RequestTimeout);

			return ValueTask.FromResult(isRetryException || isRetryStatusCode);
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ValidateRequestBody<TRequestBody>(TRequestBody? body, RetryOptions<HttpResponseMessage>? retryOptions, ApiContext context) {
		if ((body is Stream bodyStream && !bodyStream.CanSeek)
			|| (!(retryOptions?.AllowUnsafeRequestBody ?? false)
			&& body is HttpContent or HttpRequestMessage)) {
			ThrowUnsafeHTTPBodyException("Request body type Stream/HttpContent/HttpRequestMessage is insecure when configured with a Retry strategy. Use IStreamFactory instead.");
		}
	}

	[DoesNotReturn]
	private static void ThrowUnsafeHTTPMethodException(HttpMethod httpMethod) {
		throw new UnsafeRetryHTTPMethodException(httpMethod, $"HTTP Method: {httpMethod} is insecure when configured with a Retry strategy.");
	}

	[DoesNotReturn]
	private static void ThrowUnsafeHTTPBodyException(string msg) => throw new UnsafeRetryHTTPRequestBodyException(msg);
}

public sealed class UnsafeRetryHTTPMethodException(HttpMethod httpMethod, string msg): Exception(msg) {
	public readonly HttpMethod HttpMethod = httpMethod;
}


public sealed class UnsafeRetryHTTPRequestBodyException(string msg): Exception(msg);

