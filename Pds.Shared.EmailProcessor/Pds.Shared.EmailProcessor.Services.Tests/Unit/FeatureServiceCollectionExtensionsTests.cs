using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Shared.EmailProcessor.Services.Base;
using Pds.Shared.EmailProcessor.Services.DependencyInjection;

namespace Pds.Shared.EmailProcessor.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class FeatureServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddFeatureServicesTests()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            services.AddFeatureServices();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<IGovUkNotifyClientProxy>().Should().NotBeNull();
        }
    }
}