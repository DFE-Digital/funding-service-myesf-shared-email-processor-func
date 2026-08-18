using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.AzureStorage.Interfaces;
using Pds.Core.Logging;
using Pds.Shared.EmailProcessor.Services.Implementations;
using Pds.Shared.EmailProcessor.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class AzureTableStorageDbAuditServiceTests
    {
        private readonly Mock<IAzureTableStorageRepository<NotificationAuditEntry>> _mockRepository = new Mock<IAzureTableStorageRepository<NotificationAuditEntry>>(MockBehavior.Strict);
        private readonly ILoggerAdapter<AzureTableStorageDbAuditService<NotificationAuditEntry>> _logger = null;

        [TestMethod]
        public async Task AddAsync_ReturnsExpected()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.Insert(It.IsAny<List<NotificationAuditEntry>>())).Returns(Task.CompletedTask);
            var service = GetCosmosDbService();
            var auditEntry = new NotificationAuditEntry("test");

            // Act
            var result = await service.AddEntryAsync(auditEntry);

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(repo => repo.Insert(It.IsAny<List<NotificationAuditEntry>>()), Times.Once);
        }

        private AzureTableStorageDbAuditService<NotificationAuditEntry> GetCosmosDbService()
        {
            return new AzureTableStorageDbAuditService<NotificationAuditEntry>(_mockRepository.Object, _logger);
        }
    }
}