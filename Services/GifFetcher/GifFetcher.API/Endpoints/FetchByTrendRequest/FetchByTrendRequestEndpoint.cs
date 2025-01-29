namespace GifFetcher.API.Endpoints.FetchByTrendRequest;

public record FetchByTrendResponse(GifFetchResult Result);

public class FetchByTrendRequestEndpoint() : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/fetchbytrend",
            async (int offset, ISender sender, CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new FetchByTrendRequestCommand(offset);

                    // Send to FetchByTrendRequestCommandHandler.Handle
                    var result = await sender.Send(command, cancellationToken);
                    var response = result.Adapt<FetchByTrendResponse>();

                    return Results.Ok(response);
                }
                catch (Exception)
                {
                    throw;
                }
            });
    }
}
