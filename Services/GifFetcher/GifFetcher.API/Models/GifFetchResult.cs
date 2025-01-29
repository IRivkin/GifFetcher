namespace GifFetcher.API.Models;

/// <summary>
/// Used to deserialize the provider response
/// </summary>
public record GifFetchResult
{
    public required List<GifObject> Data { get; set; }
}
