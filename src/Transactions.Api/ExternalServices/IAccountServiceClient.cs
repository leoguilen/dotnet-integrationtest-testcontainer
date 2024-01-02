namespace Transactions.Api.ExternalServices;

public interface IAccountServiceClient
{
    Task<Account?> FetchByIdAsync(Guid accountId, CancellationToken cancellationToken = default);
}
