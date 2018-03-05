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
                    "{ \"id\": 1, \"name\": \"AAA01\", \"capacity\": 11 }," +
                    "{ \"id\": 2, \"name\": \"AAA02\", \"capacity\": 12 }," +
                    "{ \"id\": 3, \"name\": \"AAA03\", \"capacity\": 13 }," +
                    "{ \"id\": 4, \"name\": \"AAA04\", \"capacity\": 14 }," +
                    "{ \"id\": 5, \"name\": \"AAA05\", \"capacity\": 15 }," +
                    "{ \"id\": 6, \"name\": \"AAA06\", \"capacity\": 16 }," +
                    "{ \"id\": 7, \"name\": \"AAA07\", \"capacity\": 17 }," +
                    "{ \"id\": 8, \"name\": \"AAA08\", \"capacity\": 18 }," +
                    "{ \"id\": 9, \"name\": \"AAA09\", \"capacity\": 19 }," +
                    "{ \"id\": 10, \"name\": \"AAA10\", \"capacity\": 20 }" +
                "] }";

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new RoomComparer();

                var expect = new List<Room>()
                {
                    new Room() { Id = 1, Name ="AAA01", Capacity = 11 },
                    new Room() { Id = 2, Name ="AAA02", Capacity = 12 },
                    new Room() { Id = 3, Name ="AAA03", Capacity = 13 },
                    new Room() { Id = 4, Name ="AAA04", Capacity = 14 },
                    new Room() { Id = 5, Name ="AAA05", Capacity = 15 },
                    new Room() { Id = 6, Name ="AAA06", Capacity = 16 },
                    new Room() { Id = 7, Name ="AAA07", Capacity = 17 },
                    new Room() { Id = 8, Name ="AAA08", Capacity = 18 },
                    new Room() { Id = 9, Name ="AAA09", Capacity = 19 },
                    new Room() { Id = 10, Name ="AAA10", Capacity = 20 },
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
