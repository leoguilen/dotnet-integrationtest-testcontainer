namespace Transactions.Api.Contracts.Requests;

public record UndoTransactionRequest
{
    public string? Reason { get; init; } = "Not specified.";
}
