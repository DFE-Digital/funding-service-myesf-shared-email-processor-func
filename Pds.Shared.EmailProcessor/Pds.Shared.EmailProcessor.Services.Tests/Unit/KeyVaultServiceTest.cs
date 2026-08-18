using Azure;
using Azure.Security.KeyVault.Secrets;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Logging;
using Pds.Shared.EmailProcessor.Services.Implementations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class KeyVaultServiceTest
    {
        private readonly Mock<ILoggerAdapter<KeyVaultService>> _mockLogger =
           new Mock<ILoggerAdapter<KeyVaultService>>(MockBehavior.Strict);

        private Mock<SecretClient> _secretClientMock = new Mock<SecretClient>();

        [TestMethod]
        public void GetSecretValue_WhenCalledWithSecretName_ReturnsSecretValueAsync()
        {
            // Arrange
            var keyVaultService = GetKeyVaultService();

            // Act
            var result = keyVaultService.GetSecretValue("name");

            // Assert
            result.Result.Should().BeEquivalentTo("secret");
            SecretClientCalledTimes(Times.Once);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void GetSecretValue_WhenCalledWithoutSecretName_ReturnsSecretValue(string secretName)
        {
            // Arrange
            var keyVaultService = GetKeyVaultService();

            // Assert
            Assert.ThrowsExceptionAsync<ArgumentException>(async () => await keyVaultService.GetSecretValue(secretName))
                .Result.Message.Should().Be("Secret name cannot be null or empty");
            SecretClientCalledTimes(Times.Never);
        }

        [TestMethod]
        public void GetSecretValue_WhenException_ThrowsException()
        {
            // Arrange
            var keyVaultService = GetKeyVaultService();
            _secretClientMock.Setup(client => client.GetSecret(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            // Act
            Assert.ThrowsExceptionAsync<Exception>(async () => await keyVaultService.GetSecretValue("name"));
            SecretClientCalledTimes(Times.Once);
        }

        private KeyVaultService GetKeyVaultService(string secretName = "name", string secretValue = "secret")
        {
            var expected = new Mock<Response<KeyVaultSecret>>();
            expected.Setup(x => x.Value).Returns(new KeyVaultSecret(secretName, secretValue));

            _secretClientMock.Setup(client => client.GetSecretAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected.Object));

            _mockLogger.Setup(logger => logger.LogError(It.IsAny<string>()));
            _mockLogger.Setup(logger => logger.LogInformation(It.IsAny<string>()));

            var keyVaultService = new KeyVaultService(_secretClientMock.Object, _mockLogger.Object);

            return keyVaultService;
        }

        private void SecretClientCalledTimes(Func<Times> times)
        {
            _secretClientMock.Verify(client => client.GetSecretAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), times);
        }
    }
}
