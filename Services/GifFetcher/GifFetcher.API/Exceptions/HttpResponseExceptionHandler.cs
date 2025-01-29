namespace GifFetcher.API.Exceptions;

public class HttpResponseExceptionHandler(ILogger<HttpResponseExceptionHandler> Logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not HttpBadResponseException originalException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails();
        problemDetails.Title = originalException.OriginalException.Message;

        Logger.LogError($"HttpResponseExceptionHandler.TryHandleAsync {problemDetails.Title}");

        await context.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
