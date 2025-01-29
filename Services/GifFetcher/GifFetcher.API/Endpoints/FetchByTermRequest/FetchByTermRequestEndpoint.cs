namespace GifFetcher.API.Endpoints.FetchByTermRequest;

public record FetchByTermResponse(GifFetchResult Result);

public class FetchByTermRequestEndpoint() : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/fetchbyterm",
            async (string term, int offset, ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new FetchByTermRequestCommand(term, offset);

                    // Send to FetchByTermRequestCommandHandler.Handle
                    var result = await sender.Send(command, cancellationToken);
                    var response = result.Adapt<FetchByTermResponse>();

                    return Results.Ok(response);
                }
                catch (Exception)
                {
                    throw;
                }
            });
    }
}
