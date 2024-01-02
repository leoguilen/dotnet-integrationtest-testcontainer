namespace Transactions.Api.IntegrationTest.Containers.WireMock;

public sealed class WireMockContainer(
    IContainerConfiguration configuration,
    ILogger logger)
    : DockerContainer(configuration, logger)
{
    public Uri GetBaseUrl() => new UriBuilder("http", Hostname, GetMappedPublicPort(WireMockBuilder.WireMockPort)).Uri;
}
