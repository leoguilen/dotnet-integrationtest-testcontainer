namespace Transactions.Api.Services;

public interface ITransactionService
{
    Task<IResult> RegisterAsync(TransactionRequest request, CancellationToken cancellationToken = default);

    Task<IResult> UndoAsync(Guid transactionId, UndoTransactionRequest request, CancellationToken cancellationToken = default);

    Task<IResult> GetAsync(Guid transactionId, CancellationToken cancellationToken = default);
}
