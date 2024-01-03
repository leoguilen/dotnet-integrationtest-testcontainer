namespace Transactions.Api.ExternalServices;

internal sealed class HttpCustomerAccountServiceClient(
    HttpClient client,
    ILogger<HttpCustomerAccountServiceClient> logger)
    : ICustomerAccountServiceClient
{
    public async Task<Account?> FetchByIdAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.GetAsync(
                requestUri: $"/api/v1/accounts/{accountId}",
                completionOption: HttpCompletionOption.ResponseHeadersRead,
                cancellationToken: cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Account>(cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch account with id {AccountId}", accountId);
            throw;
        }
    }
}
