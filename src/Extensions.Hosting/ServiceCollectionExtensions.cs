using Microsoft.Extensions.DependencyInjection;
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
    }
}
