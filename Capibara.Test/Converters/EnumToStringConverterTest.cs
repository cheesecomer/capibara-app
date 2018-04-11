using System;
using System.Collections.Generic;
using NUnit.Framework;

using Capibara.Models;
using System.Linq;

using Xamarin.Forms;

using SubjectClass = Capibara.Converters.EnumToStringConverter;

namespace Capibara.Test.Converters.EnumToStringConverter
{
    namespace ConvertTest
    {
        public class WhenNull
        {
            public void ItShouldResultIsExpect()
            {
                var actual = new SubjectClass().Convert(null, null, null, null) as IEnumerable<string>;
                Assert.That(actual, Is.EqualTo(null));
            }
        }

        public class WhenNotIEnumerable
        {
            public void ItShouldResultIsExpect()
            {
                var actual = new SubjectClass().Convert(1, null, null, null) as IEnumerable<string>;
                Assert.That(actual, Is.EqualTo(1));
            }
        }

        public class WhenNotDisplayNameList
        {
            [TestCase(0, "Local")]
            [TestCase(1, "Unspecified")]
            [TestCase(2, "Utc")]
            public void ItShouldResultIsExpect(int index, string expect)
            {
                var collection = new List<DateTimeKind> { DateTimeKind.Local, DateTimeKind.Unspecified, DateTimeKind.Utc };
                var actual = new SubjectClass().Convert(collection, null, null, null) as IEnumerable<string>;
                Assert.That(actual?.ElementAtOrDefault(index), Is.EqualTo(expect));
            }
        }

        public class WhenReportReasonList
        {
            [TestCase(0, "その他")]
            [TestCase(1, "スパム")]
            public void ItShouldResultIsExpect(int index, string expect)
            {
                var collection = new List<ReportReason> { ReportReason.Other, ReportReason.Spam };
                var actual = new SubjectClass().Convert(collection, null, null, null) as IEnumerable<string>;
                Assert.That(actual?.ElementAtOrDefault(index), Is.EqualTo(expect));
            }
        }
    }

    public class ConvertBack
    {
        [TestCase]
        public void ItShouldThrowNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => new SubjectClass().ConvertBack(null, null, null, null));
        }
    }
}
