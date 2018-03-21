using System;
using System.Net.Http;
using System.IO;

using Capibara.Models;
using Capibara.Net.Users;

using NUnit.Framework;

namespace Capibara.Test.Net.Users.UpdateRequestTest
{
    public class MethodTest
    {
        [TestCase]
        public void ItShouldExpect()
        {
            Assert.That(new UpdateRequest(new User()).Method, Is.EqualTo(HttpMethod.Put));
        }
    }

    public class PathsTest
    {
        [TestCase]
        public void ItShouldExpect()
        {
            Assert.That(new UpdateRequest(new User()).Paths, Is.EqualTo(new[] { "users" }));
        }
    }

    public class NeedAuthenticationTest
    {
        [TestCase]
        public void ItShouldExpect()
        {
            Assert.That(new UpdateRequest(new User()).NeedAuthentication, Is.EqualTo(true));
        }
    }

    public class NicknameTest
    {
        [TestCase]
        public void ItShouldExpect()
        {
            Assert.That(new UpdateRequest(new User { Nickname = "FooBar" }).Nickname, Is.EqualTo("FooBar"));
        }
    }

    public class BiographyTest
    {
        [TestCase]
        public void ItShouldExpect()
        {
            Assert.That(new UpdateRequest(new User { Biography = "FooBar" }).Biography, Is.EqualTo("FooBar"));
        }
    }

    public class BIconBase64Test
    {
        [TestCase("", null)]
        [TestCase(null, null)]
        [TestCase("1234567890", "data:image/png;base64,1234567890")]
        public void ItShouldExpect(string iconBase64, string expect)
        {
            var Subject = new UpdateRequest(new User { IconBase64 = iconBase64 });
            Assert.That(new UpdateRequest(new User { IconBase64 = iconBase64 }).IconBase64, Is.EqualTo(expect));
        }
    }

    namespace StringContentTest
    {
        public class WhenIconBase64IsEmpty
        {
            [TestCase]
            public void ItShouldExpect()
            {
                var request = new UpdateRequest(new User { Nickname = "FooBar", Biography = "Hi!" });
                var expect = "{\"nickname\": \"FooBar\", \"biography\": \"Hi!\", \"accepted\": false}".ToSlim();
                Assert.That(request.StringContent.ToSlim(), Is.EqualTo(expect));
            }
        }

        public class WhenIconBase64IsPresent
        {
            [TestCase]
            public void ItShouldExpect()
            {
                var request = new UpdateRequest(new User { Nickname = "FooBar", Biography = "Hi!", IconBase64 = "1234567890" });
                var expect = "{\"nickname\": \"FooBar\", \"biography\": \"Hi!\", \"accepted\": false, \"icon\": \"data:image/png;base64,1234567890\"}".ToSlim();
                Assert.That(request.StringContent.ToSlim(), Is.EqualTo(expect));
            }
        }
    }
}
