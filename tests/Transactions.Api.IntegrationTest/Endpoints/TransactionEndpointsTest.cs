namespace Transactions.Api.IntegrationTest.Endpoints;

[Trait("IntegrationTest", "TransactionEndpoints")]
public class TransactionEndpointsTest(
    CustomWebApplicationFactory factory,
    ITestOutputHelper outputHelper)
    : IntegrationTest(factory, outputHelper)
{
    private static readonly Guid _validAccountId = Guid.Parse("7fb0f9ea-ded3-4747-bb9d-4fccfc1b8c36");

    [Theory]
    [MemberData(nameof(TestDataFixture.InvalidTransactionRequestTestData), MemberType = typeof(TestDataFixture))]
    public async Task PostTransaction_GivenInvalidRequest_ThenReturnsBadRequest(
        TransactionRequest request,
        string propertyName,
        string errorMessage)
    {
        // Arrange
        var expectedResponse = new HttpValidationProblemDetails()
        {
            Errors = new Dictionary<string, string[]>()
            {
                [propertyName] = [errorMessage],
            },
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/transactions", request);
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        (await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>()).Should().BeEquivalentTo(
            expectation: expectedResponse,
            config => config.Including(x => x.Errors));
    }

    [Fact]
    public async Task PostTransaction_GivenInvalidAccount_ThenReturnsUnprocessableEntity()
    {
        // Arrange
        var request = TransactionRequestFixture.Instance.Generate();
        var expectedResponse = new ProblemDetails()
        {
            Detail = $"The account with id {request.AccountId} does not exist.",
            Instance = $"urn:transactions:register:{request.TransactionId}",
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "The account does not exist.",
            Type = "https://httpstatuses.com/422",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/transactions", request);
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        (await response.Content.ReadFromJsonAsync<ProblemDetails>()).Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task PostTransaction_GivenRequestAmountGreaterThanAccountBalance_ThenReturnsUnprocessableEntity()
    {
        // Arrange
        var request = TransactionRequestFixture.Instance
            .WithAccountId(_validAccountId)
            .WithAmount(1_000_000)
            .Generate();
        var expectedResponse = new ProblemDetails()
        {
            Detail = $"The account with id {request.AccountId} does not have enough balance.",
            Instance = $"urn:transactions:register:{request.TransactionId}",
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "The account does not have enough balance.",
            Type = "https://httpstatuses.com/422",
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/transactions", request);
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        (await response.Content.ReadFromJsonAsync<ProblemDetails>()).Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task PostTransaction_GivenValidRequest_ThenReturnsCreated()
    {
        // Arrange
        var request = TransactionRequestFixture.Instance
            .WithAccountId(_validAccountId)
            .Generate();

        // Act
        var response = await Client.PostAsJsonAsync("/api/transactions", request);
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        using (new AssertionScope())
        {
            response.Should()
                .HaveStatusCode(HttpStatusCode.Created).And
                .Match(res => res.Headers.Location!.ToString().Equals($"/api/transactions/{request.TransactionId}"));
            await VerifyTransactionRecordInDatabaseAsync(request.TransactionId);
            VerifyTransactionMessageInBroker(request.TransactionId);
        }
    }
}
