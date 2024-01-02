namespace Transactions.Api.IntegrationTest;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly ContainersFixture _containersFixture = new();

    public async Task InitializeAsync() => await _containersFixture.InitializeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration((context, config) => config
            .AddInMemoryCollection([
                new KeyValuePair<string, string?>("ConnectionStrings:Database", _containersFixture.PostgreSqlContainer.GetConnectionString()),
                new KeyValuePair<string, string?>("ConnectionStrings:RabbitMq", _containersFixture.RabbitMqContainer.GetConnectionString()),
                new KeyValuePair<string, string?>("AccountApi:BaseUrl", _containersFixture.WireMockContainer.GetBaseUrl().ToString()),
                new KeyValuePair<string, string?>("AccountApi:Timeout", "00:00:30"),
            ]));
        builder.UseTestServer();
    }

    async Task IAsyncLifetime.DisposeAsync() => await _containersFixture.DisposeAsync();
}

[CollectionDefinition(nameof(WebApplicationFactoryCollection))]
public class WebApplicationFactoryCollection : ICollectionFixture<CustomWebApplicationFactory>;