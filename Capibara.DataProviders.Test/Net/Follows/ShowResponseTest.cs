using System;
using System.Collections.Generic;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

using Subject = Capibara.Net.Follows.ShowResponse;

namespace Capibara.Test.Net.Follows.ShowResponse
{
    public class DeserializeTest
    {
        [TestCase]
        public void ItShouldExpected()
        {
            var json = "{ \"follow\": { \"id\": 10 } }";
            var actual = JsonConvert.DeserializeObject<Subject>(json);
            var comparer = new FollowComparer();
            var expect = new Follow { Id = 10 };
            Assert.That(actual.Follow, Is.EqualTo(expect).Using(comparer));
        }
    }
}
