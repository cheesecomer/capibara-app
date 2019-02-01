using System;
using System.Collections.Generic;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

using Subject = Capibara.Net.Rooms.IndexResponse;

namespace Capibara.Test.Net.Rooms
{
    public class IndexResponseTest
    {
        public class DeserializeTest
        {
            [TestCase]
            public void ItShouldExpected()
            {
                var json = "{ \"rooms\": [" +
                    "{ \"id\":  1, \"name\": \"AAA01\", \"capacity\": 11}," +
                    "{ \"id\":  2, \"name\": \"AAA02\", \"capacity\": 12}," +
                    "{ \"id\":  3, \"name\": \"AAA03\", \"capacity\": 13}," +
                    "{ \"id\":  4, \"name\": \"AAA04\", \"capacity\": 14}," +
                    "{ \"id\":  5, \"name\": \"AAA05\", \"capacity\": 15}," +
                    "{ \"id\":  6, \"name\": \"AAA06\", \"capacity\": 16}," +
                    "{ \"id\":  7, \"name\": \"AAA07\", \"capacity\": 17}," +
                    "{ \"id\":  8, \"name\": \"AAA08\", \"capacity\": 18}," +
                    "{ \"id\":  9, \"name\": \"AAA09\", \"capacity\": 19}," +
                    "{ \"id\": 10, \"name\": \"AAA10\", \"capacity\": 20}" +
                    "]}";

                var actual = JsonConvert.DeserializeObject<Subject>(json);
                var comparer = new RoomComparer();
                var expect = new List<Room>
                {
                    new Room { Id = 1, Name ="AAA01", Capacity = 11 },
                    new Room { Id = 2, Name ="AAA02", Capacity = 12 },
                    new Room { Id = 3, Name ="AAA03", Capacity = 13 },
                    new Room { Id = 4, Name ="AAA04", Capacity = 14 },
                    new Room { Id = 5, Name ="AAA05", Capacity = 15 },
                    new Room { Id = 6, Name ="AAA06", Capacity = 16 },
                    new Room { Id = 7, Name ="AAA07", Capacity = 17 },
                    new Room { Id = 8, Name ="AAA08", Capacity = 18 },
                    new Room { Id = 9, Name ="AAA09", Capacity = 19 },
                    new Room { Id = 10, Name ="AAA10", Capacity = 20 },
                };
                Assert.That(actual.Rooms, Is.EqualTo(expect).Using(comparer));
            }
        }
    }
}
