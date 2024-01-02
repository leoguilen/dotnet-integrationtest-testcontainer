namespace Transactions.Api.IntegrationTest.Containers.WireMock;

public sealed class WireMockBuilder(WireMockConfiguration resourceConfiguration)
    : ContainerBuilder<WireMockBuilder, WireMockContainer, WireMockConfiguration>(resourceConfiguration)
{
    public const string WireMockImage = "wiremock/wiremock:3.3.1-1-alpine";

    public const ushort WireMockPort = 8080;

    public WireMockBuilder()
        : this(new WireMockConfiguration())
    {
        DockerResourceConfiguration = base.Init().DockerResourceConfiguration;
    }

    protected override WireMockConfiguration DockerResourceConfiguration { get; } = resourceConfiguration;

    public override WireMockContainer Build()
    {
        Validate();
        return new WireMockContainer(Init().DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    protected override WireMockBuilder Init()
    {
        return base.Init()
            .WithImage(WireMockImage)
            .WithPortBinding(WireMockPort, true)
            .WithEntrypoint("/docker-entrypoint.sh", "--global-response-templating", "--disable-gzip", "--verbose")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(s => s
                .WithMethod(HttpMethod.Get)
                .ForPort(WireMockPort)
                .ForPath("/__admin/health")
                .ForStatusCode(HttpStatusCode.OK)));
    }

    protected override void Validate() => base.Validate();

    protected override WireMockBuilder Clone(IContainerConfiguration resourceConfiguration)
        => Merge(DockerResourceConfiguration, new WireMockConfiguration(resourceConfiguration));

    protected override WireMockBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        => Merge(DockerResourceConfiguration, new WireMockConfiguration(resourceConfiguration));

    protected override WireMockBuilder Merge(WireMockConfiguration oldValue, WireMockConfiguration newValue)
        => new(new WireMockConfiguration(oldValue, newValue));
}
