namespace Transactions.Api.Messaging;

internal sealed class RabbitMqMessagePublisher(IConnectionFactory connectionFactory) : IMessagePublisher
{
    private const string QueueName = "transactions";

    public Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        channel.ConfirmSelect();

        var properties = channel.CreateBasicProperties();
        properties.MessageId = Guid.NewGuid().ToString();
        properties.ContentType = "application/json";
        properties.ContentEncoding = "utf-8";
        properties.Headers = new Dictionary<string, object>
        {
            { "x-user-agent", "Transactions.Api/1.0" },
        };


        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        channel.WaitForConfirmsOrDie();
        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: QueueName,
            mandatory: true,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }
}
