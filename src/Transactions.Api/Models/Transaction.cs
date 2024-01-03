namespace Transactions.Api.Models;

public record Transaction
{
    public required Guid Id { get; init; }

    public required Guid AccountId { get; init; }

    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required DateTimeOffset Date { get; init; }

    public required Account Account { get; init; }
}
