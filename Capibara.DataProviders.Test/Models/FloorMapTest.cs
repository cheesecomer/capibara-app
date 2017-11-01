using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using Capibara.Models;
using Capibara.Net;
using Capibara.Test.Net;

using Moq;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace Capibara.Test.Models.FloorMapTest
{
    namespace RefreshTest
    {
        [TestFixture]
        public abstract class RefreshTestBase : TestFixtureBase
        {
            protected FloorMap model;

            protected bool IsRefreshSuccess { get; private set; }

            protected bool IsRefreshFail { get; private set; }

            protected virtual bool NeedEventHandler { get; } = true;

            [SetUp]
            public void Setup()
            {
                this.model = new FloorMap().BuildUp(this.GenerateUnityContainer());

                if (this.NeedEventHandler)
                {
                    this.model.RefreshSuccess += (sender, e) => this.IsRefreshSuccess = true;
                    this.model.RefreshFail += (sender, e) => this.IsRefreshFail = true;
                }

                // リフレッシュの終了を待機
                this.model.Refresh().Wait();
            }
        }

        [TestFixture]
        public class WhenUnauthorizedWithoutEventHandler : RefreshTestBase
        {
            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            protected override bool NeedEventHandler { get; } = false;

            [TestCase]
            public void ItShouldRefreshSuccessEventToNotOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenUnauthorized : RefreshTestBase
        {
            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            [TestCase]
            public void ItShouldRefreshSuccessEventToNotOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenEmptyWithoutEventHandler : RefreshTestBase
        {
            protected override bool NeedEventHandler { get; } = false;

            [TestCase]
            public void ItShouldRefreshSuccessEventToNotOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenEmpty : RefreshTestBase
        {
            [TestCase]
            public void ItShouldRoomsWithEmpty()
            {
                Assert.That(this.model.Rooms.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ItShouldRefreshSuccessEventToOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class When1 : RefreshTestBase
        {
            protected override string HttpStabResponse
                => "{\"rooms\": [{ \"name\": \"AAA\", \"capacity\": 10 }] }";

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new RoomComparer();
                var expect = new List<Room>() { new Room() { Name = "AAA", Capacity = 10 } };
                Assert.That(this.model.Rooms, Is.EqualTo(expect).Using(comparer));
            }

            [TestCase]
            public void ItShouldRefreshSuccessEventToOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class When10 : RefreshTestBase
        {
            protected override string HttpStabResponse
                => "{\"rooms\": [" +
                    "{ \"name\": \"AAA01\", \"capacity\": 11 }," +
                    "{ \"name\": \"AAA02\", \"capacity\": 12 }," +
                    "{ \"name\": \"AAA03\", \"capacity\": 13 }," +
                    "{ \"name\": \"AAA04\", \"capacity\": 14 }," +
                    "{ \"name\": \"AAA05\", \"capacity\": 15 }," +
                    "{ \"name\": \"AAA06\", \"capacity\": 16 }," +
                    "{ \"name\": \"AAA07\", \"capacity\": 17 }," +
                    "{ \"name\": \"AAA08\", \"capacity\": 18 }," +
                    "{ \"name\": \"AAA09\", \"capacity\": 19 }," +
                    "{ \"name\": \"AAA10\", \"capacity\": 20 }" +
                "] }";

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new RoomComparer();

                var expect = new List<Room>()
                {
                    new Room() { Name ="AAA01", Capacity = 11 },
                    new Room() { Name ="AAA02", Capacity = 12 },
                    new Room() { Name ="AAA03", Capacity = 13 },
                    new Room() { Name ="AAA04", Capacity = 14 },
                    new Room() { Name ="AAA05", Capacity = 15 },
                    new Room() { Name ="AAA06", Capacity = 16 },
                    new Room() { Name ="AAA07", Capacity = 17 },
                    new Room() { Name ="AAA08", Capacity = 18 },
                    new Room() { Name ="AAA09", Capacity = 19 },
                    new Room() { Name ="AAA10", Capacity = 20 },
                };
                
                Assert.That(this.model.Rooms, Is.EqualTo(expect).Using(comparer));
            }

            [TestCase]
            public void ItShouldRefreshSuccessEventToOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(false));
            }
        }
    }
}
