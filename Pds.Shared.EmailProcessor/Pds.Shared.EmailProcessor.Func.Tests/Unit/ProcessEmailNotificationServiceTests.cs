using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Pds.Core.Caching.Interfaces;
using Pds.Core.Caching.Models;
using Pds.Core.Logging;
using Pds.Core.Notification.Models;
using Pds.Shared.EmailProcessor.Func.Config;
using Pds.Shared.EmailProcessor.Func.Exceptions;
using Pds.Shared.EmailProcessor.Func.Implementations;
using Pds.Shared.EmailProcessor.Services.Interfaces;
using Pds.Shared.EmailProcessor.Services.Models;
using System;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Func.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class ProcessEmailNotificationServiceTests : UnitTestSetupBase
    {
        private readonly Mock<IEmailTemplateService> _mockEmailTemplateService =
            new Mock<IEmailTemplateService>(MockBehavior.Strict);

        private readonly Mock<ISendNotificationService<EmailNotification>> _mockSendNotificationService =
            new Mock<ISendNotificationService<EmailNotification>>(MockBehavior.Strict);

        private readonly Mock<ICacheService> _mockCacheService =
            new Mock<ICacheService>(MockBehavior.Strict);

        private readonly Mock<ILoggerAdapter<ProcessEmailNotificationService>> _mockLogger =
            new Mock<ILoggerAdapter<ProcessEmailNotificationService>>(MockBehavior.Strict);

        private readonly IMapper _mapper = GetMapper();

        [TestMethod]
        [DynamicData(
            nameof(SendEmailNotificationInput),
            typeof(UnitTestSetupBase),
            DynamicDataSourceType.Method)]
        public async Task SendEmailNotification_ReturnsExpectedResult(
            NotificationMessage notificationMessage,
            EmailTemplateResponse emailTemplateResponse,
            int sendNotificationServiceInvocationCount,
            SendNotificationResponse sendNotificationApiResponse,
            SendNotificationResponse sendNotificationAggregateResponse)
        {
            // Arrange
            var processEmailNotificationService = GetProcessEmailNotificationService();

            _mockSendNotificationService
                .Setup(service => service.SendNotificationAsync(
                    It.IsAny<EmailNotification>()))
                .ReturnsAsync(sendNotificationApiResponse);

            // Act
            var result = await processEmailNotificationService.SendEmailNotification(notificationMessage, emailTemplateResponse, new NotificationAuditEntry("test"));

            // Assert
            result.Should().BeEquivalentTo(sendNotificationAggregateResponse);
            _mockSendNotificationService
                .Verify(
                    service => service.SendNotificationAsync(
                    It.IsAny<EmailNotification>()),
                    Times.Exactly(sendNotificationServiceInvocationCount));
        }

        [TestMethod]
        [DynamicData(
            nameof(ProcessEmailTemplateResponse),
            typeof(UnitTestSetupBase),
            DynamicDataSourceType.Method)]
        public async Task ProcessEmailTemplateResponse_ReturnsExpectedResult(
            NotificationMessage notificationMessage,
            EmailTemplateResponse emailTemplateApiResponse,
            int emailTemplateServiceInvocationCount,
            EmailTemplateResponse emailTemplateResponse)
        {
            // Arrange
            var processEmailNotificationService = GetProcessEmailNotificationService();

            _mockEmailTemplateService
                .Setup(service => service.GetEmailTemplateDetailsAsync(
                    It.IsAny<EmailTemplateRequest>()))
                .ReturnsAsync(emailTemplateApiResponse);

            _mockCacheService
                .Setup(s => s.Get(It.IsAny<string>(), It.IsAny<Func<Task<EmailTemplateResponse>>>(), It.IsAny<CacheOptions>()))
                .ReturnsAsync(emailTemplateApiResponse);

            // Act
            var result = await processEmailNotificationService.ProcessEmailTemplateResponse(notificationMessage, new NotificationAuditEntry("test"));

            // Assert
            result.Should().BeEquivalentTo(emailTemplateResponse);
            _mockCacheService
                .Verify(
                    s => s.Get(
                        It.IsAny<string>(),
                        It.IsAny<Func<Task<EmailTemplateResponse>>>(),
                        It.IsAny<CacheOptions>()),
                    Times.Exactly(emailTemplateServiceInvocationCount));
        }


        [TestMethod]
        public async Task SendEmailNotification_Throws_RateExceptionError()
        {
            // Arrange
            var processEmailNotificationService = GetProcessEmailNotificationService();

            _mockSendNotificationService
                .Setup(service => service.SendNotificationAsync(
                    It.IsAny<EmailNotification>()))
                .ReturnsAsync(new SendNotificationResponse
                {
                    RateLimitException = true
                });

            // Act
            Func<Task> act = async () => { await processEmailNotificationService.SendEmailNotification(GetNotificationMessage(), new EmailTemplateResponse(), new NotificationAuditEntry("test")); };

            // Assert
            await act.Should().ThrowAsync<ThrottleException>();
        }

        [TestMethod]
        public async Task SendEmailNotification_Throws_And_LogsExceptionError()
        {
            // Arrange
            var processEmailNotificationService = GetProcessEmailNotificationService();

            _mockSendNotificationService
                .Setup(service => service.SendNotificationAsync(
                    It.IsAny<EmailNotification>()))
                .Throws(new Exception());

            // Act
            await processEmailNotificationService.SendEmailNotification(GetNotificationMessage(), new EmailTemplateResponse(), new NotificationAuditEntry());

            var errorMessage =
                $"Failed to send GovUKNotify notification {JsonConvert.SerializeObject(GetNotificationMessage())} with exception";

            // Assert
            _mockLogger.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.Is<string>(message => message == errorMessage)));
        }

        private static IMapper GetMapper()
        {
            return new MapperConfiguration(x => x.AddProfile(new FunctionAutoMapperProfile())).CreateMapper();
        }

        private ProcessEmailNotificationService GetProcessEmailNotificationService()
        {
            _mockLogger.Setup(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()));

            _mockLogger.Setup(logger => logger.LogError(It.IsAny<string>()));

            _mockLogger.Setup(logger => logger.LogTrace(It.IsAny<string>()));

            _mockLogger.Setup(logger => logger.LogInformation(It.IsAny<string>()));

            return new ProcessEmailNotificationService(
                _mockSendNotificationService.Object,
                _mapper,
                _mockEmailTemplateService.Object,
                _mockLogger.Object,
                _mockCacheService.Object);
        }
    }
}