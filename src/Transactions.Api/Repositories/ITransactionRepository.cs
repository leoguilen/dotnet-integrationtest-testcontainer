namespace Transactions.Api.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default);

    Task<Transaction?> AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
}
