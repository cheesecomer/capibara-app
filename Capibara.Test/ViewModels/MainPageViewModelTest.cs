using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;
using Prism.Services;

using MenuItem = Capibara.ViewModels.MainPageViewModel.MenuItem;

namespace Capibara.Test.ViewModels.MainPageViewModelTest
{
    [TestFixture("FloorMapPage")]
    [TestFixture("MyProfilePage")]
    [TestFixture("SettingPage")]
    public class ItemTappedCommandTest : ViewModelTestBase
    {
        private string pagePath;

        protected string NavigatePageName { get; private set; }

        public ItemTappedCommandTest(string pagePath)
        {
            this.pagePath = pagePath;
        }

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            var navigateTaskSource = new TaskCompletionSource<bool>();
            var navigationService = new Mock<INavigationService>();
            navigationService
                .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                .Returns(navigateTaskSource.Task)
                .Callback((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                {
                    this.NavigatePageName = name;
                    navigateTaskSource.SetResult(true);
                });

            var viewModel = new MainPageViewModel(navigationService.Object);

            viewModel.ItemTappedCommand.Execute(new MenuItem { PagePath = this.pagePath} );

            while (!viewModel.ItemTappedCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToParticipantsPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo(this.pagePath));
        }
    }

    public class MenuItemsPropertyTest : ViewModelTestBase
    {
        protected MainPageViewModel actual;

        [SetUp]
        public void SetUp()
        {
            this.actual = new MainPageViewModel();
        }

        [TestCase]
        public void ItShouldCountWith3()
        {
            Assert.That(this.actual.MenuItems.Count, Is.EqualTo(3));
        }
    }

    [TestFixture(0, "ホーム", "NavigationPage/FloorMapPage")]
    [TestFixture(1, "プロフィール", "NavigationPage/MyProfilePage")]
    [TestFixture(2, "設定", "NavigationPage/SettingPage")]
    public class MenuItemsItemPropertyTest : ViewModelTestBase
    {
        protected MainPageViewModel actual;

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
            this.actual = new MainPageViewModel();
        }

        [TestCase]
        public void ItShouldFirstItemNameWithExpect()
        {
            Assert.That(this.actual.MenuItems.ElementAtOrDefault(index)?.Name, Is.EqualTo(name));
        }

        [TestCase]
        public void ItShouldFirstItemPagePathWithExpect()
        {
            Assert.That(this.actual.MenuItems.ElementAtOrDefault(index)?.PagePath, Is.EqualTo(pagePath));
        }
    }
}
