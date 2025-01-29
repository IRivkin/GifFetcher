namespace GifFetcher.API.Fetchers;

/// <summary>
/// This class provides data caching using Redis database.
/// If there is no data in the cache, a request is made to the remote data provider,
/// and the data is then stored in the cache.
/// Sometimes calling Redis methods causes an exception. It may take some time to figure out the cause.
/// In order not to delay the transfer of the project for review, I developed a Redis interface emulator.
/// By default, the emulated interface is used.
/// To switch to real mode, the EmulateDistributedCache value in the appsettings.json must be set to false.
/// The application does not need to be stopped, the change is caught on the fly.
/// 
/// The cache key is built on the basis of offset (the index of the page in the response result list)
/// </summary>
public class FetcherCached(
    IFetcher Fetcher, IDistributedCache Cache, ICacheEmulated CacheEmulated, 
    IFeatureManager FeatureManager, ILogger<FetcherCached> Logger) : IFetcher
{
    public async ValueTask<GifFetchResult> FetchByTrendAsync(int offset, CancellationToken cancellationToken)
    {
        try
        {
            if (await FeatureManager.IsEnabledAsync("EmulateDistributedCache"))
            {
                Cache = CacheEmulated;
            }

            var cacheKey = $"TREND_{offset}";
            var cachedResult = await Cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedResult))
            {
                return JsonConvert.DeserializeObject<GifFetchResult>(cachedResult)!;
            }

            var result = await Fetcher.FetchByTrendAsync(offset, cancellationToken);
            await Cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(result), cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "FetcherCached.FetchByTrendAsync. An exception was caught while fetching by trend");
            throw;
        }
    }

    public async ValueTask<GifFetchResult> FetchByTermAsync(string term, int offset, CancellationToken cancellationToken)
    {
        try
        {
            if (await FeatureManager.IsEnabledAsync("EmulateDistributedCache"))
            {
                Cache = CacheEmulated;
            }

            var cacheKey = $"TERM_'{term}'_{offset}";
            var cachedResult = await Cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedResult))
            {
                return JsonConvert.DeserializeObject<GifFetchResult>(cachedResult)!;
            }

            var result = await Fetcher.FetchByTermAsync(term, offset, cancellationToken);
            await Cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(result), cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "FetcherCached.FetchByTermAsync. An exception was caught while fetching by term");
            throw;
        }
    }
}
