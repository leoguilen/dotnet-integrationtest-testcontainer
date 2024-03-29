global using Xunit;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using System.Net;
global using FluentAssertions;
global using System.Text;
global using Testcontainers.PostgreSql;
global using Transactions.Api.IntegrationTest.Containers.WireMock;
global using Microsoft.Extensions.Configuration;
global using Transactions.Api.IntegrationTest.Fixtures;
global using DotNet.Testcontainers.Configurations;
global using DotNet.Testcontainers.Containers;
global using Microsoft.Extensions.Logging;
global using Docker.DotNet.Models;
global using DotNet.Testcontainers.Builders;
global using System.Net.Http.Json;
global using Microsoft.AspNetCore.Http;
global using Transactions.Api.Contracts.Requests;
global using Bogus;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.DependencyInjection;
global using Transactions.Api.Models;
global using Transactions.Api.Repositories;
global using Testcontainers.RabbitMq;
global using System.Text.Json;
global using RabbitMQ.Client;
global using FluentAssertions.Execution;
global using Xunit.Abstractions;
