using System;
namespace Faker
{
    public static class Time
    {
        static readonly Random random = new Random();

        public static DateTime DateTime()
        {
            var from = new DateTime(1995, 1, 1);
            var range = System.DateTime.Now - from;

            var randTimeSpan = new TimeSpan((long)(random.NextDouble() * range.Ticks));

            return from + randTimeSpan;
        }

        public static DateTimeOffset DateTimeOffset()
        {
            return System.DateTime.SpecifyKind(DateTime(), DateTimeKind.Utc);
        }
    }
}
