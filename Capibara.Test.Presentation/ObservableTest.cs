#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace Capibara.Presentation
{
    [TestFixture]
    public class ObservableTest
    {
        [Test]
        public void RetryWhen()
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<Unit>();
            //var retryObservable = new Subject<IObservable<bool>>();

            var taskQueue = new Stack<bool>();

            Func<Task<bool>> function = () =>
            {
                var needRetry = taskQueue.Peek();
                TestContext.Out.WriteLine($"Retry ? {needRetry}");
                return Task.FromResult(needRetry);
            };

            //var retryObservable = new Subject<IObservable<bool>>();

            Observable
                .Throw<Unit>(new Exception())
                .RetryWhen(x => 
                    x.ObserveOn(scheduler)
                        .SelectMany(e => function().ToObservable().Select(v => new Pair<Exception, bool>(e, v)))
                        .SelectMany(v => v.Second
                            ? Observable.Return(Unit.Default) 
                            : Observable.Throw<Unit>(v.First))
                        .Do(_ => TestContext.Out.WriteLine("Retry ... ")))
                .SubscribeOn(scheduler)
                .Subscribe(observer);

            scheduler.AdvanceBy(1);

            TestContext.Out.WriteLine("RetryWhen: Observable.Return(1)");
            taskQueue.Push(true);
            scheduler.AdvanceBy(1);
            Assert.That(observer.Messages.Count, Is.EqualTo(0));

            TestContext.Out.WriteLine("RetryWhen: Observable.Return(2)");
            taskQueue.Push(true);
            scheduler.AdvanceBy(1);
            Assert.That(observer.Messages.Count, Is.EqualTo(0));

            TestContext.Out.WriteLine("RetryWhen: Observable.Return(3)");
            taskQueue.Push(true);
            scheduler.AdvanceBy(1);
            Assert.That(observer.Messages.Count, Is.EqualTo(0));

            TestContext.Out.WriteLine("RetryWhen: Observable.Throw<Unit>(new Exception())");
            taskQueue.Push(false);
            scheduler.AdvanceBy(1);
            Assert.NotNull(observer.Messages.LastOrDefault().Value?.Exception);

            //Observable
            //    .Throw<Unit>(new NotImplementedException())
            //    .RetryWhen(
            //        f =>
            //           retryObservable
            //                .SelectMany(x => x)
            //                .Do((v) => TestContext.Out.WriteLine($"Retrying...: {v}")))
            //    .SubscribeOn(scheduler)
            //    .Subscribe(observer);

            //scheduler.AdvanceBy(1);

            //TestContext.Out.WriteLine("RetryWhen: Observable.Return(1)");
            //retryObservable.OnNext(Observable.Return(true));
            //Assert.That(observer.Messages.Count, Is.EqualTo(0));

            //TestContext.Out.WriteLine("RetryWhen: Observable.Return(2)");
            //retryObservable.OnNext(Observable.Return(true));
            //Assert.That(observer.Messages.Count, Is.EqualTo(0));

            //TestContext.Out.WriteLine("RetryWhen: Observable.Return(3)");
            //retryObservable.OnNext(Observable.Return(true));
            //Assert.That(observer.Messages.Count, Is.EqualTo(0));

            //TestContext.Out.WriteLine("RetryWhen: Observable.Throw<Unit>(new Exception())");
            //retryObservable.OnNext(Observable.Throw<bool>(new Exception()));
            //Assert.NotNull(observer.Messages.LastOrDefault().Value?.Exception);
        }
    }
}
