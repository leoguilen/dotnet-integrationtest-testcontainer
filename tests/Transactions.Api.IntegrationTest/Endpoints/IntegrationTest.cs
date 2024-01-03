namespace Transactions.Api.IntegrationTest.Endpoints;

[Collection(name: nameof(CustomWebApplicationFactoryCollection))]
public abstract class IntegrationTest(
    CustomWebApplicationFactory factory,
    ITestOutputHelper outputHelper)
{
    private readonly AsyncServiceScope _integrationTestScope = factory.Services.CreateAsyncScope();

    protected HttpClient Client => factory.Server.CreateClient();

    protected ITestOutputHelper Logger => outputHelper;

    protected async Task VerifyTransactionRecordInDatabaseAsync(Guid transactionId)
    {
        var repository = _integrationTestScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var transaction = await repository.GetByIdAsync(transactionId);
        transaction?.Id.Should().Be(transactionId, because: "the transaction id should match the one in the database");
    }

    protected void VerifyTransactionMessageInBroker(Guid transactionId)
    {
        var connectionFactory = _integrationTestScope.ServiceProvider.GetRequiredService<IConnectionFactory>();

        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        var message = channel.BasicGet(queue: "transactions", autoAck: false);

        var transaction = JsonSerializer.Deserialize<Transaction?>(message.Body.ToArray());
        transaction?.Id.Should().Be(transactionId, because: "the transaction id should match the one in the broker");
    }
}
