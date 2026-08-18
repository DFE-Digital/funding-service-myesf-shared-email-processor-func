using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pds.Shared.EmailProcessor.Services.Base;
using Pds.Shared.EmailProcessor.Services.Config;
using Pds.Shared.EmailProcessor.Services.Implementations;
using Pds.Shared.EmailProcessor.Services.Interfaces;
using Pds.Shared.EmailProcessor.Services.Models;

namespace Pds.Shared.EmailProcessor.Services.DependencyInjection
{
    /// <summary>
    /// Extensions class for <see cref="IServiceCollection"/> for registering the feature's services.
    /// </summary>
    public static class FeatureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for the current feature to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the feature's services to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFeatureServices(this IServiceCollection services)
        {
            services
                .AddSingleton<ISendNotificationService<EmailNotification>, GovUkNotifyEmailService>()
                .AddSingleton<IGovUkNotifyClientProxy, GovUkNotifyClientProxy>()
                .AddSingleton<IEmailTemplateService, EmailTemplateService>()
                .AddSingleton(secretClient => new SecretClient(
                    new System.Uri(ConfigHelper.GetIConfigurationRoot().Get<ServiceConfiguration>().AzureKeyVaultURI),
                    new DefaultAzureCredential()))
                .AddSingleton<IKeyVaultService, KeyVaultService>()
                .AddSingleton<IEncryptionService, EncryptionService>()
                .AddTransient<IStorageAuditService<NotificationAuditEntry>, AzureTableStorageDbAuditService<NotificationAuditEntry>>()
                .AddOptions<ServiceConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.Bind(settings);
                });

            return services;
        }
    }
}