using System.Collections.Generic;

namespace Pds.Shared.EmailProcessor.Services.Models
{
    /// <summary>
    /// The GovUkNotifyPersonalisation class.
    /// </summary>
    public class GovUkNotifyPersonalisation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GovUkNotifyPersonalisation"/> class.
        /// </summary>
        public GovUkNotifyPersonalisation()
        {
            Personalisation = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the personalisation.
        /// </summary>
        /// <value>
        /// The personalisation.
        /// </value>
        public Dictionary<string, object> Personalisation { get; set; }
    }
}