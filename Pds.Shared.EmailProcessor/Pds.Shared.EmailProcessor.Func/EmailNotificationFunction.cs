using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pds.Core.Logging;
using Pds.Core.Notification.Models;
using Pds.Shared.EmailProcessor.Func.Interfaces;
using Pds.Shared.EmailProcessor.Services.Config;
using Pds.Shared.EmailProcessor.Services.Interfaces;
using Pds.Shared.EmailProcessor.Services.Models;

namespace Pds.Shared.EmailProcessor.Func
{
    /// <summary>
    /// Email Notification ServiceBus queue triggered Azure Function.
    /// </summary>
    public class EmailNotificationFunction
    {
        private readonly IProcessEmailNotificationService _processEmailNotificationMessageService;
        private readonly ILoggerAdapter<EmailNotificationFunction> _logger;
        private readonly IStorageAuditService<NotificationAuditEntry> _auditService;
        private readonly ServiceConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailNotificationFunction"/> class.
        /// </summary>
        /// <param name="processEmailNotificationMessage">The send email notification service.</param>
        /// <param name="serviceConfigurationOptions">Application config.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="auditService">the audit service.</param>
        public EmailNotificationFunction(
            IProcessEmailNotificationService processEmailNotificationMessage,
            IOptions<ServiceConfiguration> serviceConfigurationOptions,
            ILoggerAdapter<EmailNotificationFunction> logger,
            IStorageAuditService<NotificationAuditEntry> auditService)
        {
            _processEmailNotificationMessageService = processEmailNotificationMessage;
            _configuration = serviceConfigurationOptions.Value;
            _logger = logger;
            _auditService = auditService;
        }


        /// <summary>
        /// Entry point to the Azure Function.
        /// </summary>
        /// <param name="notificationMessage">The queue item that triggered this function to run.</param>
        /// <returns>Async Task.</returns>
        [Function("EmailNotificationFunction")]
        public async Task Run(
            [ServiceBusTrigger("%QueueName%", Connection = "ServiceBusConnection")]
            NotificationMessage notificationMessage)
        {
            var result = new SendNotificationResponse();
            var emailTemplateResponse = new EmailTemplateResponse();

            var auditEntry = new NotificationAuditEntry(nameof(NotificationMessage))
            {
                NotificationMessage = JsonConvert.SerializeObject(notificationMessage)
            };

            if (notificationMessage.EmailAddresses == null || !notificationMessage.EmailAddresses.Any())
            {
                auditEntry.NotificationErrors.Add(
                    nameof(notificationMessage.EmailAddresses),
                    $"Missing {nameof(notificationMessage.EmailAddresses)}");
            }

            if (!auditEntry.NotificationErrors.Any())
            {
                _logger.LogInformation("Processing Email Template Response with EmailMessageType: " + notificationMessage.EmailMessageType + ", RequestingService: " + notificationMessage.RequestingService);

                emailTemplateResponse = await _processEmailNotificationMessageService.ProcessEmailTemplateResponse(notificationMessage, auditEntry);

                _logger.LogInformation("Received EmailTemplateId: " + emailTemplateResponse?.EmailTemplateId + ", NotifyApiKeySecretName: " + emailTemplateResponse?.NotifyApiKeySecretName);
            }

            if (!auditEntry.NotificationErrors.Any())
            {
                result = await _processEmailNotificationMessageService.SendEmailNotification(notificationMessage, emailTemplateResponse, auditEntry);
            }

            if (auditEntry.NotificationErrors.Any())
            {
                // Add to Audit
                auditEntry.SendNotificationResponse = result;
                auditEntry.SendNotificationResponseValue = JsonConvert.SerializeObject(result);
                auditEntry.NotificationErrorsValue = JsonConvert.SerializeObject(auditEntry.NotificationErrors);
                _logger?.LogTrace(
                    $"Encountered errors whilst sending GovUKNotify notification: {JsonConvert.SerializeObject(notificationMessage)} with notification errors {JsonConvert.SerializeObject(auditEntry)}");
                await _auditService.AddEntryAsync(auditEntry);
            }

            _logger?.LogInformation(
                $"EmailNotificationFunction ServiceBus queue trigger function processed message: {JsonConvert.SerializeObject(notificationMessage)} with result {JsonConvert.SerializeObject(result)}");
        }
    }
}