using System;
using System.Collections.Generic;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

using Subject = Capibara.Net.DirectMessages.ShowResponse;

namespace Capibara.Test.Net.DirectMessages.ShowResponse
{
    public class DeserializeTest
    {
        [TestCase]
        public void ItShouldExpected()
        {
            var json = "{ \"direct_messages\": [" +
                "{ \"id\": 1, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"id\": 2, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"id\": 3, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"id\": 4, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"id\": 5, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"id\": 6, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"id\": 7, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"id\": 8, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "{ \"id\": 9, \"content\": \"Message!!!!\", \"at\": \"2017-10-28T20:25:20.000+09:00\", \"sender\": { \"id\": 1, \"nickname\": \"smith\" } }," +
                "]}";

            var actual = JsonConvert.DeserializeObject<Subject>(json);
            var comparer = new DirectMessageComparer();
            var expect = new List<DirectMessage>
                {
                    new DirectMessage { Id = 1 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessage { Id = 2 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessage { Id = 3 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessage { Id = 4 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessage { Id = 5 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessage { Id = 6 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessage { Id = 7 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessage { Id = 8 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessage { Id = 9 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } },
                };
            Assert.That(actual.DirectMessages, Is.EqualTo(expect).Using(comparer));
        }
    }

    public class DirectMessageComparer : IEqualityComparer<DirectMessage>
    {
        public bool Equals(DirectMessage x, DirectMessage y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else
                return x.Content == y.Content
                        && x.At == y.At
                        && x.Sender.Id == y.Sender.Id
                        && x.Sender.Nickname == y.Sender.Nickname
                        && x.Sender.IconThumbnailUrl == y.Sender.IconThumbnailUrl;
        }

        public int GetHashCode(DirectMessage room)
        {
            return room.GetHashCode();
        }
    }
}
