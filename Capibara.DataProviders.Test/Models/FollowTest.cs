using System;
using System.Collections.Generic;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

using Subject = Capibara.Models.Follow;

namespace Capibara.Test.Models.Follow
{
    public class DeserializeTest
    {
        Subject Subject;

        [SetUp]
        public void SetUp()
        {
            var json = "{ \"follow\": { \"id\": 10 } }";
            this.Subject = JsonConvert.DeserializeObject<Subject>(json);
        }

        public void ItShouldIdExpect()
        {
            Assert.That(this.Subject.Id, Is.EqualTo(10));
        }
    }
}
