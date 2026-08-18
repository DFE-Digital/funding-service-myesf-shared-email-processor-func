using Pds.Shared.EmailProcessor.Services.Models;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Interfaces
{
    /// <summary>
    /// The email Template service.
    /// </summary>
    public interface IEmailTemplateService
    {
        /// <summary>
        /// Gets the email template details.
        /// </summary>
        /// <param name="emailTemplateRequest">The email template request.</param>
        /// <returns>The email template response.</returns>
        Task<EmailTemplateResponse> GetEmailTemplateDetailsAsync(EmailTemplateRequest emailTemplateRequest);
    }
}