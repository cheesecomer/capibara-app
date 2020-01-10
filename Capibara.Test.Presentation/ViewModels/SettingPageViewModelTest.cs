#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.Reactive;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Prism.Navigation;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class SettingPageViewModelTest
    {
        #region SettingItem

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

        #endregion

        #region

        public class RefreshCommandTestSubject
        {
            public TestScheduler Scheduler { get; private set; }

            public SettingPageViewModel ViewModel { get; private set; }

            public Mock<IGetWebPageUrlUseCase> GetWebPageUrlUseCase { get; private set; }

            public string WebPageUrl { get; private set; }

            public Mock<NavigationService> NavigationService { get; private set; }

            public RefreshCommandTestSubject()
            {
                var schedulerProvider = new SchedulerProvider();
                var navigationService = Mock.NavigationService();
                var useCase = new Mock<IGetWebPageUrlUseCase>();
                var url = Faker.Url.Root();
                var viewModel = new SettingPageViewModel(navigationService.Object)
                {
                    GetWebPageUrlUseCase = useCase.Object,
                    SchedulerProvider = schedulerProvider
                };

                useCase.Setup(x => x.Invoke(It.IsAny<WebPage>())).ReturnsObservable(url);

                this.WebPageUrl = url;
                this.Scheduler = schedulerProvider.Scheduler;
                this.ViewModel = viewModel;
                this.GetWebPageUrlUseCase = useCase;
                this.NavigationService = navigationService;
            }
        }

        public static IEnumerable ItemTappedCommandTest_TestCaseSource()
        {

            yield return
                new TestCaseData(0, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .GetWebPageUrlUseCase
                        .Verify(x => x.Invoke(It.IsAny<WebPage>()), Times.Never());
                })).SetName("ItemTappedCommand.Execute(SettingItems[0]) should not invoke use case");

            yield return
                new TestCaseData(0, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .NavigationService
                        .Verify(
                            x =>
                                x.NavigateAsync(
                                    "BlockUsersPage",
                                    null,
                                    null,
                                    true),
                            Times.Once());
                })).SetName("ItemTappedCommand.Execute(SettingItems[0]) should navigate to BlockUsersPage");

            yield return
                new TestCaseData(1, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .GetWebPageUrlUseCase
                        .Verify(x => x.Invoke(WebPage.Terms), Times.Once());
                })).SetName("ItemTappedCommand.Execute(SettingItems[1]) should invoke use case");

            yield return
                new TestCaseData(1, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .NavigationService
                        .Verify(
                            x =>
                                x.NavigateAsync(
                                    "WebViewPage",
                                    It.Is<NavigationParameters>(parameters =>
                                        parameters.Count == 2 &&
                                        parameters[ParameterNames.Title].Equals("利用規約") &&
                                        parameters[ParameterNames.Url].Equals(subject.WebPageUrl)),
                                    null,
                                    true),
                            Times.Once());
                })).SetName("ItemTappedCommand.Execute(SettingItems[1]) should navigate to 利用規約 on WebViewPage");

            yield return
                new TestCaseData(2, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .GetWebPageUrlUseCase
                        .Verify(x => x.Invoke(WebPage.PrivacyPolicy), Times.Once());
                })).SetName("ItemTappedCommand.Execute(SettingItems[2]) should invoke use case");

            yield return
                new TestCaseData(2, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .NavigationService
                        .Verify(
                            x =>
                                x.NavigateAsync(
                                    "WebViewPage",
                                    It.Is<NavigationParameters>(parameters =>
                                        parameters.Count == 2 &&
                                        parameters[ParameterNames.Title].Equals("プライバシーポリシー") &&
                                        parameters[ParameterNames.Url].Equals(subject.WebPageUrl)),
                                    null,
                                    true),
                            Times.Once());
                })).SetName("ItemTappedCommand.Execute(SettingItems[2]) should navigate to プライバシーポリシー on WebViewPage");


            yield return
                new TestCaseData(3, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .GetWebPageUrlUseCase
                        .Verify(x => x.Invoke(It.IsAny<WebPage>()), Times.Never());
                })).SetName("ItemTappedCommand.Execute(SettingItems[3]) should not invoke use case");

            yield return
                new TestCaseData(3, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .NavigationService
                        .Verify(
                            x =>
                                x.NavigateAsync(
                                    "InquiryPage",
                                    null,
                                    null,
                                    true),
                            Times.Once());
                })).SetName("ItemTappedCommand.Execute(SettingItems[3]) should navigate to InquiryPage");

            yield return
                new TestCaseData(4, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .GetWebPageUrlUseCase
                        .Verify(x => x.Invoke(It.IsAny<WebPage>()), Times.Never());
                })).SetName("ItemTappedCommand.Execute(SettingItems[4]) should not invoke use case");

            yield return
                new TestCaseData(4, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .NavigationService
                        .Verify(
                            x =>
                                x.NavigateAsync(
                                    "AboutPage",
                                    null,
                                    null,
                                    true),
                            Times.Once());
                })).SetName("ItemTappedCommand.Execute(SettingItems[4]) should navigate to AboutPage");

            yield return
                new TestCaseData(5, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .GetWebPageUrlUseCase
                        .Verify(x => x.Invoke(It.IsAny<WebPage>()), Times.Never());
                })).SetName("ItemTappedCommand.Execute(SettingItems[5]) should not invoke use case");

            yield return
                new TestCaseData(5, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .NavigationService
                        .Verify(
                            x =>
                                x.NavigateAsync(
                                    "LicensePage",
                                    null,
                                    null,
                                    true),
                            Times.Once());
                })).SetName("ItemTappedCommand.Execute(SettingItems[5]) should navigate to LicensePage");

            yield return
                new TestCaseData(6, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .GetWebPageUrlUseCase
                        .Verify(x => x.Invoke(It.IsAny<WebPage>()), Times.Never());
                })).SetName("ItemTappedCommand.Execute(SettingItems[6]) should not invoke use case");

            yield return
                new TestCaseData(6, 2, new Action<RefreshCommandTestSubject>(subject =>
                {
                    subject
                        .NavigationService
                        .Verify(
                            x =>
                                x.NavigateAsync(
                                    "UnsubscribePage",
                                    null,
                                    null,
                                    true),
                            Times.Once());
                })).SetName("ItemTappedCommand.Execute(SettingItems[6]) should navigate to UnsubscribePage");
        }

        [Test]
        [TestCaseSource("ItemTappedCommandTest_TestCaseSource")]
        public void ItemTappedCommand_Test(int index, int advanceTime, Action<RefreshCommandTestSubject> assert)
        {
            var subject = new RefreshCommandTestSubject();

            subject.ViewModel.ItemTappedCommand.Execute(subject.ViewModel.SettingItems[index]);

            subject.Scheduler.AdvanceBy(advanceTime);

            assert(subject);
        }

        #endregion
    }
}
