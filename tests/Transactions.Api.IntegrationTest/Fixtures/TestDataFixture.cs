namespace Transactions.Api.IntegrationTest.Fixtures;

public static class TestDataFixture
{
    private static readonly Faker _faker = new();

    public static IEnumerable<object[]> InvalidTransactionRequestTestData()
    {
        yield return new object[]
        {
            new TransactionRequest()
            {
                TransactionId = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                Amount = -_faker.Random.Decimal(),
                Currency = _faker.Finance.Currency().Code,
                TransactionDate = _faker.Date.RecentOffset(),
            },
            nameof(TransactionRequest.Amount),
            "The amount must be greater than zero.",
        };

        yield return new object[]
        {
            new TransactionRequest()
            {
                TransactionId = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                Amount = _faker.Random.Decimal(),
                Currency = _faker.PickRandom(null!, string.Empty, " "),
                TransactionDate = _faker.Date.RecentOffset(),
            },
            nameof(TransactionRequest.Currency),
            "The currency must be provided.",
        };

        yield return new object[]
        {
            new TransactionRequest()
            {
                TransactionId = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                Amount = _faker.Random.Decimal(),
                Currency = _faker.Finance.Currency().Code,
                TransactionDate = default,
            },
            nameof(TransactionRequest.TransactionDate),
            "The transaction date must be provided.",
        };
    }
}
