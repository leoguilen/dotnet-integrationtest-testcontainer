namespace Transactions.Api.IntegrationTest.Fixtures;

public sealed class TransactionRequestFixture : Faker<TransactionRequest>
{
    public static TransactionRequestFixture Instance => new();

    public TransactionRequestFixture()
    {
        RuleFor(x => x.TransactionId, faker => faker.Random.Guid());
        RuleFor(x => x.AccountId, faker => faker.Random.Guid());
        RuleFor(x => x.Amount, faker => faker.Finance.Amount(0, 100));
        RuleFor(x => x.Currency, faker => faker.Finance.Currency().Code);
        RuleFor(x => x.TransactionDate, faker => faker.Date.RecentOffset());
    }

    public TransactionRequestFixture WithAccountId(Guid accountId)
    {
        RuleFor(x => x.AccountId, _ => accountId);
        return this;
    }

    public TransactionRequestFixture WithAmount(decimal amount)
    {
        RuleFor(x => x.Amount, _ => amount);
        return this;
    }
}
