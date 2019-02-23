using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using Prism.Services;

namespace Capibara.Presentation
{
    public static class ObservableExtensions
    {
        public static IObservable<T> RetryWhen<T>(
            this IObservable<T> source,
            IPageDialogService pageDialogService,
            IScheduler scheduler)
        {
            return source
                .RetryWhen(observable =>
                    observable
                        .SelectMany(e =>
                            Observable.FromAsync(
                                () => pageDialogService.DisplayErrorAlertAsync(e),
                                scheduler)
                                .Select(v => new Pair<Exception, bool>(e, v)))
                        .SelectMany(v => v.Second
                            ? Observable.Return(Unit.Default)
                            : Observable.Throw<Unit>(v.First)));
        }
    }
}
