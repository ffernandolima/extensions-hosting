using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions.Hosting.Scheduling
{
    public abstract class HostedService : IHostedService, IDisposable
    {
        private Task _executingTask;
        private CancellationTokenSource _stoppingCts;

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _executingTask = ExecuteAsync(_stoppingCts.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
            }
        }

        public virtual void Dispose()
        {
            _stoppingCts?.Cancel();
        }
    }
}
