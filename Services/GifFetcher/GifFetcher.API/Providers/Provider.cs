namespace GifFetcher.API.Providers;

public class Provider : IProvider
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly string _fetchByTrendRoute;
    private readonly string _fetchByTermRoute;
    private readonly string _fetchByTrendParams;
    private readonly string _fetchByTermParams;
    private readonly int _fetchRequestTimeoutSec;

    public Provider(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<Provider> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;

        // In the production environment, the key value is stored in a KeyVault
        var providerApiKey = _configuration.GetValue<string>("AppSettings:ProviderApiKey");
        var fetchByTrendRoute = _configuration.GetValue<string>("AppSettings:Routs:FetchByTrend");
        var fetchByTermRoute = _configuration.GetValue<string>("AppSettings:Routs:FetchByTerm");

        _fetchByTrendRoute = string.Format(fetchByTrendRoute!, providerApiKey);
        _fetchByTermRoute = string.Format(fetchByTermRoute!, providerApiKey);
        _fetchByTrendParams = _configuration.GetValue<string>("AppSettings:Params:FetchByTrend")!;
        _fetchByTermParams = _configuration.GetValue<string>("AppSettings:Params:FetchByTerm")!;
        _fetchRequestTimeoutSec = _configuration.GetValue<int>("AppSettings:FetchRequestTimeoutSec");
    }

    public async ValueTask<GifFetchResult> FetchByTrendAsync(int offset, CancellationToken cancellationToken)
    {
        try
        {
            var parameters = string.Format(_fetchByTrendParams, offset);
            var route = $"{_fetchByTrendRoute}{parameters}";

            return await FetchAsync(route, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider.FetchByTrendAsync. An exception was caught while fetching by trend");
            throw;
        }
    }

    public async ValueTask<GifFetchResult> FetchByTermAsync(string term, int offset, CancellationToken cancellationToken)
    {
        try
        {
            var parameters = string.Format(_fetchByTermParams, term, offset);
            var route = $"{_fetchByTermRoute}{parameters}";

            return await FetchAsync(route, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider.FetchByTermAsync. An exception was caught while fetching by term");
            throw;
        }
    }

    private async ValueTask<GifFetchResult> FetchAsync(string route, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient("GifsProvider");
        var url = $"{client.BaseAddress}{route}";
        client.DefaultRequestHeaders.ExpectContinue = false;

        // To distinguish a "normal" cancellation from a timeout
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(_fetchRequestTimeoutSec));

        try
        {
            var response = await client.GetAsync(url, cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                var message = 
                    @$"Unsuccessful response from provider.
                       StatusCode: {response.StatusCode} Reason: {response.ReasonPhrase}";
                _logger.LogError(
                    @$"Provider.FetchGifsAsync.
                       Request error while accessing the gif provider service {url}. 
                       {message}");

                throw new HttpBadResponseException(new Exception(message));
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var gifFetchResult = JsonConvert.DeserializeObject<GifFetchResult>(responseContent)!;

            return gifFetchResult;
        }
        catch (OperationCanceledException)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                // Normal cancellation
                _logger.LogWarning("Provider.FetchGifsAsync. Get request was cancelled");
            }

            // Timeout
            _logger.LogError(
                @$"Provider.FetchGifsAsync. Sending a request
                   to the gif provider service '{url}' was timeouted. {DateTime.Now}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                @$"Provider.FetchGifsAsync. An exception was caught while accessing the gif provider service
                  '{url}'");
            throw;
        }
    }
}
