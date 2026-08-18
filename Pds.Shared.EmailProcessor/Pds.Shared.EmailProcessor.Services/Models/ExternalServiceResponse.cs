namespace Pds.Shared.EmailProcessor.Services.Models
{
    /// <summary>
    /// The External service response.
    /// </summary>
    public class ExternalServiceResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ExternalServiceResponse"/> is success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }
    }
}