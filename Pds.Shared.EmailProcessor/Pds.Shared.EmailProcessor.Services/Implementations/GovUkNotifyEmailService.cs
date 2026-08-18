using Microsoft.Extensions.Options;
using Notify.Exceptions;
using Pds.Core.Caching.Interfaces;
using Pds.Core.Logging;
using Pds.Shared.EmailProcessor.Services.Base;
using Pds.Shared.EmailProcessor.Services.Config;
using Pds.Shared.EmailProcessor.Services.Constants;
using Pds.Shared.EmailProcessor.Services.Interfaces;
using Pds.Shared.EmailProcessor.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Implementations
{
    /// <summary>
    /// The GovUkNotify Email Service class.
    /// </summary>
    /// <seealso cref="ISendNotificationService{EmailNotification}" />
    public class GovUkNotifyEmailService : ISendNotificationService<EmailNotification>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILoggerAdapter<GovUkNotifyEmailService> _logger;

        /// <summary>
        /// The client proxy.
        /// </summary>
        private readonly IGovUkNotifyClientProxy _clientProxy;

        /// <summary>
        /// The cache service.
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// The key vault service.
        /// </summary>
        private readonly IKeyVaultService _keyVaultService;

        /// <summary>
        /// The encryption service.
        /// </summary>
        private readonly IEncryptionService _encryptionService;

        /// <summary>
        /// The Cache Encryption Secret Key.
        /// </summary>
        private readonly string _cacheEncryptionSecretKey;

        /// <summary>
        /// The dictionary containing api keys fetched from key vault or cache.
        /// </summary>
        private readonly Dictionary<string, string> _apiKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="GovUkNotifyEmailService"/> class.
        /// </summary>
        /// <param name="applicationLogger">The application logger.</param>
        /// <param name="clientProxy">The client proxy.</param>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="keyVaultService">The key vault service.</param>
        /// <param name="encryptionService">The encryption service.</param>
        /// <param name="serviceConfigurationOptions">The app configuration options.</param>
        public GovUkNotifyEmailService(
            ILoggerAdapter<GovUkNotifyEmailService> applicationLogger,
            IGovUkNotifyClientProxy clientProxy,
            ICacheService cacheService,
            IKeyVaultService keyVaultService,
            IEncryptionService encryptionService,
            IOptions<ServiceConfiguration> serviceConfigurationOptions)
        {
            _logger = applicationLogger;
            _clientProxy = clientProxy;
            _cacheService = cacheService;
            _encryptionService = encryptionService;
            _cacheEncryptionSecretKey = serviceConfigurationOptions.Value.CacheEncryptionSecretKey;
            _keyVaultService = keyVaultService;
            _apiKeys = new Dictionary<string, string>();
        }

        /// <inheritdoc/>
        public async Task<SendNotificationResponse> SendNotificationAsync(EmailNotification notification)
        {
            var sendNotificationResponse = new SendNotificationResponse
            {
                EmailAddress = notification.EmailAddress
            };

            try
            {
                _logger.LogInformation($"Sending template id [{notification.TemplateId}; to email [{notification.EmailAddress}];");
                string apiKey = GetApiKey(notification.NotifyApiKeySecretName);

                var response = _clientProxy.SendEmail(
                apiKey,
                notification.EmailAddress,
                notification.TemplateId,
                Convert(notification.EmailPersonalisation));

                sendNotificationResponse.Success = !string.IsNullOrEmpty(response?.id);
            }
            catch (NotifyClientException ex)
            {
                if (ex.Message.ToLowerInvariant()
                    .Contains(ServicesConstants.GovUkNotifyRateLimitException))
                {
                    sendNotificationResponse.RateLimitException = true;
                }

                sendNotificationResponse.ErrorMessage = ex.Message;

                _logger.LogError("Failed to send email with GovUKNotify", ex);
            }

            return await Task.FromResult(sendNotificationResponse);
        }

        /// <summary>
        /// Converts the specified gov uk notify personalisation.
        /// </summary>
        /// <param name="govUkNotifyPersonalisation">The gov uk notify personalisation.</param>
        /// <returns>The personalisation dictionary.</returns>
        public Dictionary<string, dynamic> Convert(GovUkNotifyPersonalisation govUkNotifyPersonalisation)
        {
            if (govUkNotifyPersonalisation?.Personalisation != null)
            {
                try
                {
                    foreach (var (key, value) in govUkNotifyPersonalisation.Personalisation?.ToArray())
                    {
                        if (string.IsNullOrEmpty(value?.ToString()))
                        {
                            govUkNotifyPersonalisation.Personalisation[key] = ServicesConstants.UnknownValue;
                        }
                    }

                    return govUkNotifyPersonalisation?.Personalisation.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value is JsonElement je ? je.ToString() : keyValuePair.Value);
                }
                catch
                {
                    throw new Exception("Error converting personalisation details.");
                }
            }

            return default;
        }

        private string GetApiKey(string apiKeySecretName)
        {
            try
            {
                if (!_apiKeys.Keys.Contains(apiKeySecretName))
                {
                    _logger.LogInformation("Api key does not exist in dictionary. Fetching from cache.");

                    string encryptedApiKey = _cacheService.Get(apiKeySecretName, () => GetEncryptedSecretValue(apiKeySecretName)).Result;
                    string decryptedApiKey = GetDecryptedSecretValue(encryptedApiKey);
                    _apiKeys[apiKeySecretName] = decryptedApiKey;
                }
                else
                {
                    _logger.LogInformation("Api key exists in dictionary. Fetching from dictionary");
                }

                return _apiKeys[apiKeySecretName];
            }
            catch
            {
                throw new Exception("Error fetching api key.");
            }
        }

        private async Task<string> GetEncryptedSecretValue(string secretName)
        {
            _logger.LogInformation("Api key does not exist in cache. Fetching from key vault.");

            var secretValue = await _keyVaultService.GetSecretValue(secretName);

            try
            {
                return _encryptionService.EncryptStringToHex(_cacheEncryptionSecretKey, secretValue);
            }
            catch
            {
                throw new Exception("Error encrypting the NotifyApiKeySecretValue.");
            }
        }

        private string GetDecryptedSecretValue(string encryptedSecretValue)
        {
            try
            {
                _logger.LogInformation("Decrypting secret from cache");

                if (string.IsNullOrWhiteSpace(encryptedSecretValue))
                {
                    throw new Exception("secret value in cache is null or empty.");
                }

                return _encryptionService.DecryptStringFromHex(_cacheEncryptionSecretKey, encryptedSecretValue);
            }
            catch
            {
                throw new Exception("Error decrypting the NotifyApiKeySecretValue from cache.");
            }
        }
    }
}