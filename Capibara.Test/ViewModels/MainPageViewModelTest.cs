using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Unity;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

using MenuItem = Capibara.ViewModels.MainPageViewModel.MenuItem;

namespace Capibara.Test.ViewModels.MainPageViewModelTest
{
    [TestFixture("FloorMapPage")]
    [TestFixture("MyProfilePage")]
    [TestFixture("SettingPage")]
    public class ItemTappedCommandTest : ViewModelTestBase
    {
        private string pagePath;

        public ItemTappedCommandTest(string pagePath)
        {
            this.pagePath = pagePath;
        }

        [SetUp]
        public void SetUp()
        {
            var viewModel = new MainPageViewModel(this.NavigationService);

            viewModel.ItemTappedCommand.Execute(new MenuItem { PagePath = this.pagePath} );

            while (!viewModel.ItemTappedCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToParticipantsPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo(this.pagePath));
        }
    }

    public class NicknamePropertyTest : ViewModelTestBase
    {
        protected MainPageViewModel Subject;

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();
            container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, new User() { Nickname = "xxxx"});

            this.Subject = new MainPageViewModel().BuildUp(container);
        }

        [TestCase]
        public void ItShouldValueWithExpect()
        {
            Assert.That(this.Subject.Nickname.Value, Is.EqualTo("xxxx"));
        }

        [TestCase]
        public void ItShouldUpdate()
        {
            this.Subject.CurrentUser.Nickname = "xxxx!!!";
            Assert.That(this.Subject.Nickname.Value, Is.EqualTo("xxxx!!!"));
        }
    }

    public class MenuItemsPropertyTest : ViewModelTestBase
    {
        protected MainPageViewModel Subject;

        [SetUp]
        public void SetUp()
        {
            this.Subject = new MainPageViewModel();
        }

        [TestCase]
        public void ItShouldCountExpected()
        {
            Assert.That(this.Subject.MenuItems.Count, Is.EqualTo(4));
        }
    }

    [TestFixture(0, "ホーム", "NavigationPage/FloorMapPage")]
    [TestFixture(1, "プロフィール", "NavigationPage/MyProfilePage")]
    [TestFixture(2, "お知らせ", "NavigationPage/InformationsPage")]
    [TestFixture(3, "設定", "NavigationPage/SettingPage")]
    public class MenuItemsItemPropertyTest : ViewModelTestBase
    {
        protected MainPageViewModel Subject;

        private int index;

        private string name;

        private string pagePath;

        public MenuItemsItemPropertyTest(int index, string name, string pagePath)
        {
            this.index = index;
            this.name = name;
            this.pagePath = pagePath;
        }

        [SetUp]
        public void SetUp()
        {
            this.Subject = new MainPageViewModel();
        }

        [TestCase]
        public void ItShouldFirstItemNameWithExpect()
        {
            Assert.That(this.Subject.MenuItems.ElementAtOrDefault(index)?.Name, Is.EqualTo(name));
        }

        [TestCase]
        public void ItShouldFirstItemPagePathWithExpect()
        {
            Assert.That(this.Subject.MenuItems.ElementAtOrDefault(index)?.PagePath, Is.EqualTo(pagePath));
        }
    }
}
