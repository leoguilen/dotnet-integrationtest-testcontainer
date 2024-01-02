namespace Transactions.Api.Models;

public record Account
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required decimal Balance { get; init; }

    public bool IsBalanceEnough(decimal amount) => Balance >= amount;
}
