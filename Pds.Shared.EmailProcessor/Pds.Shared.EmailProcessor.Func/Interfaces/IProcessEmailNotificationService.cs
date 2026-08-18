using Pds.Core.Notification.Models;
using Pds.Shared.EmailProcessor.Services.Models;

namespace Pds.Shared.EmailProcessor.Func.Interfaces
{
    /// <summary>
    /// The Process Email Notification service class.
    /// </summary>
    public interface IProcessEmailNotificationService
    {
        /// <summary>
        /// Sends the email notification.
        /// </summary>
        /// <param name="notificationMessage">The notification message.</param>
        /// <param name="emailTemplateResponse">The email template response.</param>
        /// <param name="auditEntry">The audit entry.</param>
        /// <returns>Send Notification Response.</returns>
        Task<SendNotificationResponse> SendEmailNotification(
            NotificationMessage notificationMessage,
            EmailTemplateResponse emailTemplateResponse,
            NotificationAuditEntry auditEntry);

        /// <summary>
        /// Processes the email template response.
        /// </summary>
        /// <param name="notificationMessage">The notification message.</param>
        /// <param name="auditEntry">The audit entry.</param>
        /// <returns>Email Template Response.</returns>
        Task<EmailTemplateResponse> ProcessEmailTemplateResponse(
            NotificationMessage notificationMessage,
            NotificationAuditEntry auditEntry);
    }
}