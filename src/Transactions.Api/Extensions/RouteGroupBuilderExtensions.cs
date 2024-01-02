namespace Transactions.Api.Extensions;

internal static class RouteGroupBuilderExtensions
{
    internal static RouteGroupBuilder MapTransactionEndpoints(this RouteGroupBuilder builder)
    {
        builder
            .MapPost(
                pattern: "/transactions",
                handler: async (
                    [FromBody] TransactionRequest request,
                    [FromServices] ITransactionService transactionService,
                    CancellationToken requestAborted)
                    => await transactionService.RegisterAsync(request, requestAborted))
            .WithName("RegisterTransaction")
            .WithTags("Transactions")
            .WithOpenApi();

        builder
            .MapPost(
                pattern: "/transactions/{transactionId:guid}/undo",
                handler: async (
                    [FromRoute] Guid transactionId,
                    [FromBody] UndoTransactionRequest request,
                    [FromServices] ITransactionService transactionService,
                    CancellationToken requestAborted)
                    => await transactionService.UndoAsync(transactionId, request, requestAborted))
            .WithName("UndoTransaction")
            .WithTags("Transactions")
            .WithOpenApi();

        builder
            .MapGet(
                pattern: "/transactions/{transactionId:guid}",
                handler: async (
                    [FromRoute] Guid transactionId,
                    [FromServices] ITransactionService transactionService,
                    CancellationToken requestAborted)
                    => await transactionService.GetAsync(transactionId, requestAborted))
            .WithName("GetTransactionDetails")
            .WithTags("Transactions")
            .WithOpenApi();

        return builder;
    }
}
