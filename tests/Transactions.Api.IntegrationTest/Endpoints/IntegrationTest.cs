namespace Transactions.Api.IntegrationTest.Endpoints;

[Collection(nameof(WebApplicationFactoryCollection))]
public abstract class IntegrationTest(
    CustomWebApplicationFactory factory,
    ITestOutputHelper outputHelper)
{
    protected HttpClient Client => factory.Server.CreateClient();

    protected ITestOutputHelper Logger => outputHelper;

    protected async Task VerifyTransactionRecordInDatabaseAsync(Guid transactionId)
    {
        var repository = factory.Services.GetRequiredService<ITransactionRepository>();
        var transaction = await repository.GetByIdAsync(transactionId);
        transaction?.Id.Should().Be(transactionId, because: "the transaction id should match the one in the database");
    }

    protected void VerifyTransactionMessageInBroker(Guid transactionId)
    {
        var connection = factory.Services.GetRequiredService<IConnection>();
        var channel = connection.CreateModel();

        var message = channel.BasicGet(queue: "transactions", autoAck: false);

        var transaction = JsonSerializer.Deserialize<Transaction?>(message.Body.ToArray());
        transaction?.Id.Should().Be(transactionId, because: "the transaction id should match the one in the broker");
    }
}
