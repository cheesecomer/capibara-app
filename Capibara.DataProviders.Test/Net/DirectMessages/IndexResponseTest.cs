using System;
using System.Collections.Generic;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

using Subject = Capibara.Net.DirectMessages.IndexResponse;

namespace Capibara.Test.Net.DirectMessages.IndexResponse
{
    public class DeserializeTest
    {
        [TestCase]
        public void ItShouldExpected()
        {
            var json = "{ \"threads\": [" +
                "{ \"latest_direct_message\": { \"id\": 1, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }, \"user\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"latest_direct_message\": { \"id\": 2, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }, \"user\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"latest_direct_message\": { \"id\": 3, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }, \"user\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"latest_direct_message\": { \"id\": 4, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }, \"user\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"latest_direct_message\": { \"id\": 5, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }, \"user\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"latest_direct_message\": { \"id\": 6, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }, \"user\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"latest_direct_message\": { \"id\": 7, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }, \"user\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"latest_direct_message\": { \"id\": 8, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }, \"user\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"latest_direct_message\": { \"id\": 9, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }, \"user\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "]}";

            var actual = JsonConvert.DeserializeObject<Subject>(json);
            var comparer = new DirectMessageThreadComparer();
            var expect = new List<DirectMessageThread>
                {
                new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 1 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 2 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 3 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 4 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 5 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 6 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 7 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 8 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 9 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                };
            Assert.That(actual.Threads, Is.EqualTo(expect).Using(comparer));
        }
    }

    public class DirectMessageThreadComparer : IEqualityComparer<DirectMessageThread>
    {
        public bool Equals(DirectMessageThread x, DirectMessageThread y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else
                return x.User.Id == y.User.Id
                        && x.User.Nickname == y.User.Nickname
                        && x.User.IconThumbnailUrl == y.User.IconThumbnailUrl
                        && x.LatestDirectMessage.Content == y.LatestDirectMessage.Content
                        && x.LatestDirectMessage.At == y.LatestDirectMessage.At
                        && x.LatestDirectMessage.Sender.Id == y.LatestDirectMessage.Sender.Id
                        && x.LatestDirectMessage.Sender.Nickname == y.LatestDirectMessage.Sender.Nickname
                        && x.LatestDirectMessage.Sender.IconThumbnailUrl == y.LatestDirectMessage.Sender.IconThumbnailUrl;
        }

        public int GetHashCode(DirectMessageThread room)
        {
            return room.GetHashCode();
        }
    }
}
