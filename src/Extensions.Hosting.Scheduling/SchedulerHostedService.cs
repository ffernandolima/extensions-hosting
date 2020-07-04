using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions.Hosting.Scheduling
{
    public class SchedulerHostedService : HostedService
    {
        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        private readonly List<SchedulerTaskWrapper> _scheduledTasks;
        private readonly TimeSpan _delay;

        public SchedulerHostedService(IEnumerable<IScheduledTask> scheduledTasks, TimeSpan? delay = null)
        {
            _scheduledTasks = new List<SchedulerTaskWrapper>();

            var referenceTime = DateTime.UtcNow;

            foreach (var scheduledTask in scheduledTasks)
            {
                var wrapper = new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
                    Task = scheduledTask,
                    NextRunTime = referenceTime
                };

                _scheduledTasks.Add(wrapper);
            }

            _delay = delay ?? TimeSpan.FromMinutes(1);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);

                await Task.Delay(_delay, cancellationToken);
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);

            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await taskThatShouldRun.Task.ExecuteAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var aggregateEx = ex as AggregateException ?? new AggregateException(ex);

                            var args = new UnobservedTaskExceptionEventArgs(aggregateEx);

                            UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }

        private class SchedulerTaskWrapper
        {
            public CrontabSchedule Schedule { get; set; }
            public IScheduledTask Task { get; set; }

            public DateTime LastRunTime { get; set; }
            public DateTime NextRunTime { get; set; }

            public void Increment()
            {
                LastRunTime = NextRunTime;
                NextRunTime = Schedule.GetNextOccurrence(NextRunTime);
            }

            public bool ShouldRun(DateTime currentTime)
                => NextRunTime < currentTime && LastRunTime != NextRunTime;
        }
    }
}
