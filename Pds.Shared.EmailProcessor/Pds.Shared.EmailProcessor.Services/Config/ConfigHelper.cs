using Microsoft.Extensions.Configuration;

namespace Pds.Shared.EmailProcessor.Services.Config
{
    /// <summary>
    /// Config helper class.
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Gets the configuration root.
        /// </summary>
        /// <returns>The configuration.</returns>
        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}