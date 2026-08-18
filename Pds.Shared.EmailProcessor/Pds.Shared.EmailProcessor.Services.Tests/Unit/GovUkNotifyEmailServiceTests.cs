using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Notify.Exceptions;
using Pds.Core.Caching.Interfaces;
using Pds.Core.Caching.Models;
using Pds.Core.Logging;
using Pds.Shared.EmailProcessor.Services.Base;
using Pds.Shared.EmailProcessor.Services.Config;
using Pds.Shared.EmailProcessor.Services.Implementations;
using Pds.Shared.EmailProcessor.Services.Interfaces;
using Pds.Shared.EmailProcessor.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class GovUkNotifyEmailServiceTests : UnitTestSetupBase
    {
        private readonly Mock<ILoggerAdapter<GovUkNotifyEmailService>> _mockLogger =
            new Mock<ILoggerAdapter<GovUkNotifyEmailService>>(MockBehavior.Strict);

        private readonly Mock<IGovUkNotifyClientProxy> _mockNotifyProxy =
            new Mock<IGovUkNotifyClientProxy>(MockBehavior.Strict);

        private readonly Mock<IKeyVaultService> _mockKeyVaultService =
            new Mock<IKeyVaultService>(MockBehavior.Strict);

        private readonly Mock<IEncryptionService> _mockEncryptionService =
            new Mock<IEncryptionService>(MockBehavior.Strict);

        private readonly Mock<ICacheService> _mockCacheService =
            new Mock<ICacheService>(MockBehavior.Strict);

        private readonly Mock<IOptions<ServiceConfiguration>> _mockConfigurationOptions =
            new Mock<IOptions<ServiceConfiguration>>(MockBehavior.Strict);

        [TestMethod]
        [DataRow(nameof(EmailNotification.EmailPersonalisation), nameof(EmailNotification.EmailPersonalisation), nameof(EmailNotification.EmailPersonalisation))]
        [DataRow(nameof(EmailNotification.EmailPersonalisation), null, Constants.ServicesConstants.UnknownValue)]
        public void ConvertTest(string key, string sourceValue, string expectedValue)
        {
            // Arrange
            _mockLogger.Setup(logger => logger.LogInformation(It.IsAny<string>()));

            var input = new GovUkNotifyPersonalisation
            {
                Personalisation = new Dictionary<string, object>
                {
                    { key, sourceValue }
                }
            };

            var expectation = new Dictionary<string, dynamic>
            {
                { key, expectedValue }
            };

            // Act
            var govUkNotifyService = GetGovUkNotifyEmailService();
            var result = govUkNotifyService.Convert(input);

            // Assert
            result.Should().BeEquivalentTo(expectation);
        }

        [TestMethod]
        [DynamicData(
            nameof(SendNotificationInput),
            typeof(UnitTestSetupBase),
            DynamicDataSourceType.Method)]
        public async Task SendNotificationAsyncAsyncTests(
            string responseId,
            bool throwException,
            bool isRateLimitException,
            SendNotificationResponse expectation)
        {
            var citizenEmailNotification = new EmailNotification
            {
                EmailAddress = "dumy@email.com",
                NotifyApiKeySecretName = "MockService"
            };
            var emailResponse = responseId == null ? null : new Notify.Models.Responses.EmailNotificationResponse
            {
                id = responseId,
            };
            expectation.EmailAddress = citizenEmailNotification.EmailAddress;

            if (throwException)
            {
                _mockNotifyProxy.Setup(proxy => proxy.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>())).Throws(new NotifyClientException(isRateLimitException ? Constants.ServicesConstants.GovUkNotifyRateLimitException : nameof(Exception).ToLowerInvariant()));
            }
            else
            {
                _mockNotifyProxy.Setup(proxy => proxy.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>())).Returns(emailResponse);
            }

            _mockKeyVaultService.Setup(keyVault => keyVault.GetSecretValue(It.IsAny<string>())).Returns(Task.FromResult("TestSecretValue"));
            _mockCacheService.Setup(cache => cache.Get(It.IsAny<string>(), It.IsAny<Func<Task<string>>>(), It.IsAny<CacheOptions>())).ReturnsAsync("testencryptedsecret");
            _mockEncryptionService.Setup(encryption => encryption.EncryptStringToHex(It.IsAny<string>(), It.IsAny<string>()));
            _mockEncryptionService.Setup(encryption => encryption.DecryptStringFromHex(It.IsAny<string>(), It.IsAny<string>())).Returns("TestSecretValue");
            _mockLogger.Setup(logger => logger.LogInformation(It.IsAny<string>()));

            //Act
            var govUkNotifyService = GetGovUkNotifyEmailService();
            var result = await govUkNotifyService.SendNotificationAsync(citizenEmailNotification);

            result.Should().BeEquivalentTo(expectation);

            _mockNotifyProxy.Verify(proxy => proxy.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>()), Times.Once);
            if (throwException)
            {
                _mockLogger.Verify(mock => mock.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
            }
        }

        private GovUkNotifyEmailService GetGovUkNotifyEmailService()
        {
            _mockLogger.Setup(logger => logger.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            _mockConfigurationOptions.Setup(x => x.Value)
                .Returns(new ServiceConfiguration());

            var govUkNotifyService =
                new GovUkNotifyEmailService(_mockLogger.Object, _mockNotifyProxy.Object, _mockCacheService.Object, _mockKeyVaultService.Object, _mockEncryptionService.Object, _mockConfigurationOptions.Object);
            return govUkNotifyService;
        }
    }
}