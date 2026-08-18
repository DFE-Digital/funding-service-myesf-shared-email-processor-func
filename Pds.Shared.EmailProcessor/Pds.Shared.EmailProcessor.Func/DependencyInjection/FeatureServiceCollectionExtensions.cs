using Microsoft.Extensions.DependencyInjection;
using Pds.Shared.EmailProcessor.Func.Implementations;
using Pds.Shared.EmailProcessor.Func.Interfaces;

namespace Pds.Shared.EmailProcessor.Func.DependencyInjection
{
    /// <summary>
    /// Extensions class for <see cref="IServiceCollection"/> for registering the feature's services.
    /// </summary>
    public static class FeatureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for the function to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the feature's services to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFunctionServices(this IServiceCollection services)
        {
            services
                .AddSingleton<IProcessEmailNotificationService, ProcessEmailNotificationService>();
            return services;
        }
    }
}