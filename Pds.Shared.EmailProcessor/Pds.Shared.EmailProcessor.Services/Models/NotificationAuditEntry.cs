using Pds.Core.AzureStorage.Models;
using System;
using System.Collections.Generic;

namespace Pds.Shared.EmailProcessor.Services.Models
{
    /// <summary>
    /// Audit Entry class.
    /// </summary>
    public class NotificationAuditEntry : PdsAzureTableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationAuditEntry"/> class.
        /// </summary>
        public NotificationAuditEntry()
            : base(nameof(NotificationMessage), Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationAuditEntry"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public NotificationAuditEntry(string type)
        : base(type, Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the notification message.
        /// </summary>
        /// <value>
        /// The notification message.
        /// </value>
        public string NotificationMessage { get; set; }

        /// <summary>
        /// Gets or sets the send notification response.
        /// </summary>
        /// <value>
        /// The send notification response.
        /// </value>
        public SendNotificationResponse SendNotificationResponse { get; set; }

        /// <summary>
        /// Gets or sets the send notification response value.
        /// </summary>
        /// <value>
        /// The send notification response value.
        /// </value>
        public string SendNotificationResponseValue { get; set; }

        /// <summary>
        /// Gets or sets the notification errors.
        /// </summary>
        /// <value>
        /// The notification errors.
        /// </value>
        public Dictionary<string, string> NotificationErrors { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the notification errors value.
        /// </summary>
        /// <value>
        /// The notification errors value.
        /// </value>
        public string NotificationErrorsValue { get; set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public override string TableName => nameof(NotificationAuditEntry);
    }
}