using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Pds.Core.Logging;
using Pds.Core.Notification.Models;
using Pds.Shared.EmailProcessor.Func.Interfaces;
using Pds.Shared.EmailProcessor.Services.Config;
using Pds.Shared.EmailProcessor.Services.Interfaces;
using Pds.Shared.EmailProcessor.Services.Models;
using System;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Func.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class EmailNotificationFunctionTests : UnitTestSetupBase
    {
        private readonly Mock<IProcessEmailNotificationService> _mockProcessEmailNotificationService =
            new Mock<IProcessEmailNotificationService>(MockBehavior.Strict);

        private readonly Mock<IStorageAuditService<NotificationAuditEntry>> _mockCosmosAuditService =
            new Mock<IStorageAuditService<NotificationAuditEntry>>(MockBehavior.Strict);

        private readonly Mock<IOptions<ServiceConfiguration>> _mockServiceConfiguration =
            new Mock<IOptions<ServiceConfiguration>>(MockBehavior.Strict);

        private readonly Mock<ILoggerAdapter<EmailNotificationFunction>> _mockLogger =
            new Mock<ILoggerAdapter<EmailNotificationFunction>>(MockBehavior.Strict);

        [TestMethod]
        [DynamicData(
            nameof(RunInput),
            typeof(UnitTestSetupBase),
            DynamicDataSourceType.Method)]
        public async Task Run_ReturnsExpectedResult(
            NotificationMessage notificationMessage,
            EmailTemplateResponse emailTemplateResponse,
            int sendNotificationServiceInvocationCount,
            SendNotificationResponse sendNotificationResponse,
            int auditServiceInvocationCount,
            NotificationAuditEntry expectedNotificationAuditEntry)
        {
            // Arrange
            var function = GetEmailNotificationFunction();

            _mockProcessEmailNotificationService
                .Setup(service => service.ProcessEmailTemplateResponse(
                    It.IsAny<NotificationMessage>(),
                    It.IsAny<NotificationAuditEntry>()))
                .ReturnsAsync(emailTemplateResponse);

            _mockProcessEmailNotificationService
                .Setup(service => service.SendEmailNotification(
                    It.IsAny<NotificationMessage>(),
                    It.IsAny<EmailTemplateResponse>(),
                    It.IsAny<NotificationAuditEntry>()))
                .ReturnsAsync(sendNotificationResponse);

            _mockCosmosAuditService.Setup(service => service.AddEntryAsync(It.IsAny<NotificationAuditEntry>()))
                    .ReturnsAsync(true);

            // Act
            await function.Run(notificationMessage);

            // Assert
            _mockProcessEmailNotificationService
                .Verify(
                    service => service.SendEmailNotification(
                    It.IsAny<NotificationMessage>(),
                    It.IsAny<EmailTemplateResponse>(),
                    It.IsAny<NotificationAuditEntry>()),
                    Times.Exactly(sendNotificationServiceInvocationCount));

            _mockProcessEmailNotificationService
                .Verify(
                    service => service.ProcessEmailTemplateResponse(
                        It.IsAny<NotificationMessage>(),
                        It.IsAny<NotificationAuditEntry>()),
                    Times.Exactly(sendNotificationServiceInvocationCount));

            _mockCosmosAuditService.Verify(
                service => service.AddEntryAsync(
                    It.Is<NotificationAuditEntry>(auditEntry =>
                        JsonConvert.SerializeObject(auditEntry.NotificationErrors) == JsonConvert.SerializeObject(expectedNotificationAuditEntry.NotificationErrors))),
                Times.Exactly(auditServiceInvocationCount));
        }

        private EmailNotificationFunction GetEmailNotificationFunction()
        {
            _mockServiceConfiguration.Setup(x => x.Value)
                .Returns(new ServiceConfiguration());

            _mockLogger.Setup(logger => logger.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            _mockLogger.Setup(logger => logger.LogTrace(It.IsAny<string>()));

            _mockLogger.Setup(logger => logger.LogInformation(It.IsAny<string>()));

            return new EmailNotificationFunction(
                _mockProcessEmailNotificationService.Object,
                _mockServiceConfiguration.Object,
                _mockLogger.Object,
                _mockCosmosAuditService.Object);
        }
    }
}