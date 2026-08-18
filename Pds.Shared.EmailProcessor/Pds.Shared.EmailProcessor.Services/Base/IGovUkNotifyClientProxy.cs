using Notify.Models.Responses;
using System.Collections.Generic;

namespace Pds.Shared.EmailProcessor.Services.Base
{
    /// <summary>
    /// The GovUkNotifyClientProxy interface.
    /// This is only a proxy to GovNotify client as there are no interface we could hang our unit tests over.
    /// </summary>
    public interface IGovUkNotifyClientProxy
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="templateId">The template identifier.</param>
        /// <param name="notifyUkDynamicObject">The notify uk dynamic object.</param>
        /// <returns>The EmailNotificationResponse.</returns>
        EmailNotificationResponse SendEmail(string apiKey, string emailAddress, string templateId, Dictionary<string, dynamic> notifyUkDynamicObject);
    }
}