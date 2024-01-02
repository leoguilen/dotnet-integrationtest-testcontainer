namespace Transactions.Api.IntegrationTest.Endpoints.V1;

[Trait("IntegrationTest", "TransactionEndpoints")]
public class TransactionEndpointsTest(
    CustomWebApplicationFactory factory,
    ITestOutputHelper outputHelper)
    : IntegrationTest(factory, outputHelper)
{
    private static readonly Guid _validTransactionId = Guid.Parse("e4d875b4-e6e9-4c6f-a50b-df648b8a1969");

    private static readonly Guid _validAccountId = Guid.Parse("7fb0f9ea-ded3-4747-bb9d-4fccfc1b8c36");

    [Fact]
    public async Task GetTransactionDetails_GivenInvalidTransactionId_ThenReturnsNotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/v1/transactions/{transactionId}");
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
        (await response.Content.ReadAsStringAsync()).Should().BeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetTransactionDetails_GivenValidTransactionId_ThenReturnsTransactionDetails()
    {
        // Arrange
        var transactionId = _validTransactionId;
        var expectedResponse = new TransactionResponse()
        {
            TransactionId = transactionId,
            AccountId = _validAccountId,
            Amount = 100,
            Currency = "EUR",
            Status = "Registered",
            TransactionDate = new DateTime(2021, 1, 1, 0, 0, 0),
        };

        // Act
        var response = await Client.GetAsync($"/api/v1/transactions/{transactionId}");
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<TransactionResponse?>()).Should().BeEquivalentTo(expectedResponse);
    }

    [Theory]
    [MemberData(nameof(TestDataFixture.InvalidTransactionRequestTestData), MemberType = typeof(TestDataFixture))]
    public async Task PostRegisterTransaction_GivenInvalidRequest_ThenReturnsBadRequest(
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
        var response = await Client.PostAsJsonAsync("/api/v1/transactions", request);
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        (await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>()).Should().BeEquivalentTo(
            expectation: expectedResponse,
            config => config.Including(x => x.Errors));
    }

    [Fact]
    public async Task PostRegisterTransaction_GivenInvalidAccount_ThenReturnsUnprocessableEntity()
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
        var response = await Client.PostAsJsonAsync("/api/v1/transactions", request);
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        (await response.Content.ReadFromJsonAsync<ProblemDetails>()).Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task PostRegisterTransaction_GivenRequestAmountGreaterThanAccountBalance_ThenReturnsUnprocessableEntity()
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
        var response = await Client.PostAsJsonAsync("/api/v1/transactions", request);
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        (await response.Content.ReadFromJsonAsync<ProblemDetails>()).Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task PostRegisterTransaction_GivenValidRequest_ThenReturnsCreated()
    {
        // Arrange
        var request = TransactionRequestFixture.Instance
            .WithAccountId(_validAccountId)
            .Generate();

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/transactions", request);
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        using (new AssertionScope())
        {
            response.Should()
                .HaveStatusCode(HttpStatusCode.Created).And
                .Match(res => res.Headers.Location!.ToString().Equals($"/api/v1/transactions/{request.TransactionId}"));
            await VerifyTransactionRecordInDatabaseAsync(request.TransactionId);
            VerifyTransactionMessageInBroker(request.TransactionId);
        }
    }

    [Fact]
    public async Task PostUndoTransaction_GivenInvalidTransactionId_ThenReturnsNotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var request = UndoTransactionRequestFixture.Instance.Generate();

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/transactions/{transactionId}/undo", request);
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
        (await response.Content.ReadAsStringAsync()).Should().BeNullOrWhiteSpace();
    }

    [Fact]
    public async Task PostUndoTransaction_GivenValidTransactionId_ThenReturnsNoContent()
    {
        // Arrange
        var transactionId = _validTransactionId;
        var request = UndoTransactionRequestFixture.Instance.Generate();

        // Act
        var response = await Client.PostAsJsonAsync($"/api/v1/transactions/{transactionId}/undo", request);
        Logger.WriteLine("Response: {0}", await response.Content.ReadAsStringAsync());

        // Assert
        using (new AssertionScope())
        {
            response.Should()
                .HaveStatusCode(HttpStatusCode.Created).And
                .Match(res => res.Headers.Location!.ToString().StartsWith("/api/v1/transactions/"));
            var transactionIdFromLocationHeader = Guid.Parse(response.Headers.Location!.OriginalString.Split('/').Last());
            await VerifyTransactionRecordInDatabaseAsync(transactionIdFromLocationHeader);
            VerifyTransactionMessageInBroker(transactionIdFromLocationHeader);
        }
    }
}
