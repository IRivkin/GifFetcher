namespace GifFetcher.API.Providers.Interfaces;

public interface IProvider
{
    ValueTask<GifFetchResult> FetchByTrendAsync(int offset, CancellationToken cancellationToken);
    ValueTask<GifFetchResult> FetchByTermAsync(string term, int offset, CancellationToken cancellationToken);
}
