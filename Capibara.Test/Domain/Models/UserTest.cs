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
    public class UserTest
    {
        static private IEnumerable RestoreTestCase()
        {
            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Restore(expected).Id, Is.EqualTo(expected.Id))))
                .SetName("Restore Should Id is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Restore(expected).Nickname, Is.EqualTo(expected.Nickname))))
                .SetName("Restore Should Nickname is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Restore(expected).Biography, Is.EqualTo(expected.Biography))))
                .SetName("Restore Should Biography is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Restore(expected).IconUrl, Is.EqualTo(expected.IconUrl))))
                .SetName("Restore Should IconUrl is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Restore(expected).IconThumbnailUrl, Is.EqualTo(expected.IconThumbnailUrl))))
                .SetName("Restore Should IconThumbnailUrl is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Restore(expected).BlockId, Is.EqualTo(expected.BlockId))))
                .SetName("Restore Should BlockId is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Restore(expected).FollowId, Is.EqualTo(expected.FollowId))))
                .SetName("Restore Should FollowId is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Restore(expected).FriendsCount, Is.EqualTo(expected.FriendsCount))))
                .SetName("Restore Should FriendsCount is to be restored");
        }

        [Test]
        [TestCaseSource("RestoreTestCase")]
        public void Restore(Action<User, User> assert)
        {
            assert.Invoke(new User(), ModelFixture.User());
        }

        [Test]
        public void BlockId_WhenUpdate_ShouldRisedIsBlockPropertyChanged()
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new User();
            subject.PropertyChangedAsObservable()
                .Where(x => x.PropertyName == nameof(User.IsBlock))
                .Subscribe(observer);

            subject.BlockId = Faker.RandomNumber.Next();

            Assert.That(observer.Messages.Count, Is.EqualTo(1));
        }

        [TestCase(false, TestName = "BlockId WhenEmpty ShouldIsNotBlock")]
        [TestCase(true, TestName = "BlockId WhenPresent ShouldIsBlock")]
        public void BlockId_WhenPresent_ShouldIsBlock(bool hasBlockId)
        {
            var subject = new User { BlockId = hasBlockId ? (int?)Faker.RandomNumber.Next() : null };
            Assert.That(subject.IsBlock, Is.EqualTo(hasBlockId));
        }

        [Test]
        public void FollowId_WhenUpdate_ShouldRisedIsFollowPropertyChanged()
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new User();
            subject.PropertyChangedAsObservable()
                .Where(x => x.PropertyName == nameof(User.IsFollow))
                .Subscribe(observer);

            subject.FollowId = Faker.RandomNumber.Next();

            Assert.That(observer.Messages.Count, Is.EqualTo(1));
        }

        [TestCase(false, TestName = "FollowId WhenEmpty ShouldIsNotFollow")]
        [TestCase(true, TestName = "FollowId WhenPresent ShouldIsFollow")]
        public void FollowId_WhenPresent_ShouldIsBlock(bool hasFollowId)
        {
            var subject = new User { FollowId = hasFollowId ? (int?)Faker.RandomNumber.Next() : null };
            Assert.That(subject.IsFollow, Is.EqualTo(hasFollowId));
        }

        static private IEnumerable PropertyChangedTestCase()
        {
            yield return new TestCaseData(
                "Id", new Action<User>(x => { }), 0)
                .SetName("Id Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Id", new Action<User>(x => x.Id = x.Id + 1), 1)
                .SetName("Id Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "Nickname", new Action<User>(x => { }), 0)
                .SetName("Name Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Nickname", new Action<User>(x => x.Nickname = Faker.Name.FullName()), 1)
                .SetName("Name Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "Biography", new Action<User>(x => { }), 0)
                .SetName("Biography Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Biography", new Action<User>(x => x.Biography = Faker.Lorem.Paragraph()), 1)
                .SetName("Biography Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "IconUrl", new Action<User>(x => { }), 0)
                .SetName("IconUrl Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "IconUrl", new Action<User>(x => x.IconUrl = Faker.Url.Image()), 1)
                .SetName("IconUrl Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "IconThumbnailUrl", new Action<User>(x => { }), 0)
                .SetName("IconThumbnailUrl Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "IconThumbnailUrl", new Action<User>(x => x.IconThumbnailUrl = Faker.Url.Image()), 1)
                .SetName("IconThumbnailUrl Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "BlockId", new Action<User>(x => { }), 0)
                .SetName("BlockId Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "BlockId", new Action<User>(x => x.BlockId = (x.BlockId ?? Faker.RandomNumber.Next()) + 1), 1)
                .SetName("BlockId Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "FollowId", new Action<User>(x => { }), 0)
                .SetName("FollowId Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "FollowId", new Action<User>(x => x.FollowId = (x.FollowId ?? Faker.RandomNumber.Next()) + 1), 1)
                .SetName("FollowId Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "FriendsCount", new Action<User>(x => { }), 0)
                .SetName("FollowId Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "FriendsCount", new Action<User>(x => x.FriendsCount = x.FriendsCount + 1), 1)
                .SetName("FriendsCount Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "IsAccepted", new Action<User>(x => { }), 0)
                .SetName("IsAccepted Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "IsAccepted", new Action<User>(x => x.IsAccepted = !x.IsAccepted), 1)
                .SetName("IsAccepted Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "IsAccepted", new Action<User>(x => { }), 0)
                .SetName("IsAccepted Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "IsFollower", new Action<User>(x => x.IsFollower = !x.IsFollower), 1)
                .SetName("IsFollower Property When chnaged Should raise PropertyChanged");
        }

        [Test]
        [TestCaseSource("PropertyChangedTestCase")]
        public void PropertyChanged(string propertyName, Action<User> setter, int expected)
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new User();
            subject.PropertyChangedAsObservable()
                .Where(x => x.PropertyName == propertyName)
                .Subscribe(observer);

            setter.Invoke(subject);

            Assert.That(observer.Messages.Count, Is.EqualTo(expected));
        }
    }
}