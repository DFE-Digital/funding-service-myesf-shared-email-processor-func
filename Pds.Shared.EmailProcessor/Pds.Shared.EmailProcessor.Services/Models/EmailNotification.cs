namespace Pds.Shared.EmailProcessor.Services.Models
{
    /// <summary>
    /// The Email notification.
    /// </summary>
    public class EmailNotification
    {
        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        public string TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the NotifyApiKey Secret Name.
        /// </summary>
        /// <value>
        /// The notify ApiKey secret name.
        /// </value>
        public string NotifyApiKeySecretName { get; set; }

        /// <summary>
        /// Gets or sets the email personalisation.
        /// </summary>
        /// <value>
        /// The email personalisation.
        /// </value>
        public GovUkNotifyPersonalisation EmailPersonalisation { get; set; }
    }
}