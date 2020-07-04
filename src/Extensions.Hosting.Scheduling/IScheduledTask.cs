using System.Threading;
using System.Threading.Tasks;

namespace Extensions.Hosting.Scheduling
{
    public interface IScheduledTask
    {
        string Schedule { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
