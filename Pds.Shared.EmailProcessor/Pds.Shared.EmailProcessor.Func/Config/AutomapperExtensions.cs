using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Pds.Shared.EmailProcessor.Func.Config
{
    /// <summary>
    /// The automapper extensions class.
    /// </summary>
    public static class AutomapperExtensions
    {
        /// <summary>
        /// Adds the automapper configuration.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAutomapperConfiguration(this IServiceCollection services)
        {
            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new FunctionAutoMapperProfile());
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}