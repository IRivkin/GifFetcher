namespace GifFetcher.API.Fetchers;

public class Fetcher(IProvider Provider, ILogger<Fetcher> Logger) : IFetcher
{
    public async ValueTask<GifFetchResult> FetchByTrendAsync(int offset, CancellationToken cancellationToken)
    {
        try
        {
            var result = await Provider.FetchByTrendAsync(offset:offset, cancellationToken:cancellationToken); 

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Fetcher.FetchByTrendAsync. An exception was caught while fetching by trend");
            throw;
        }
    }

    public async ValueTask<GifFetchResult> FetchByTermAsync(string term, int offset, CancellationToken cancellationToken)
    {
        try
        {
            var result = await Provider.FetchByTermAsync(term:term, offset:offset, cancellationToken:cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Fetcher.FetchByTrendAsync. An exception was caught while fetching by term");
            throw;
        }
    }
}
