namespace Transactions.Api.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessagingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IConnectionFactory>(_ =>
        {
            var rabbitMqConnectionString = configuration.GetConnectionString("RabbitMq");

            return new ConnectionFactory
            {
                Uri = new Uri(rabbitMqConnectionString!),
            };
        });
        services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();

        return services;
    }
}
