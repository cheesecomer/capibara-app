using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;
using Prism.Services;

namespace Capibara.Test.ViewModels.InformationsPageViewModelTest
{
    namespace RefreshCommandTest
    {
        public abstract class WhenSuccessBase : ViewModelTestBase
        {
            protected InformationsPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                this.ViewModel = new InformationsPageViewModel();

                this.ViewModel.BuildUp(container);

                this.ViewModel.RefreshCommand.Execute();

                while (!this.ViewModel.RefreshCommand.CanExecute()) { };
            }

            [TestCase]
            public void ItShouldShowDialog()
            {
                Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSuccess10 : WhenSuccessBase
        {
            protected override string HttpStabResponse
            => "{\"informations\": [" +
                    "{ \"id\":  1, \"title\": \"Title0001\", \"message\": \"Message0001\", \"published_at\": \"2017-01-28T20:25:20.000+09:00\"}," +
                    "{ \"id\":  2, \"title\": \"Title0002\", \"message\": \"Message0002\", \"published_at\": \"2017-02-28T20:25:20.000+09:00\"}," +
                    "{ \"id\":  3, \"title\": \"Title0003\", \"message\": \"Message0003\", \"published_at\": \"2017-03-28T20:25:20.000+09:00\"}," +
                    "{ \"id\":  4, \"title\": \"Title0004\", \"message\": \"Message0004\", \"published_at\": \"2017-04-28T20:25:20.000+09:00\"}," +
                    "{ \"id\":  5, \"title\": \"Title0005\", \"message\": \"Message0005\", \"published_at\": \"2017-05-28T20:25:20.000+09:00\"}," +
                    "{ \"id\":  6, \"title\": \"Title0006\", \"message\": \"Message0006\", \"published_at\": \"2017-06-28T20:25:20.000+09:00\"}," +
                    "{ \"id\":  7, \"title\": \"Title0007\", \"message\": \"Message0007\", \"published_at\": \"2017-07-28T20:25:20.000+09:00\"}," +
                    "{ \"id\":  8, \"title\": \"Title0008\", \"message\": \"Message0008\", \"published_at\": \"2017-08-28T20:25:20.000+09:00\"}," +
                    "{ \"id\":  9, \"title\": \"Title0009\", \"message\": \"Message0009\", \"published_at\": \"2017-09-28T20:25:20.000+09:00\"}," +
                    "{ \"id\": 10, \"title\": \"Title0010\", \"message\": \"Message0010\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\"}" +
                "] }";

            [TestCase]
            public void ItShouldInformationsWithExpected()
            {
                var comparer = new InformationComparer();
                var expect = new List<Information>
                {
                    new Information { Id = 01, Title = "Title0001", Message = "Message0001", PublishedAt = new DateTimeOffset(2017, 01, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 02, Title = "Title0002", Message = "Message0002", PublishedAt = new DateTimeOffset(2017, 02, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 03, Title = "Title0003", Message = "Message0003", PublishedAt = new DateTimeOffset(2017, 03, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 04, Title = "Title0004", Message = "Message0004", PublishedAt = new DateTimeOffset(2017, 04, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 05, Title = "Title0005", Message = "Message0005", PublishedAt = new DateTimeOffset(2017, 05, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 06, Title = "Title0006", Message = "Message0006", PublishedAt = new DateTimeOffset(2017, 06, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 07, Title = "Title0007", Message = "Message0007", PublishedAt = new DateTimeOffset(2017, 07, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 08, Title = "Title0008", Message = "Message0008", PublishedAt = new DateTimeOffset(2017, 08, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 09, Title = "Title0009", Message = "Message0009", PublishedAt = new DateTimeOffset(2017, 09, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 10, Title = "Title0010", Message = "Message0010", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) }
                };
                Assert.That(this.ViewModel.Informations, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenSuccess1 : WhenSuccessBase
        {
            protected override string HttpStabResponse
            => "{\"informations\": [" +
                    "{ \"id\": 1, \"title\": \"Title0001\", \"message\": \"Message0001\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\"}" +
                "] }";

            [TestCase]
            public void ItShouldInformationsWithExpected()
            {
                var comparer = new InformationComparer();
                var expect = new List<Information>
                {
                    new Information { Id = 1, Title = "Title0001", Message = "Message0001", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) }
                };
                Assert.That(this.ViewModel.Informations, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenUnauthorizedWithService : ViewModelTestBase
        {
            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            protected string NavigatePageName { get; private set; }

            protected bool IsShowDialog { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var navigationService = new Mock<INavigationService>();
                navigationService
                    .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                    .Returns((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                    {
                        this.NavigatePageName = name;
                        return Task.Run(() => { });
                    });


                var pageDialogService = new Mock<IPageDialogService>();
                pageDialogService
                    .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.Run(() => true))
                    .Callback(() => this.IsShowDialog = true);

                var viewModel = new FloorMapPageViewModel(
                    navigationService.Object,
                    pageDialogService.Object);

                viewModel.BuildUp(container);

                viewModel.RefreshCommand.Execute();

                while (!viewModel.RefreshCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldShowDialog()
            {
                Assert.That(this.IsShowDialog, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNavigateToLogin()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo("/SignInPage"));
            }
        }

        public class InformationComparer : IEqualityComparer<Information>
        {
            public bool Equals(Information x, Information y)
            {
                if (x == null && y == null)
                    return true;
                else if (x == null | y == null)
                    return false;
                else
                    return 
                        x.Id == y.Id 
                     && x.Title == y.Title 
                     && x.Message == y.Message 
                     && x.PublishedAt == y.PublishedAt;
            }

            public int GetHashCode(Information value)
            {
                return value.GetHashCode();
            }
        }
    }
}
