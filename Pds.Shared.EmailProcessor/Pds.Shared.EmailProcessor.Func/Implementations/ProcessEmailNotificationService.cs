using AutoMapper;
using Newtonsoft.Json;
using Pds.Core.Caching.Interfaces;
using Pds.Core.Logging;
using Pds.Core.Notification.Models;
using Pds.Shared.EmailProcessor.Func.Exceptions;
using Pds.Shared.EmailProcessor.Func.Helpers;
using Pds.Shared.EmailProcessor.Func.Interfaces;
using Pds.Shared.EmailProcessor.Services.Interfaces;
using Pds.Shared.EmailProcessor.Services.Models;

namespace Pds.Shared.EmailProcessor.Func.Implementations
{
    /// <summary>
    /// The Process Email Notification Message Service class.
    /// </summary>
    /// <seealso cref="IProcessEmailNotificationService" />
    public class ProcessEmailNotificationService : IProcessEmailNotificationService
    {
        private readonly ISendNotificationService<EmailNotification> _sendEmailNotificationService;
        private readonly IMapper _mapper;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILoggerAdapter<ProcessEmailNotificationService> _logger;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEmailNotificationService"/> class.
        /// </summary>
        /// <param name="sendEmailNotificationService">The send email notification service.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="emailTemplateService">The email template service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cacheService">The cache service.</param>
        public ProcessEmailNotificationService(
            ISendNotificationService<EmailNotification> sendEmailNotificationService,
            IMapper mapper,
            IEmailTemplateService emailTemplateService,
            ILoggerAdapter<ProcessEmailNotificationService> logger,
            ICacheService cacheService)
        {
            _sendEmailNotificationService = sendEmailNotificationService;
            _mapper = mapper;
            _emailTemplateService = emailTemplateService;
            _logger = logger;
            _cacheService = cacheService;
        }

        /// <inheritdoc />
        public async Task<SendNotificationResponse> SendEmailNotification(
            NotificationMessage notificationMessage,
            EmailTemplateResponse emailTemplateResponse,
            NotificationAuditEntry auditEntry)
        {
            SendNotificationResponse result = null;
            try
            {
                result =
                    await GetSendNotificationResponse(notificationMessage, emailTemplateResponse.EmailTemplateId, emailTemplateResponse.NotifyApiKeySecretName);

                if (result.EmailErrorMessages.Any())
                {
                    auditEntry.NotificationErrors.Add($"{nameof(SendNotificationResponse)}-{nameof(result.EmailErrorMessages)}", JsonConvert.SerializeObject(result.EmailErrorMessages));
                }
            }
            catch (Exception exception)
            {
                _logger?.LogError(
                    exception,
                    $"Failed to send GovUKNotify notification {JsonConvert.SerializeObject(notificationMessage)} with exception");
            }

            if (result?.RateLimitException == true)
            {
                // This is set 1t 3000 messages per minute by Notify. Only case it will normally happen is if the Notify Function App was offline for some reason,
                // and there has been a build up of messages on the SB queue in excess of 3k.
                // Normal case is messages are processed as they arrive on the queue, with small surges depending on upstream applications.
                // Depending on number of instances running the rate limit can be hit so this applied across any instance call at play.
                //https://docs.notifications.service.gov.uk/net.html#rate-limits
                _logger?.LogError(
                    "Request has been throttled, so back off for a minute before retrying");

                await Task.Delay(TimeSpan.FromMinutes(1));

                if (!result.PartialSuccess)
                {
                    throw new ThrottleException("Request throttled");
                }
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<EmailTemplateResponse> ProcessEmailTemplateResponse(NotificationMessage notificationMessage, NotificationAuditEntry auditEntry)
        {
            var emailTemplateRequest = _mapper.Map<EmailTemplateRequest>(notificationMessage);

            var cacheKey =
                    $"{nameof(EmailTemplateResponse)}-{notificationMessage.EmailMessageType}-{notificationMessage.RequestingService}";

            var emailTemplateResponse = await _cacheService.Get(cacheKey, () => _emailTemplateService.GetEmailTemplateDetailsAsync(emailTemplateRequest));

            NotificationMessageHelper.AddEmailTemplateAuditEntries(auditEntry, emailTemplateResponse);

            return emailTemplateResponse;
        }

        private async Task<SendNotificationResponse> GetSendNotificationResponse(
            NotificationMessage notificationMessage,
            string emailTemplateId,
            string notifyApiKeySecretName)
        {
            var result = new SendNotificationResponse();
            var emailNotification = _mapper.Map<EmailNotification>(notificationMessage);
            emailNotification.TemplateId = emailTemplateId;
            emailNotification.NotifyApiKeySecretName = notifyApiKeySecretName;

            var emailSendTasks = new List<Task<SendNotificationResponse>>();

            foreach (var emailAddress in notificationMessage.EmailAddresses.Distinct())
            {
                emailNotification.EmailAddress = emailAddress;
                emailSendTasks.Add(_sendEmailNotificationService.SendNotificationAsync(emailNotification));
            }

            var emailSendResponses = await Task.WhenAll(emailSendTasks);

            result.Success = emailSendResponses.All(emailSendResponse => emailSendResponse.Success);
            result.RateLimitException = emailSendResponses.Any(emailSendResponse => emailSendResponse.RateLimitException);

            result.EmailErrorMessages = emailSendResponses
                .Where(emailResponse => !string.IsNullOrWhiteSpace(emailResponse.ErrorMessage))
                .ToDictionary(emailResponse => emailResponse.EmailAddress, emailResponse => emailResponse.ErrorMessage);

            result.PartialSuccess = !result.Success ? emailSendResponses.Any(emailSendResponse => emailSendResponse.Success) : false;
            return result;
        }
    }
}