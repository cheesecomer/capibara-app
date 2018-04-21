using System;
using System.Collections.Generic;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

using Subject = Capibara.Net.Informations.IndexResponse;

namespace Capibara.Test.Net.Informations.IndexResponse
{
    public class DeserializeTest
    {
        [TestCase]
        public void ItShouldExpected()
        {
            var json = "{ \"informations\": [" +
                "{ \"id\": 1, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\", \"url\": \"http://example.com/informations/1\"}," +
                "{ \"id\": 2, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\", \"url\": \"http://example.com/informations/2\"}," +
                "{ \"id\": 3, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\", \"url\": \"http://example.com/informations/3\"}," +
                "{ \"id\": 4, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\", \"url\": \"http://example.com/informations/4\"}," +
                "{ \"id\": 5, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\", \"url\": \"http://example.com/informations/5\"}," +
                "{ \"id\": 6, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\", \"url\": \"http://example.com/informations/6\"}," +
                "{ \"id\": 7, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\", \"url\": \"http://example.com/informations/7\"}," +
                "{ \"id\": 8, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\", \"url\": \"http://example.com/informations/8\"}," +
                "{ \"id\": 9, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\", \"url\": \"http://example.com/informations/9\"}" +
                "]}";
            
            var actual = JsonConvert.DeserializeObject<Subject>(json);
            var comparer = new InformationComparer();
            var expect = new List<Information>
                {
                    new Information { Id = 1, Title = "Title!!!", Message = "Message!!!!", Url = "http://example.com/informations/1", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 2, Title = "Title!!!", Message = "Message!!!!", Url = "http://example.com/informations/2", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 3, Title = "Title!!!", Message = "Message!!!!", Url = "http://example.com/informations/3", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 4, Title = "Title!!!", Message = "Message!!!!", Url = "http://example.com/informations/4", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 5, Title = "Title!!!", Message = "Message!!!!", Url = "http://example.com/informations/5", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 6, Title = "Title!!!", Message = "Message!!!!", Url = "http://example.com/informations/6", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 7, Title = "Title!!!", Message = "Message!!!!", Url = "http://example.com/informations/7", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 8, Title = "Title!!!", Message = "Message!!!!", Url = "http://example.com/informations/8", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 9, Title = "Title!!!", Message = "Message!!!!", Url = "http://example.com/informations/9", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                };
            Assert.That(actual.Informations, Is.EqualTo(expect).Using(comparer));
        }
    }

    public class InformationComparer : IEqualityComparer<Information>
    {
        public bool Equals(Information x, Information y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else
                return
                   x.Title == y.Title
                && x.Message == y.Message
                && x.Id == y.Id
                && x.Url == y.Url
                && x.PublishedAt == y.PublishedAt;
        }

        public int GetHashCode(Information room)
        {
            return room.GetHashCode();
        }
    }
}
