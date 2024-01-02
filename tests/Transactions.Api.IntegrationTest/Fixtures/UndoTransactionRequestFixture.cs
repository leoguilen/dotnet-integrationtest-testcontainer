namespace Transactions.Api.IntegrationTest.Fixtures;

public class UndoTransactionRequestFixture : Faker<UndoTransactionRequest>
{
    public static readonly UndoTransactionRequestFixture Instance = new();

    public UndoTransactionRequestFixture()
    {
        RuleFor(x => x.Reason, f => f.Lorem.Sentence());
    }
}
