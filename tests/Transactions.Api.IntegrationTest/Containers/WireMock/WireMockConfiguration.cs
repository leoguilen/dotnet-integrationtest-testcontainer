namespace Transactions.Api.IntegrationTest.Containers.WireMock;

public sealed class WireMockConfiguration : ContainerConfiguration
{
    public WireMockConfiguration()
    {
    }

    public WireMockConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    public WireMockConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    public WireMockConfiguration(WireMockConfiguration resourceConfiguration)
        : this(new WireMockConfiguration(), resourceConfiguration)
    {
    }

    public WireMockConfiguration(WireMockConfiguration oldValue, WireMockConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}
