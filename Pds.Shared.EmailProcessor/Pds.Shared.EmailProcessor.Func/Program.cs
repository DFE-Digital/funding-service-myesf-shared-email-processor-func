using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pds.Admin.Api.Client.Models;
using Pds.Admin.Api.Client.Registration;
using Pds.Core.AzureStorage;
using Pds.Core.AzureStorage.Interfaces;
using Pds.Core.AzureStorage.Models;
using Pds.Core.AzureStorage.Services;
using Pds.Core.Caching;
using Pds.Core.Caching.Models;
using Pds.Core.Logging;
using Pds.Core.Telemetry.ApplicationInsights;
using Pds.Shared.EmailProcessor.Func.Config;
using Pds.Shared.EmailProcessor.Func.DependencyInjection;
using Pds.Shared.EmailProcessor.Services.DependencyInjection;

var builder = FunctionsApplication.CreateBuilder(args);

try
{
    builder.ConfigureFunctionsWebApplication();

    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

    builder.Configuration.AddEnvironmentVariables();

    builder.Services
        .AddFunctionServices()
        .AddFeatureServices()
        .AddLoggerAdapter()
        .AddAutomapperConfiguration()
        .AddRedisAndMemoryCache(options => builder.Configuration.Bind(nameof(RedisConfiguration), options))
        .AddNotifyApiClient(options => builder.Configuration.Bind(nameof(AdminApiClientConfiguration), options))
        .AddPdsApplicationInsightsTelemetry(options =>
        {
            builder.Configuration.Bind(nameof(PdsApplicationInsightsConfiguration), options);
            options.Component = typeof(Program).Assembly.GetName().Name;
        })
        .AddAzureStorage(options => builder.Configuration.Bind(nameof(AzureStorageConfiguration), options), options => builder.Configuration.Bind(nameof(AzureTableStorageConfiguration), options), null)
                    .AddTransient(
                        typeof(IAzureTableStorageRepository<>),
                        typeof(AzureTableStorageRepository<>))
        .AddAzureClients(options =>
        {
            options.AddServiceBusClient(builder.Configuration.GetValue<string>("ServiceBusConnection"));
        });
}
catch (Exception ex)
{
    Console.WriteLine($"Startup exception: {ex.Message}");
    throw;
}

builder.Build().Run();