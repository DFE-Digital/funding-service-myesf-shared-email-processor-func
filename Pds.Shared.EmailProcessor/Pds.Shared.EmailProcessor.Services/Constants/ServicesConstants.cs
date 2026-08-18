namespace Pds.Shared.EmailProcessor.Services.Constants
{
    /// <summary>
    /// The service constants.
    /// </summary>
    public static class ServicesConstants
    {
        /// <summary>
        /// The unknown value.
        /// </summary>
        public const string UnknownValue = "Unknown";

        /// <summary>
        /// The gov uk notify rate limit exception.
        /// Status code 429 returned when the service requests are throttled.
        /// </summary>
        public const string GovUkNotifyRateLimitException = "429";

        /// <summary>
        /// The notification audit collection name.
        /// </summary>
        public const string NotificationAuditCollectionName = "notificationAudit";
    }
}
