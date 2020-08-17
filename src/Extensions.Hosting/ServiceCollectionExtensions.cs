using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Extensions.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseStartup<TStartup>(this IServiceCollection services) where TStartup : IStartup
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton(typeof(IStartup), typeof(TStartup));

            var serviceProvider = services.BuildServiceProvider();

            var startup = serviceProvider.GetService<IStartup>();

            startup.ConfigureServices(services);

            return services;
        }

        public static IServiceCollection UseStartup<TStartup>(this IServiceCollection services, IConfiguration configuration) where TStartup : IStartup
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var startup = (IStartup)Activator.CreateInstance(typeof(TStartup), new object[] { configuration });

            startup.ConfigureServices(services);

            return services;
        }

        public static IServiceCollection UseStartup<TStartup>(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment) where TStartup : IStartup
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var startup = (IStartup)Activator.CreateInstance(typeof(TStartup), new object[] { configuration, hostEnvironment });

            startup.ConfigureServices(services);

            return services;
        }
    }
}
