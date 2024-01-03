namespace Transactions.Api.ExternalServices;

public interface ICustomerAccountServiceClient
{
    Task<Account?> FetchByIdAsync(Guid accountId, CancellationToken cancellationToken = default);
}
