namespace Transactions.Api.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(configuration.GetConnectionString("Database")));
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}
