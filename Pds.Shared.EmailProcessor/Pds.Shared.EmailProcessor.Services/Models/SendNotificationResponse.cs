using System.Collections.Generic;

namespace Pds.Shared.EmailProcessor.Services.Models
{
    /// <summary>
    /// The Send Notification response.
    /// </summary>
    public class SendNotificationResponse : ExternalServiceResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this response has a rate limit exception.
        /// </summary>
        public bool RateLimitException { get; set; }

        /// <summary>
        /// Gets or sets the email error messages.
        /// </summary>
        public IDictionary<string, string> EmailErrorMessages { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it has been a partial success.
        /// If true we cannot reprocess this message as it would result in some email being sent multiple times.
        /// </summary>
        public bool PartialSuccess { get; set; }
    }
}