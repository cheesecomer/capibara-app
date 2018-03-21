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

        public ItemTappedCommandTest(string pagePath)
        {
            this.pagePath = pagePath;
        }

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            var viewModel = new SettingPageViewModel(this.NavigationService);

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
        protected SettingPageViewModel Subject;

        [SetUp]
        public void SetUp()
        {
            this.Subject = new SettingPageViewModel();
        }

        [TestCase]
        public void ItShouldCountWithExpected()
        {
            Assert.That(this.Subject.SettingItems.Count, Is.EqualTo(2));
        }
    }

    [TestFixture(0, "ブロック中のユーザー", "BlockUsersPage")]
    [TestFixture(1, "退会する", "UnsubscribePage")]
    public class SettingItemsPropertyItemTest : ViewModelTestBase
    {
        protected SettingPageViewModel Subject;

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
            this.Subject = new SettingPageViewModel();
        }

        [TestCase]
        public void ItShouldFirstItemNameWithExpect()
        {
            Assert.That(this.Subject.SettingItems.ElementAtOrDefault(index)?.Name, Is.EqualTo(name));
        }

        [TestCase]
        public void ItShouldFirstItemPagePathWithExpect()
        {
            Assert.That(this.Subject.SettingItems.ElementAtOrDefault(index)?.PagePath, Is.EqualTo(pagePath));
        }
    }
}
