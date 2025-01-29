namespace GifFetcher.API.Fetchers.Interfaces;

public interface IFetcher
{
    ValueTask<GifFetchResult> FetchByTrendAsync(int offset, CancellationToken cancellationToken);
    ValueTask<GifFetchResult> FetchByTermAsync(string term, int offset, CancellationToken cancellationToken);
}
