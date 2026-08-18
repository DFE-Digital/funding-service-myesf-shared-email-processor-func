namespace Pds.Shared.EmailProcessor.Services.Config
{
    /// <summary>
    /// A class used to represent configuration settings.
    /// </summary>
    public class ServiceConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the service bus connection string.
        /// </summary>
        /// <value>
        /// The service bus connection string.
        /// </value>
        public string ServiceBusConnection { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of API retries.
        /// </summary>
        /// <value>
        /// The maximum number of API retries.
        /// </value>
        public int MaxNumberOfApiRetries { get; set; } = 5;

        /// <summary>
        /// Gets or sets the Azure Key Vault URI.
        /// </summary>
        /// <value>
        /// The Azure Key Vault URI.
        /// </value>
        public string AzureKeyVaultURI { get; set; }

        /// <summary>
        /// Gets or sets the cache encryption secret key.
        /// </summary>
        /// <value>
        /// The cache encryption secret key.
        /// </value>
        public string CacheEncryptionSecretKey { get; set; }
    }
}