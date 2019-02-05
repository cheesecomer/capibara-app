using System.Reactive.Concurrency;
using Capibara.Reactive;
using Microsoft.Reactive.Testing;

namespace Capibara.Presentation
{
    public class SchedulerProvider : ISchedulerProvider
    {
        public TestScheduler Scheduler { get; } = new TestScheduler();

        public IScheduler UI => Scheduler;

        public IScheduler Delay => Scheduler;

        public IScheduler IO => Scheduler;
    }
}
