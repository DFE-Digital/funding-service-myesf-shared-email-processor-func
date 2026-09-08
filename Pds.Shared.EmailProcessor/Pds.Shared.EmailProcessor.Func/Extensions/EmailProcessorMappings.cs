using EmailNotification = Pds.Shared.EmailProcessor.Services.Models.EmailNotification;
using EmailTemplateRequest = Pds.Shared.EmailProcessor.Services.Models.EmailTemplateRequest;
using GovUkNotifyPersonalisationCore = Pds.Core.Notification.Models.GovUkNotifyPersonalisation;
using NotificationMessage = Pds.Core.Notification.Models.NotificationMessage;

namespace Pds.Shared.EmailProcessor.Func.Extensions
{
    /// <summary>
    /// The mapping meythods.
    /// </summary>
    /// <seealso cref="Profile" />
    public static class EmailProcessorMappings
    {
        /// <summary>
        /// Mapping method.
        /// </summary>
        /// <param name="notification">The configuration.</param>
        /// <returns>
        /// A mapping between audit EmailNotification and NotificationMessage.
        /// </returns>
        public static EmailNotification ToEmailNotifiction(this NotificationMessage notification)
        {
            return new EmailNotification
            {
                EmailPersonalisation = notification.EmailPersonalisation.ToGovUkNotifyPersonalisationCore()
            };
        }

        /// <summary>
        /// Mapping method.
        /// </summary>
        /// <param name="notification">The configuration.</param>
        /// <returns>
        /// A mapping between EmailTemplateRequest and NotificationMessage.
        /// </returns>
        public static EmailTemplateRequest ToEmailTemplateRequest(this NotificationMessage notification)
        {
            return new EmailTemplateRequest
            {
                MetaData = notification.MetaData,
                EmailMessageType = notification.EmailMessageType,
                RequestingService = notification.RequestingService
            };
        }

        /// <summary>
        /// Mapping method.
        /// </summary>
        /// <param name="personalisation">The configuration.</param>
        /// <returns>
        /// A mapping between Services.Models.GovUkNotifyPersonalisation and Pds.Core.Notification.Models.GovUkNotifyPersonalisation.
        /// </returns>
        public static Services.Models.GovUkNotifyPersonalisation ToGovUkNotifyPersonalisationCore(this GovUkNotifyPersonalisationCore personalisation)
        {
            return new Services.Models.GovUkNotifyPersonalisation
            {
                Personalisation = new Dictionary<string, object>(personalisation.Personalisation)
            };
        }
    }
}