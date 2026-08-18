using Azure.Security.KeyVault.Secrets;
using Pds.Core.Logging;
using Pds.Shared.EmailProcessor.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Implementations
{
    /// <summary>
    /// Key vault service for retreiving secrets.
    /// </summary>
    public class KeyVaultService : IKeyVaultService
    {
        private readonly SecretClient _secretClient;
        private readonly ILoggerAdapter<KeyVaultService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultService"/> class.
        /// </summary>
        /// <param name="secretClient">SecretClient.</param>
        /// <param name="logger">Logger.</param>
        public KeyVaultService(SecretClient secretClient, ILoggerAdapter<KeyVaultService> logger)
        {
            _logger = logger;
            _secretClient = secretClient;
        }

        /// <summary>
        /// Retrieves the secret value for the provided secret name.
        /// </summary>
        /// <param name="secretName">Key vault secret name.</param>
        /// <returns>secret value.</returns>
        public async Task<string> GetSecretValue(string secretName)
        {
            _logger.LogInformation("Received secret name: " + secretName);

            if (string.IsNullOrWhiteSpace(secretName))
            {
                throw new ArgumentException("Secret name cannot be null or empty");
            }

            try
            {
                KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);

                if (string.IsNullOrWhiteSpace(secret?.Value))
                {
                    throw new Exception("Null secret value retrieved");
                }

                return secret?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
