using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Microsoft.Practices.Unity;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

using SettingItem = Capibara.ViewModels.SettingPageViewModel.SettingItem;

namespace Capibara.Test.ViewModels.SettingPageViewModelTest
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

            var viewModel = new SettingPageViewModel(navigationService.Object);

            viewModel.ItemTappedCommand.Execute(new SettingItem { PagePath = this.pagePath });

            while (!viewModel.ItemTappedCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToParticipantsPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo(this.pagePath));
        }
    }

    public class SettingItemsPropertyTest : ViewModelTestBase
    {
        protected SettingPageViewModel Actual;

        [SetUp]
        public void SetUp()
        {
            this.Actual = new SettingPageViewModel();
        }

        [TestCase]
        public void ItShouldCountWith3()
        {
            Assert.That(this.Actual.SettingItems.Count, Is.EqualTo(1));
        }
    }

    [TestFixture(0, "ブロックしたユーザー", "BlockUsersPage")]
    public class SettingItemsPropertyItemTest : ViewModelTestBase
    {
        protected SettingPageViewModel Actual;

        private int index;

        private string name;

        private string pagePath;

        public SettingItemsPropertyItemTest(int index, string name, string pagePath)
        {
            this.index = index;
            this.name = name;
            this.pagePath = pagePath;
        }

        [SetUp]
        public void SetUp()
        {
            this.Actual = new SettingPageViewModel();
        }

        [TestCase]
        public void ItShouldFirstItemNameWithExpect()
        {
            Assert.That(this.Actual.SettingItems.ElementAtOrDefault(index)?.Name, Is.EqualTo(name));
        }

        [TestCase]
        public void ItShouldFirstItemPagePathWithExpect()
        {
            Assert.That(this.Actual.SettingItems.ElementAtOrDefault(index)?.PagePath, Is.EqualTo(pagePath));
        }
    }
}
