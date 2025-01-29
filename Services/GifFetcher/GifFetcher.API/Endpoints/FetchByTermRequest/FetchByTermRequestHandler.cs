namespace GifFetcher.API.Endpoints.FetchByTermRequest;

public record FetchByTermRequestCommand(string Term, int Offset) : IRequest<FetchByTermRequestResult>;
public record FetchByTermRequestResult(GifFetchResult Result);

public class FetchByTermRequestCommandHandler(IFetcher Fetcher)
    : IRequestHandler<FetchByTermRequestCommand, FetchByTermRequestResult>
{
    public async Task<FetchByTermRequestResult> Handle(FetchByTermRequestCommand command, CancellationToken cancellationToken)
    {
        var result = await Fetcher.FetchByTermAsync(command.Term, command.Offset, cancellationToken);

        return new FetchByTermRequestResult(result);
    }
}
