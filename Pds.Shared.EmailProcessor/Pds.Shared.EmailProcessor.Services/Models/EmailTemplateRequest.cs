using System.Collections.Generic;

namespace Pds.Shared.EmailProcessor.Services.Models
{
    /// <summary>
    /// The email request class.
    /// </summary>
    public class EmailTemplateRequest
    {
        /// <summary>
        /// Gets or sets the meta data.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> MetaData { get; set; } = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Gets or sets the email message type e.g chaseEmail, UpdatedAllocationPublished, NewAllocationPublished.
        /// </summary>
        public string EmailMessageType { get; set; }

        /// <summary>
        /// Gets or sets the requesting service e.g VYF, Docex.
        /// </summary>
        public string RequestingService { get; set; }
    }
}