var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExternalServicesHttpClients(builder.Configuration);
builder.Services.AddMessagingServices(builder.Configuration);
builder.Services.AddDataRepositories(builder.Configuration);
builder.Services.AddScoped<ITransactionProcessingService, TransactionProcessingService>();

var app = builder.Build();

var api = app.MapGroup("/api");

api.MapPost(
    pattern: "/transactions",
    handler: async (
        [FromBody] TransactionRequest request,
        [FromServices] ITransactionProcessingService processingService,
        CancellationToken requestAborted)
        => await processingService.ExecuteAsync(request, requestAborted))
.WithName("RegisterTransaction")
.WithTags("Transactions")
.WithOpenApi();

await app
    .RunAsync()
    .ConfigureAwait(false);

public partial class Program { }