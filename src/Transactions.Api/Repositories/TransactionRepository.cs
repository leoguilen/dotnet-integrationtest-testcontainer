namespace Transactions.Api.Repositories;

internal sealed class TransactionRepository(IDbConnection dbConnection) : ITransactionRepository
{
    public async Task<Transaction?> AddAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            commandText:
            """
            INSERT INTO "TRANSACTIONS" ("ID", "ACCOUNT_ID", "AMOUNT", "CURRENCY", "DATE", "PARENT_TRANSACTION_ID", "OBSERVATION")
            VALUES (@Id, @AccountId, @Amount, @Currency, @Date, @ParentTransactionId, @Observation)
            RETURNING
                "ID" AS "Id",
                "ACCOUNT_ID" AS "AccountId",
                "AMOUNT" AS "Amount",
                "CURRENCY" AS "Currency",
                "DATE" AS "Date",
                "PARENT_TRANSACTION_ID" AS "ParentTransactionId",
                "OBSERVATION" AS "Observation";
            """,
            parameters: transaction,
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        using var reader = await dbConnection.ExecuteReaderAsync(command, CommandBehavior.SingleRow);

        return reader.Parse<Transaction?>().SingleOrDefault();
    }

    public Task<Transaction?> GetByIdAsync(
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            commandText:
            """
            SELECT
                "ID" AS "Id",
                "ACCOUNT_ID" AS "AccountId",
                "AMOUNT" AS "Amount",
                "CURRENCY" AS "Currency",
                "DATE" AS "Date",
                "PARENT_TRANSACTION_ID" AS "ParentTransactionId",
                "OBSERVATION" AS "Observation"
            FROM "TRANSACTIONS"
            WHERE "ID" = @TransactionId;
            """,
            parameters: new { transactionId },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        return dbConnection.QuerySingleOrDefaultAsync<Transaction?>(command);
    }
}
