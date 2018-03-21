using System.Linq;
using NUnit.Framework;

using Prism.Navigation;

using ParameterNames = Capibara.ViewModels.ParameterNames;
using SubjectViewModel = Capibara.ViewModels.SettingPageViewModel;
using SettingItem = Capibara.ViewModels.SettingPageViewModel.SettingItem;

namespace Capibara.Test.ViewModels.SettingPageViewModel
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

            var viewModel = new SubjectViewModel(this.NavigationService);

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
        protected SubjectViewModel Subject;

        [SetUp]
        public void SetUp()
        {
            this.Subject = new SubjectViewModel();
        }

        [TestCase]
        public void ItShouldCountWithExpected()
        {
            Assert.That(this.Subject.SettingItems.Count, Is.EqualTo(3));
        }
    }

    [TestFixtureSource("FixtureArgs")]
    public class SettingItemsPropertyItemTest : ViewModelTestBase
    {
        static object[] FixtureArgs = {
            new object[] { 0, "ブロック中のユーザー", "BlockUsersPage", null },
            new object[] { 1, "プライバシーポリシー", "WebViewPage", new NavigationParameters {
                    { ParameterNames.Url, "http://localhost:9999/privacy_policy" },
                    { ParameterNames.Title, "プライバシーポリシー" }
                } },
            new object[] { 2, "退会する", "UnsubscribePage", null }
        };

        protected SubjectViewModel Subject;

        private int index;

        private string name;

        private string pagePath;

        private NavigationParameters parameters;

        public SettingItemsPropertyItemTest(int index, string name, string pagePath, NavigationParameters parameters)
        {
            this.index = index;
            this.name = name;
            this.pagePath = pagePath;
            this.parameters = parameters;
        }

        [SetUp]
        public void SetUp()
        {
            this.Subject = new SubjectViewModel().BuildUp(this.GenerateUnityContainer());
        }

        [TestCase]
        public void ItShouldItemNameWithExpect()
        {
            Assert.That(this.Subject.SettingItems.ElementAtOrDefault(index)?.Name, Is.EqualTo(name));
        }

        [TestCase]
        public void ItShouldItemPagePathWithExpect()
        {
            Assert.That(this.Subject.SettingItems.ElementAtOrDefault(index)?.PagePath, Is.EqualTo(pagePath));
        }

        [TestCase]
        public void ItShouldItemParametersWithExpect()
        {
            Assert.That(this.Subject.SettingItems.ElementAtOrDefault(index)?.Parameters, Is.EqualTo(parameters));
        }
    }
}
