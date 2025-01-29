namespace GifFetcher.API.Exceptions;

/// <summary>
/// IExceptionHandler is an interface for handling exceptions in ASP.NET Core applications.
/// It defines an interface that we can implement to handle different exceptions. This allows us to write custom logic for handling individual exceptions or groups of exceptions based on their type, in turn providing tailored responses, error messages as well as logging.
/// In this particular case, the interface is used only to format exception information.
/// </summary>
public class GlobalExceptionHandler(
    IHostEnvironment Env, IFeatureManager FeatureManager,
    ILogger<GlobalExceptionHandler> Logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is HttpBadResponseException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails();
        problemDetails.Instance = context.Request.Path;
        problemDetails.Title = exception.Message;
        problemDetails.Status = context.Response.StatusCode;

        if (Env.IsDevelopment() && !await FeatureManager.IsEnabledAsync("DisableExceptionDetails"))
        {
            // Show problem details in development environment only
            problemDetails.Detail = exception.ToString();
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;
            problemDetails.Extensions["data"] = exception.Data;
        }

        Logger.LogError($"GlobalExceptionHandler.TryHandleAsync {problemDetails.Title}, {problemDetails.Status}, {problemDetails.Instance}");

        await context.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
