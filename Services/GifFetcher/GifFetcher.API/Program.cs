var builder = WebApplication.CreateBuilder(args);

AddCarter(builder);
AddMediatR(builder);
AddHttpClient(builder);
AddHealthCheck(builder);
AddExceptionHandler(builder);
AddFeatureManagement(builder);
AddServices(builder);
AddSwagger(builder);

var app = builder.Build();

UseCarter(app);
UseExceptionHandler(app);
UseHealthChecks(app);
UseSwagger(app);

app.Run();

void AddCarter(WebApplicationBuilder builder) => builder.Services.AddCarter();

void AddMediatR(WebApplicationBuilder builder)
{
    builder.Services.AddMediatR(config =>
    {
        // Find all endpoints in an assembly
        config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    });
}

void AddHttpClient(WebApplicationBuilder builder)
{
    var providerUri = builder.Configuration.GetValue<string>("AppSettings:ProviderUrl")!;

    builder.Services.AddHttpClient("GifsProvider", client =>
    {
        client.BaseAddress = new Uri(providerUri);
    }).ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

        if (environment.Equals("Development"))
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        return handler;
    });
}

void AddHealthCheck(WebApplicationBuilder builder) => builder.Services.AddHealthChecks();

void AddExceptionHandler(WebApplicationBuilder builder)
{
    // Generic exception handler
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    // Handler for custom exceptions thrown when receiving an unsuccessful response from the provider
    builder.Services.AddExceptionHandler<HttpResponseExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddSingleton<IProvider, Provider>();
    builder.Services.AddScoped<IFetcher, Fetcher>();
    builder.Services.Decorate<IFetcher, FetcherCached>();
}

void AddFeatureManagement(WebApplicationBuilder builder) => builder.Services.AddFeatureManagement();

void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<ICacheEmulated, FetcherCachedEmulated>();
    builder.Services.AddSingleton<IFetcher, Fetcher>();
    // Decorator design pattern
    builder.Services.Decorate<IFetcher, FetcherCached>();
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
    });
}

void AddSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

void UseCarter(WebApplication app) => app.MapCarter();

void UseExceptionHandler(WebApplication app) => app.UseExceptionHandler();

void UseHealthChecks(WebApplication app)
{
    app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
}

void UseSwagger(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.DefaultModelsExpandDepth(-1);// Schema filter is useless 
            options.RoutePrefix = string.Empty;
        });
    }
}
