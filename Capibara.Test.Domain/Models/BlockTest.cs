#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.ComponentModel;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Reactive.Bindings.Extensions;

namespace Capibara.Domain.Models
{
    [TestFixture]
    public class BlockTest
    {
        static private IEnumerable RestoreTestCase()
        {
            yield return new TestCaseData(
                new Action<Block, Block>((actual, expected) => Assert.That(actual.Restore(expected).Id, Is.EqualTo(expected.Id))))
                .SetName("Restore Should Id is to be restored");

            yield return new TestCaseData(
                new Action<Block, Block>((actual, expected) => Assert.That(actual.Restore(expected).Target, Is.EqualTo(expected.Target))))
                .SetName("Restore Should Target is to be restored");
        }

        [Test]
        [TestCaseSource("RestoreTestCase")]
        public void Restore(Action<Block, Block> assert)
        {
            assert.Invoke(new Block(), ModelFixture.Block());
        }

        static private IEnumerable PropertyChangedTestCase()
        {
            yield return new TestCaseData(
                "Id", new Action<Block>(x => { }), 0)
                .SetName("Id Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Id", new Action<Block>(x => x.Id = Faker.RandomNumber.Next()), 1)
                .SetName("Id Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "Target", new Action<Block>(x => { }), 0)
                .SetName("Target Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Target", new Action<Block>(x => x.Target = ModelFixture.User()), 1)
                .SetName("Target Property When chnaged Should raise PropertyChanged");
        }

        [Test]
        [TestCaseSource("PropertyChangedTestCase")]
        public void PropertyChanged(string propertyName, Action<Block> setter, int expected)
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new Block();
            subject.PropertyChangedAsObservable()
                .Where(x => x.PropertyName == propertyName)
                .Subscribe(observer);

            setter.Invoke(subject);

            Assert.That(observer.Messages.Count, Is.EqualTo(expected));
        }
    }
}
