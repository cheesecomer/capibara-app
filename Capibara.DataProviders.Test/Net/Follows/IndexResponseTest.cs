using System;
using System.Collections.Generic;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

using Subject = Capibara.Net.Follows.IndexResponse;

namespace Capibara.Test.Net.Follows
{
    public class IndexResponseTest
    {
        public class DeserializeTest
        {
            [TestCase]
            public void ItShouldExpected()
            {
                var json = "{ \"follows\": [" +
                    "{ \"id\":  1 }," +
                    "{ \"id\":  2 }," +
                    "{ \"id\":  3 }," +
                    "{ \"id\":  4 }," +
                    "{ \"id\":  5 }," +
                    "{ \"id\":  6 }," +
                    "{ \"id\":  7 }," +
                    "{ \"id\":  8 }," +
                    "{ \"id\":  9 }," +
                    "]}";

                var actual = JsonConvert.DeserializeObject<Subject>(json);
                var comparer = new FollowComparer();
                var expect = new List<Follow>
                {
                    new Follow { Id = 1 },
                    new Follow { Id = 2 },
                    new Follow { Id = 3 },
                    new Follow { Id = 4 },
                    new Follow { Id = 5 },
                    new Follow { Id = 6 },
                    new Follow { Id = 7 },
                    new Follow { Id = 8 },
                    new Follow { Id = 9 },
                };
                Assert.That(actual.Follows, Is.EqualTo(expect).Using(comparer));
            }
        }
    }
}
