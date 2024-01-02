namespace Transactions.Api.Contracts.Requests;

public record TransactionRequest : IValidatableObject
{
    public required Guid TransactionId { get; init; }

    public required Guid AccountId { get; init; }

    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required DateTimeOffset TransactionDate { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Amount <= 0)
        {
            yield return new ValidationResult(
                "The amount must be greater than zero.",
                new[] { nameof(Amount) });
        }

        if (string.IsNullOrWhiteSpace(Currency))
        {
            yield return new ValidationResult(
                "The currency must be provided.",
                new[] { nameof(Currency) });
        }

        if (TransactionDate == default)
        {
            yield return new ValidationResult(
                "The transaction date must be provided.",
                new[] { nameof(TransactionDate) });
        }
    }
}
