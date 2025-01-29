namespace GifFetcher.API.Fetchers;

/// <summary>
/// This class emulates the Redis interface.
/// </summary>
public class FetcherCachedEmulated : ICacheEmulated
{
    private readonly ReaderWriterLockSlim _cacheLocker = new ReaderWriterLockSlim();
    private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();

    public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        try
        {
            _cacheLocker.EnterReadLock();
            var result =_cache.ContainsKey(key) ? _cache[key] : null;
            return Task.FromResult(result);
        }
        finally
        {
            _cacheLocker.ExitReadLock();
        }
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        try
        {
            _cacheLocker.EnterWriteLock();
            if (_cache.ContainsKey(key)) 
            {
                _cache[key] = value;
            }
            else
            {
                _cache.Add(key, value);
            }

            return Task.FromResult(0);
        }
        finally
        {
            _cacheLocker.ExitWriteLock();
        }
    }

    public byte[]? Get(string key) => throw new NotImplementedException();
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options) => throw new NotImplementedException();
    public void Refresh(string key)=> throw new NotImplementedException();
    public Task RefreshAsync(string key, CancellationToken token = default) => throw new NotImplementedException();
    public void Remove(string key)=> throw new NotImplementedException();
    public Task RemoveAsync(string key, CancellationToken token = default) => throw new NotImplementedException();
}
