using Pds.Shared.EmailProcessor.Services.Models;

namespace Pds.Shared.EmailProcessor.Func.Helpers
{
    /// <summary>
    /// The Notification Message helper class.
    /// </summary>
    public static class NotificationMessageHelper
    {
        /// <summary>
        /// Adds the email template audit entries.
        /// </summary>
        /// <param name="auditEntry">The audit entry.</param>
        /// <param name="emailTemplateResponse">The email template response.</param>
        public static void AddEmailTemplateAuditEntries(
            NotificationAuditEntry auditEntry,
            EmailTemplateResponse emailTemplateResponse)
        {
            if (!emailTemplateResponse.Success)
            {
                if (string.IsNullOrWhiteSpace(emailTemplateResponse.EmailTemplateId))
                {
                    auditEntry.NotificationErrors.Add(
                        $"{nameof(EmailTemplateResponse)}-{nameof(EmailTemplateResponse.EmailTemplateId)}",
                        $"Missing {nameof(EmailTemplateResponse.EmailTemplateId)}");
                }

                if (string.IsNullOrWhiteSpace(emailTemplateResponse.NotifyApiKeySecretName))
                {
                    auditEntry.NotificationErrors.Add(
                        $"{nameof(EmailTemplateResponse)}-{nameof(EmailTemplateResponse.NotifyApiKeySecretName)}",
                        $"Missing {nameof(EmailTemplateResponse.NotifyApiKeySecretName)}");
                }

                if (!string.IsNullOrWhiteSpace(emailTemplateResponse.ErrorMessage))
                {
                    auditEntry.NotificationErrors.Add(
                        $"{nameof(EmailTemplateResponse)}-{nameof(EmailTemplateResponse.ErrorMessage)}",
                        emailTemplateResponse.ErrorMessage);
                }
            }
        }
    }
}