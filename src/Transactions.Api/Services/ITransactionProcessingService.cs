namespace Transactions.Api.Services;

public interface ITransactionProcessingService
{
    Task<IResult> ExecuteAsync(TransactionRequest request, CancellationToken cancellationToken = default);
}
