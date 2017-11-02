using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Microsoft.Practices.Unity;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

namespace Capibara.Test.ViewModels.MyProfilePageViewModelTest
{
    public class NicknamePropertyTest : ViewModelTestBase
    {
        protected MyProfilePageViewModel Actual;

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();
            container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, new User() { Nickname = "xxxx" });

            this.Actual = new MyProfilePageViewModel().BuildUp(container);
        }

        [TestCase]
        public void ItShouldValueWithExpect()
        {
            Assert.That(this.Actual.Nickname.Value, Is.EqualTo("xxxx"));
        }

        [TestCase]
        public void ItShouldUpdate()
        {
            this.Actual.CurrentUser.Nickname = "xxxx!!!";
            Assert.That(this.Actual.Nickname.Value, Is.EqualTo("xxxx!!!"));
        }
    }
}
