namespace Transactions.Api.IntegrationTest.Fixtures;

public class ContainersFixture : IAsyncLifetime
{
    public ContainersFixture()
    {
        PostgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:alpine")
            .WithDatabase("transactions")
            .WithResourceMapping(
                resourceContent: Encoding.UTF8.GetBytes(
                    """
                    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
                    CREATE TABLE IF NOT EXISTS "TRANSACTIONS" (
                        "ID" UUID NOT NULL,
                        "ACCOUNT_ID" UUID NOT NULL,
                        "AMOUNT" NUMERIC(10,2) NOT NULL,
                        "CURRENCY" VARCHAR(3) NOT NULL,
                        "DATE" timestamp NOT NULL,
                        "PARENT_TRANSACTION_ID" UUID NULL,
                        "OBSERVATION" VARCHAR(255) NULL,
                        PRIMARY KEY ("ID")
                    );
                    INSERT INTO "TRANSACTIONS"
                    VALUES ('e4d875b4-e6e9-4c6f-a50b-df648b8a1969', '7fb0f9ea-ded3-4747-bb9d-4fccfc1b8c36', 100, 'EUR', '2021-01-01 00:00:00', NULL, NULL);
                    """
                ),
                filePath: "/docker-entrypoint-initdb.d/init.sql")
            .WithAutoRemove(true)
            .Build();

        WireMockContainer = new WireMockBuilder()
            .WithImage("wiremock/wiremock:latest-alpine")
            .WithResourceMapping(
                resourceContent: Encoding.UTF8.GetBytes(
                    """
                    {
                        "id": "7fb0f9ea-ded3-4747-bb9d-4fccfc1b8c36",
                        "name": "test",
                        "balance": 100
                    }
                    """
                ),
                filePath: "/home/wiremock/__files/account.json")
            .WithResourceMapping(
                resourceContent: Encoding.UTF8.GetBytes(
                    """
                    {
                        "mappings": [
                            {
                                "priority": 1,
                                "request": {
                                    "urlPathTemplate": "/api/v1/accounts/{accountId}",
                                    "method": "GET",
                                    "pathParameters": {
                                        "accountId": {
                                            "equalTo": "7fb0f9ea-ded3-4747-bb9d-4fccfc1b8c36"
                                        }
                                    }
                                },
                                "response": {
                                    "status": 200,
                                    "bodyFileName": "account.json",
                                    "headers": {
                                        "Content-Type": "application/json"
                                    }
                                }
                            },
                            {
                                "priority": 10,
                                "request": {
                                    "urlPathTemplate": "/api/v1/accounts/{accountId}",
                                    "method": "GET"
                                },
                                "response": {
                                    "status": 404,
                                    "headers": {}
                                }
                            }
                        ]
                    }
                    """
                ),
                filePath: "/home/wiremock/mappings/account-api.json")
            .WithAutoRemove(true)
            .Build();

        RabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.11-alpine")
            .WithAutoRemove(true)
            .Build();
    }

    public PostgreSqlContainer PostgreSqlContainer { get; }

    public WireMockContainer WireMockContainer { get; }

    public RabbitMqContainer RabbitMqContainer { get; }

    public Task DisposeAsync() => Task.WhenAll(
            PostgreSqlContainer.DisposeAsync().AsTask(),
            WireMockContainer.DisposeAsync().AsTask(),
            RabbitMqContainer.DisposeAsync().AsTask());

    public Task InitializeAsync() => Task.WhenAll(
            PostgreSqlContainer.StartAsync(),
            WireMockContainer.StartAsync(),
            RabbitMqContainer.StartAsync());
}
