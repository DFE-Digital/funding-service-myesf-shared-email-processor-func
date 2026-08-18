using Microsoft.Extensions.Options;
using Pds.Admin.Api.Client.Interfaces;
using Pds.Core.Utils.Interfaces;
using Pds.Shared.EmailProcessor.Services.Config;
using Pds.Shared.EmailProcessor.Services.Interfaces;
using Pds.Shared.EmailProcessor.Services.Models;
using System;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Implementations
{
    /// <summary>
    /// The email template service.
    /// </summary>
    /// <seealso cref="IEmailTemplateService" />
    public class EmailTemplateService : IEmailTemplateService
    {
        /// <summary>
        /// The admin API client.
        /// </summary>
        private readonly INotifyApiClient _notifyApiClient;

        /// <summary>
        /// The retry mechanism.
        /// </summary>
        private readonly IRetryMechanism _retryMechanism;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly ServiceConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateService"/> class.
        /// </summary>
        /// <param name="notifyApiClient">The admin API client.</param>
        /// <param name="retryMechanism">The retry mechanism.</param>
        /// <param name="serviceConfigurationOptions">The service configuration options.</param>
        public EmailTemplateService(
            INotifyApiClient notifyApiClient,
            IRetryMechanism retryMechanism,
            IOptions<ServiceConfiguration> serviceConfigurationOptions)
        {
            _notifyApiClient = notifyApiClient;
            _retryMechanism = retryMechanism;
            _configuration = serviceConfigurationOptions.Value;
        }

        /// <inheritdoc/>
        public async Task<EmailTemplateResponse> GetEmailTemplateDetailsAsync(EmailTemplateRequest emailTemplateRequest)
        {
            var response = new EmailTemplateResponse();

            try
            {
                var details = await _retryMechanism.TryUntilSuccessOrThrow(
                    () => _notifyApiClient.GetTemplateDetails(
                        emailTemplateRequest.RequestingService,
                        emailTemplateRequest.EmailMessageType,
                        emailTemplateRequest.MetaData),
                    _configuration.MaxNumberOfApiRetries);

                if (!string.IsNullOrWhiteSpace(details?.TemplateId))
                {
                    response.EmailTemplateId = details.TemplateId;
                    response.NotifyApiKeySecretName = details.NotifyApiKeySecretName;
                    response.Success = true;
                }
            }
            catch (Exception exception)
            {
                response.ErrorMessage = exception.Message;
            }

            return response;
        }
    }
}