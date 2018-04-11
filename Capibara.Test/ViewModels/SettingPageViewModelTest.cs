using System.Linq;
using NUnit.Framework;

using Moq;

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
        public override void SetUp()
        {
            base.SetUp();
        }
        
        [TestCase("FloorMapPage")]
        [TestCase("MyProfilePage")]
        [TestCase("SettingPage")]
        public void ItShouldNavigateToParticipantsPage(string pagePath)
        {
            var viewModel = new SubjectViewModel(this.NavigationService.Object);

            viewModel.ItemTappedCommand.Execute(new SettingItem { PagePath = pagePath });

            while (!viewModel.ItemTappedCommand.CanExecute()) { }

            this.NavigationService.Verify(x => x.NavigateAsync(pagePath, null), Times.Once());
        }
    }

    public class SettingItemsPropertyTest : ViewModelTestBase
    {
        protected SubjectViewModel Subject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Subject = new SubjectViewModel();
        }

        [TestCase]
        public void ItShouldCountWithExpected()
        {
            Assert.That(this.Subject.SettingItems.Count, Is.EqualTo(7));
        }
    }

    [TestFixtureSource("FixtureArgs")]
    public class SettingItemsPropertyItemTest : ViewModelTestBase
    {
        static object[] FixtureArgs = {
            new object[] { 0, "ブロック中のユーザー", "BlockUsersPage", null },
            new object[] { 1, "利用規約", "WebViewPage", new NavigationParameters {
                    { ParameterNames.Url, "http://localhost:9999/terms" },
                    { ParameterNames.Title, "利用規約" }
                } },
            new object[] { 2, "プライバシーポリシー", "WebViewPage", new NavigationParameters {
                    { ParameterNames.Url, "http://localhost:9999/privacy_policy" },
                    { ParameterNames.Title, "プライバシーポリシー" }
                } },
            new object[] { 3, "お問い合わせ", "InquiryPage", null },
            new object[] { 4, "バージョン情報", "AboutPage", null },
            new object[] { 5, "ライセンス", "LicensePage", null },
            new object[] { 6, "退会する", "UnsubscribePage", null },
        };

        protected SubjectViewModel Subject;

        private int Index { get; }

        private string Name { get; }

        private string PagePath { get; }

        private NavigationParameters parameters;

        public SettingItemsPropertyItemTest(int index, string name, string pagePath, NavigationParameters parameters)
        {
            this.Index = index;
            this.Name = name;
            this.PagePath = pagePath;
            this.parameters = parameters;
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Subject = new SubjectViewModel().BuildUp(this.Container);
        }

        [TestCase]
        public void ItShouldItemNameWithExpect()
        {
            Assert.That(this.Subject.SettingItems.ElementAtOrDefault(Index)?.Name, Is.EqualTo(Name));
        }

        [TestCase]
        public void ItShouldItemPagePathWithExpect()
        {
            Assert.That(this.Subject.SettingItems.ElementAtOrDefault(Index)?.PagePath, Is.EqualTo(PagePath));
        }

        [TestCase]
        public void ItShouldItemParametersWithExpect()
        {
            Assert.That(this.Subject.SettingItems.ElementAtOrDefault(Index)?.Parameters, Is.EqualTo(parameters));
        }
    }
}
