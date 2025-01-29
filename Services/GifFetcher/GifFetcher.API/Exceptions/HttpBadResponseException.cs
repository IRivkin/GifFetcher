namespace GifFetcher.API.Exceptions;

public class HttpBadResponseException : Exception
{
    public Exception OriginalException { get; }

    public HttpBadResponseException(Exception originalException)
    {
        OriginalException = originalException; 
    }
}
