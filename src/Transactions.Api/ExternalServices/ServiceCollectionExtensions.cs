namespace Transactions.Api.ExternalServices;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternalServicesHttpClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<ICustomerAccountServiceClient, HttpCustomerAccountServiceClient>(client =>
        {
            var accountApiConfig = configuration.GetRequiredSection("AccountApi");

            client.BaseAddress = accountApiConfig.GetValue<Uri>("BaseUrl");
            client.Timeout = accountApiConfig.GetValue<TimeSpan>("Timeout");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "Transactions.Api/1.0");
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        });

        return services;
    }
}
