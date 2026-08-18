namespace Pds.Shared.EmailProcessor.Services.Models
{
    /// <summary>
    /// The Email template response.
    /// </summary>
    public class EmailTemplateResponse : ExternalServiceResponse
    {
        /// <summary>
        /// Gets or sets the email template identifier.
        /// </summary>
        public string EmailTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the notify ApiKey secret name.
        /// </summary>
        public string NotifyApiKeySecretName { get; set; }
    }
}