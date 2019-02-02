using System.Reactive.Concurrency;

namespace Capibara.Reactive
{
    public interface ISchedulerProvider
    {
        IScheduler UI { get; }

        IScheduler Delay { get; }

        IScheduler IO { get; }
    }
}
