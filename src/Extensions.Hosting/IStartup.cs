using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Hosting
{
    public interface IStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}
