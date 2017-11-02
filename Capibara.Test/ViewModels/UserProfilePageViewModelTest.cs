using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Microsoft.Practices.Unity;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

namespace Capibara.Test.ViewModels.UserProfilePageViewModelTest
{
    namespace NicknamePropertyTest
    {
        public class WhenOwn : ViewModelTestBase
        {
            protected User Model;

            protected UserProfilePageViewModel Actual;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();
                this.Model = new User() { Nickname = "xxxx", Id = Guid.NewGuid().ToInt() }.BuildUp(container);
                this.IsolatedStorage.UserId = this.Model.Id;

                this.Actual = new UserProfilePageViewModel(model: this.Model).BuildUp(container);
            }

            [TestCase]
            public void ItShouldValueWithExpect()
            {
                Assert.That(this.Actual.Nickname.Value, Is.EqualTo("xxxx"));
            }

            [TestCase]
            public void ItShouldUpdate()
            {
                this.Actual.Model.Nickname = "xxxx!!!";
                Assert.That(this.Actual.Nickname.Value, Is.EqualTo("xxxx!!!"));
            }
        }
    }

    namespace BiographyPropertyTest
    {
        public class WhenOwn : ViewModelTestBase
        {
            protected User Model;

            protected UserProfilePageViewModel Actual;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();
                this.Model = new User() { Biography = "xxxx", Id = Guid.NewGuid().ToInt() }.BuildUp(container);
                this.IsolatedStorage.UserId = this.Model.Id;

                this.Actual = new UserProfilePageViewModel(model: this.Model).BuildUp(container);
            }

            [TestCase]
            public void ItShouldValueWithExpect()
            {
                Assert.That(this.Actual.Biography.Value, Is.EqualTo("xxxx"));
            }

            [TestCase]
            public void ItShouldUpdate()
            {
                this.Actual.Model.Biography = "xxxx!!!";
                Assert.That(this.Actual.Biography.Value, Is.EqualTo("xxxx!!!"));
            }
        }
    }
}
