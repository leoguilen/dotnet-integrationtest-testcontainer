namespace Transactions.Api.Services;

internal sealed class TransactionService(
    IAccountServiceClient accountServiceClient,
    ITransactionRepository transactionRepository,
    IPublisher publisher)
    : ITransactionService
{
    public async Task<IResult> GetAsync(
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await transactionRepository
                .GetByIdAsync(transactionId, cancellationToken);

            return transaction is null
                ? Results.NotFound()
                : Results.Ok(TransactionResponse.From(transaction));
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                instance: $"urn:transactions:get:{transactionId}",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An error occurred while getting the transaction.",
                type: "https://httpstatuses.com/500");
        }
    }

    public async Task<IResult> RegisterAsync(
        TransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResults = request.Validate(new ValidationContext(request));
            if (validationResults.Any())
            {
                return Results.ValidationProblem(
                    errors: validationResults.ToDictionary(
                        keySelector: validationResult => validationResult.MemberNames.First(),
                        elementSelector: validationResult => new[] { validationResult.ErrorMessage! }),
                    detail: "The request payload contains invalid data.",
                    instance: $"urn:transactions:register:{request.TransactionId}",
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "The request is invalid.",
                    type: "https://httpstatuses.com/400"
                );
            }

            var account = await accountServiceClient.FetchByIdAsync(request.AccountId, cancellationToken);
            if (account is null)
            {
                return Results.UnprocessableEntity(new ProblemDetails
                {
                    Detail = $"The account with id {request.AccountId} does not exist.",
                    Instance = $"urn:transactions:register:{request.TransactionId}",
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "The account does not exist.",
                    Type = "https://httpstatuses.com/422"
                });
            }

            if (!account.IsBalanceEnough(request.Amount))
            {
                return Results.UnprocessableEntity(new ProblemDetails
                {
                    Detail = $"The account with id {request.AccountId} does not have enough balance.",
                    Instance = $"urn:transactions:register:{request.TransactionId}",
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "The account does not have enough balance.",
                    Type = "https://httpstatuses.com/422"
                });
            }

            var transaction = new Transaction()
            {
                Id = request.TransactionId,
                AccountId = account.Id,
                Account = account,
                Amount = request.Amount,
                Currency = request.Currency,
                Date = request.TransactionDate,
            };

            var registeredTransaction = await transactionRepository.AddAsync(transaction, cancellationToken);

            await publisher.PublishAsync(registeredTransaction!, cancellationToken);

            return Results.Created(
                uri: $"/api/v1/transactions/{registeredTransaction!.Id}",
                value: null);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                instance: $"urn:transactions:register:{request.TransactionId}",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An error occurred while registering the transaction.",
                type: "https://httpstatuses.com/500");
        }
    }

    public async Task<IResult> UndoAsync(
        Guid transactionId,
        UndoTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await transactionRepository
                .GetByIdAsync(transactionId, cancellationToken);
            if (transaction is null)
            {
                return Results.NotFound();
            }

            var transactionUndone = transaction.Undo(request.Reason);

            var registeredTransaction = await transactionRepository
                .AddAsync(transactionUndone, cancellationToken);

            await publisher.PublishAsync(registeredTransaction!, cancellationToken);

            return Results.Created(
                uri: $"/api/v1/transactions/{registeredTransaction!.Id}",
                value: null);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                instance: $"urn:transactions:undo:{transactionId}",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An error occurred while undoing the transaction.",
                type: "https://httpstatuses.com/500");
        }
    }
}
