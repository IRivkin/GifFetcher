namespace GifFetcher.API.Models;

/// <summary>
/// Used to deserialize the provider response.
/// Although the home task requires the return of the URL, 
/// I allowed myself to add additional information that I consider useful to the result
/// </summary>
public record GifObject
{
    public required string Id { get; set; }
    public required string Url { get; set; }
    public required string Username { get; set; }
    public required string Title { get; set; }
}
