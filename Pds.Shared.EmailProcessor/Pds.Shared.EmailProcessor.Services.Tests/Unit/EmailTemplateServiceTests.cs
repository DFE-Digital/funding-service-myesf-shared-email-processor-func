using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Admin.Api.Client.Interfaces;
using Pds.Admin.Api.Client.Models;
using Pds.Core.Logging;
using Pds.Core.Utils.Implementations;
using Pds.Shared.EmailProcessor.Services.Config;
using Pds.Shared.EmailProcessor.Services.Implementations;
using Pds.Shared.EmailProcessor.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class EmailTemplateServiceTests : UnitTestSetupBase
    {
        private readonly Mock<INotifyApiClient> _mockAdminApiClient = new Mock<INotifyApiClient>(MockBehavior.Strict);

        private readonly Mock<ILoggerAdapter<RetryMechanism>> _mockLogger =
            new Mock<ILoggerAdapter<RetryMechanism>>(MockBehavior.Strict);

        private readonly Mock<IOptions<ServiceConfiguration>> _mockServiceConfiguration =
            new Mock<IOptions<ServiceConfiguration>>(MockBehavior.Strict);

        [TestMethod]
        [DynamicData(
            nameof(GetEmailTemplateDetailsAsyncInput),
            typeof(UnitTestSetupBase),
            DynamicDataSourceType.Method)]
        public async Task GetEmailTemplateDetailsAsyncTests(
            EmailTemplateRequest emailTemplateRequest,
            NotifyTemplateDetails apiResponse,
            EmailTemplateResponse emailTemplateResponse,
            bool throwException = false)
        {
            //Arrange
            if (throwException)
            {
                _mockAdminApiClient.Setup(apiClient => apiClient.GetTemplateDetails(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                    .ThrowsAsync(new Exception(emailTemplateResponse.ErrorMessage));
            }
            else
            {
                _mockAdminApiClient.Setup(apiClient => apiClient.GetTemplateDetails(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                    .ReturnsAsync(apiResponse);
            }

            //Act
            var emailTemplateService = GetEmailTemplateService();
            var result = await emailTemplateService.GetEmailTemplateDetailsAsync(emailTemplateRequest);

            result.Should().BeEquivalentTo(emailTemplateResponse);

            if (throwException)
            {
                _mockAdminApiClient.Verify(
                    apiClient => apiClient.GetTemplateDetails(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Exactly(5));
            }
            else
            {
                _mockAdminApiClient.Verify(
                    apiClient => apiClient.GetTemplateDetails(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Once);
            }
        }

        private EmailTemplateService GetEmailTemplateService()
        {
            _mockServiceConfiguration.Setup(x => x.Value)
                .Returns(new ServiceConfiguration());

            _mockLogger.Setup(logger => logger.LogInformation(It.IsAny<Exception>(), It.IsAny<string>()));
            _mockLogger.Setup(logger => logger.LogWarning(It.IsAny<Exception>(), It.IsAny<string>()));
            _mockLogger.Setup(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()));

            var govUkNotifyService =
                new EmailTemplateService(
                    _mockAdminApiClient.Object,
                    GetRetryMechanism(),
                    _mockServiceConfiguration.Object);

            return govUkNotifyService;
        }

        private RetryMechanism GetRetryMechanism()
        {
            return new RetryMechanism(_mockLogger.Object);
        }
    }
}