using Notify.Client;
using Notify.Models.Responses;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Pds.Shared.EmailProcessor.Services.Base
{
    /// <summary>
    /// The GovUkNotifyClientProxy class.
    /// </summary>
    /// <seealso cref="IGovUkNotifyClientProxy" />
    [ExcludeFromCodeCoverage]
    public class GovUkNotifyClientProxy : IGovUkNotifyClientProxy
    {
        /// <inheritdoc />
        public EmailNotificationResponse SendEmail(string apiKey, string emailAddress, string templateId, Dictionary<string, dynamic> notifyUkDynamicObject)
        {
            var client = new NotificationClient(apiKey);
            return client.SendEmail(emailAddress, templateId, notifyUkDynamicObject);
        }
    }
}