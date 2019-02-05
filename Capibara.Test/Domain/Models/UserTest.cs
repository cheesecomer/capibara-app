#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using NUnit.Framework;

namespace Capibara.Domain.Models
{
    [TestFixture]
    public class UserTest
    {
        static private IEnumerable RestoreTestCase()
        {
            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Id, Is.EqualTo(expected.Id))))
                .SetName("Restore Should Id is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Nickname, Is.EqualTo(expected.Nickname))))
                .SetName("Restore Should Nickname is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.Biography, Is.EqualTo(expected.Biography))))
                .SetName("Restore Should Biography is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.IconUrl, Is.EqualTo(expected.IconUrl))))
                .SetName("Restore Should IconUrl is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.IconThumbnailUrl, Is.EqualTo(expected.IconThumbnailUrl))))
                .SetName("Restore Should IconThumbnailUrl is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.BlockId, Is.EqualTo(expected.BlockId))))
                .SetName("Restore Should BlockId is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.FollowId, Is.EqualTo(expected.FollowId))))
                .SetName("Restore Should FollowId is to be restored");

            yield return new TestCaseData(
                new Action<User, User>((actual, expected) => Assert.That(actual.FriendsCount, Is.EqualTo(expected.FriendsCount))))
                .SetName("Restore Should FriendsCount is to be restored");
        }

        [Test]
        [TestCaseSource("RestoreTestCase")]
        public void Restore(Action<User, User> assert)
        {
            var subject = new User();
            var expect = ModelFixture.User();
            subject.Restore(expect);

            assert.Invoke(subject.Restore(expect), expect);
        }

        [Test]
        public void BlockId_WhenUpdate_ShouldRisedIsBlockPropertyChanged()
        {
            var isBlockChanged = false;
            var subject = new User();
            subject.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(User.IsBlock))
                {
                    isBlockChanged = true;
                }
            };

            subject.BlockId = Faker.RandomNumber.Next();

            Assert.IsTrue(isBlockChanged);
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
            var isFollowChanged = false;
            var subject = new User();
            subject.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(User.IsFollow))
                {
                    isFollowChanged = true;
                }
            };

            subject.FollowId = Faker.RandomNumber.Next();

            Assert.IsTrue(isFollowChanged);
        }

        [TestCase(false, TestName = "FollowId WhenEmpty ShouldIsNotFollow")]
        [TestCase(true, TestName = "FollowId WhenPresent ShouldIsFollow")]
        public void FollowId_WhenPresent_ShouldIsBlock(bool hasFollowId)
        {
            var subject = new User { FollowId = hasFollowId ? (int?)Faker.RandomNumber.Next() : null };
            Assert.That(subject.IsFollow, Is.EqualTo(hasFollowId));
        }
    }
}