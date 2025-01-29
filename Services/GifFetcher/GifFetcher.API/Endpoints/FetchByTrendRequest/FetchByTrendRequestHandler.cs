namespace GifFetcher.API.Endpoints.FetchByTrendRequest;

public record FetchByTrendRequestCommand(int Offset) : IRequest<FetchByTrendRequestResult>;
public record FetchByTrendRequestResult(GifFetchResult Result);

public class FetchByTrendRequestCommandHandler(IFetcher Fetcher)
    : IRequestHandler<FetchByTrendRequestCommand, FetchByTrendRequestResult>
{
    public async Task<FetchByTrendRequestResult> Handle(FetchByTrendRequestCommand command, CancellationToken cancellationToken)
    {
        var result = await Fetcher.FetchByTrendAsync(command.Offset, cancellationToken);

        return new FetchByTrendRequestResult(result);
    }
}
