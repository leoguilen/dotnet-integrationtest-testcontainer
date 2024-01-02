namespace Transactions.Api.Contracts.Responses;

public readonly record struct TransactionResponse(
    Guid TransactionId,
    Guid AccountId,
    decimal Amount,
    string Currency,
    DateTimeOffset TransactionDate,
    string Status)
{
    public static TransactionResponse From(Transaction transaction) => new(
        transaction.Id,
        transaction.AccountId,
        transaction.Amount,
        transaction.Currency,
        transaction.Date,
        transaction.ParentTransactionId.HasValue ? "Undone" : "Registered");
}
