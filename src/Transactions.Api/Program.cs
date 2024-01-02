var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IAccountServiceClient, HttpAccountServiceClient>(client =>
{
    var accountApiConfig = builder.Configuration.GetRequiredSection("AccountApi");

    client.BaseAddress = accountApiConfig.GetValue<Uri>("BaseUrl");
    client.Timeout = accountApiConfig.GetValue<TimeSpan>("Timeout");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "Transactions.Api/1.0");
    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
});
builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped(_ =>
{
    var rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMq");

    var factory = new ConnectionFactory
    {
        Uri = new Uri(rabbitMqConnectionString!),
    };

    return factory.CreateConnection();
});
builder.Services.AddScoped<IPublisher, RabbitMqMessagePublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var v1 = app.MapGroup("/api/v1");

v1.MapTransactionEndpoints();

await app
    .RunAsync()
    .ConfigureAwait(false);

public partial class Program { }