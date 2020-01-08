#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System.Collections;
using System.Reactive;
using Capibara.Domain.UseCases;
using Moq;
using NUnit.Framework;
using Prism.Navigation;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class SettingPageViewModelTest
    {
        [Test(Description = "リストの要素数が７であること")]
        public void SettingItem_Should7Items()
        {
            Assert.That(new SettingPageViewModel().SettingItems.Count, Is.EqualTo(7));
        }

        [Test]
        [TestCase(0, "ブロック中のユーザー", TestName = "SettingItems[{0}].Name should be {1}")]
        [TestCase(1, "利用規約", TestName = "SettingItems[{0}].Name should be {1}")]
        [TestCase(2, "プライバシーポリシー", TestName = "SettingItems[{0}].Name should be {1}")]
        [TestCase(3, "お問い合わせ", TestName = "SettingItems[{0}].Name should be {1}")]
        [TestCase(4, "バージョン情報", TestName = "SettingItems[{0}].Name should be {1}")]
        [TestCase(5, "ライセンス", TestName = "SettingItems[{0}].Name should be {1}")]
        [TestCase(6, "退会する", TestName = "SettingItems[{0}].Name should be {1}")]
        public void SettingItem_ShouldExpectedName(int index, string expect)
        {
            Assert.That(new SettingPageViewModel().SettingItems[index].Name, Is.EqualTo(expect));
        }

        [Test]
        [TestCase(0, "BlockUsersPage", TestName = "SettingItems[{0}].PagePath should be {1}")]
        [TestCase(1, "WebViewPage", TestName = "SettingItems[{0}].PagePath should be {1}")]
        [TestCase(2, "WebViewPage", TestName = "SettingItems[{0}].PagePath should be {1}")]
        [TestCase(3, "InquiryPage", TestName = "SettingItems[{0}].PagePath should be {1}")]
        [TestCase(4, "AboutPage", TestName = "SettingItems[{0}].PagePath should be {1}")]
        [TestCase(5, "LicensePage", TestName = "SettingItems[{0}].PagePath should be {1}")]
        [TestCase(6, "UnsubscribePage", TestName = "SettingItems[{0}].PagePath should be {1}")]
        public void SettingItem_ShouldExpectePath(int index, string expect)
        {
            Assert.That(new SettingPageViewModel().SettingItems[index].PagePath, Is.EqualTo(expect));
        }

        [Test]
        public void RefreshCommand_ShouldExecuteUseCase()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var fetchEnvironmentUseCase = new Mock<IFetchEnvironmentUseCase>();
            var subject = new SettingPageViewModel
            {
                FetchEnvironmentUseCase = fetchEnvironmentUseCase.Object,
                SchedulerProvider = schedulerProvider
            };

            fetchEnvironmentUseCase.Setup(x => x.Invoke()).ReturnsObservable(new Mock<Domain.Models.IEnvironment>().Object);
            subject.RefreshCommand.Execute();
            scheduler.AdvanceBy(1);

            fetchEnvironmentUseCase.Verify(x => x.Invoke(), Times.Once);
        }

        [Test]
        [TestCaseSource("RefreshCommand_ShouldAssignPageParams_TestCase")]
        public void RefreshCommand_ShouldAssignPageParams(int index, NavigationParameters expect)
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var environment = new Mock<Domain.Models.IEnvironment>();
            var fetchEnvironmentUseCase = new Mock<IFetchEnvironmentUseCase>();
            var subject = new SettingPageViewModel
            {

                FetchEnvironmentUseCase = fetchEnvironmentUseCase.Object,
                SchedulerProvider = schedulerProvider
            };

            environment.SetupGet(x => x.PrivacyPolicyUrl).Returns(privacyPolicyUrl);
            environment.SetupGet(x => x.TermsUrl).Returns(termsUrl);

            fetchEnvironmentUseCase.Setup(x => x.Invoke()).ReturnsObservable(environment.Object);

            subject.RefreshCommand.Execute();
            scheduler.AdvanceBy(1);
            scheduler.AdvanceBy(1);

            Assert.That(subject.SettingItems[index].Parameters, Is.EqualTo(expect));
        }

        private static string privacyPolicyUrl = Faker.Url.Root();

        private static string termsUrl = Faker.Url.Root();

        private static IEnumerable RefreshCommand_ShouldAssignPageParams_TestCase()
        {
            yield return new TestCaseData(0, null).SetName("SettingItems[{0}].PagePath should be {1}");
            yield return new TestCaseData(
                1,
                new NavigationParameters
                {
                    { ParameterNames.Url, termsUrl },
                    { ParameterNames.Title, "利用規約" }
                })
                .SetName("SettingItems[{0}].PagePath should be { { ParameterNames.Title, \"利用規約\" }, { ParameterNames.Url, Environment.TermsUrl } }");
            yield return new TestCaseData(
                2,
                new NavigationParameters
                {
                    { ParameterNames.Url, privacyPolicyUrl },
                    { ParameterNames.Title, "プライバシーポリシー" }
                })
                .SetName("SettingItems[{0}].PagePath should be { { ParameterNames.Title, \"プライバシーポリシー\" }, { ParameterNames.Url, Environment.PrivacyPolicyUrl } }");
            yield return new TestCaseData(3, null).SetName("SettingItems[{0}].PagePath should be {1}");
            yield return new TestCaseData(4, null).SetName("SettingItems[{0}].PagePath should be {1}");
            yield return new TestCaseData(5, null).SetName("SettingItems[{0}].PagePath should be {1}");
            yield return new TestCaseData(6, null).SetName("SettingItems[{0}].PagePath should be {1}");
        }
    }

}
