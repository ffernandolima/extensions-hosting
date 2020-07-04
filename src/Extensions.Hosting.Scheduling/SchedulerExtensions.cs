using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Extensions.Hosting.Scheduling
{
    public static class SchedulerExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services, TimeSpan? delay = null)
            => services.AddScheduler(unobservedTaskExceptionHandler: null, delay);

        public static IServiceCollection AddScheduler(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler, TimeSpan? delay = null)
            => services.AddSingleton<IHostedService, SchedulerHostedService>(serviceProvider =>
            {
                var scheduler = new SchedulerHostedService(serviceProvider.GetServices<IScheduledTask>(), delay);

                if (unobservedTaskExceptionHandler != null)
                {
                    scheduler.UnobservedTaskException += unobservedTaskExceptionHandler;
                }

                return scheduler;
            });
    }
}
