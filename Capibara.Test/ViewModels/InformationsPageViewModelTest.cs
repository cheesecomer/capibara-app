using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;
using Capibara.Net;
using Capibara.Net.Informations;

using Moq;
using Moq.Protected;
using NUnit.Framework;

using Prism.Navigation;

using SubjectViewModel = Capibara.ViewModels.InformationsPageViewModel;

namespace Capibara.Test.ViewModels.InformationsPageViewModel
{
    namespace RefreshCommandTest
    {
        public abstract class WhenSuccessBase : ViewModelTestBase
        {
            protected SubjectViewModel Subject { get; private set; }

            protected virtual IndexResponse Response { get; } = new IndexResponse();

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var request = new Mock<RequestBase<IndexResponse>>();
                request.Setup(x => x.Execute()).ReturnsAsync(this.Response);

                this.RequestFactory.Setup(x => x.InformationsIndexRequest()).Returns(request.Object);

                this.Subject = new SubjectViewModel();

                this.Subject.BuildUp(this.Container);

                this.Subject.RefreshCommand.Execute();

                while (!this.Subject.RefreshCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldShowDialog()
            {
                this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
            }
        }

        [TestFixture]
        public class WhenSuccess10 : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Informations =
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
                }
            };

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
                Assert.That(this.Subject.Informations, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenSuccess1 : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Informations = { new Information { Id = 1, Title = "Title0001", Message = "Message0001", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) } }
            };

            [TestCase]
            public void ItShouldInformationsWithExpected()
            {
                var comparer = new InformationComparer();
                var expect = new List<Information>
                {
                    new Information { Id = 1, Title = "Title0001", Message = "Message0001", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) }
                };
                Assert.That(this.Subject.Informations, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenSuccessWithDuplicated : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Informations =
                {
                    new Information { Id = 1, Title = "Title0001a", Message = "Message0001a", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) },
                    new Information { Id = 1, Title = "Title0001b", Message = "Message0001b", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) }
                }
            };

            [TestCase]
            public void ItShouldInformationsWithExpected()
            {
                var comparer = new InformationComparer();
                var expect = new List<Information>
                {
                    new Information { Id = 1, Title = "Title0001b", Message = "Message0001b", PublishedAt = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)) }
                };
                Assert.That(this.Subject.Informations, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenUnauthorizedWithService : ViewModelTestBase
        {
            [TestCase]
            public void ItShouldDisplayErrorAlertAsyncCall()
            {
                var exception = new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

                var request = new Mock<RequestBase<IndexResponse>>();
                request.Setup(x => x.Execute()).ThrowsAsync(exception);

                this.RequestFactory.Setup(x => x.InformationsIndexRequest()).Returns(request.Object);

                var subject = new Mock<SubjectViewModel>(this.NavigationService.Object, this.PageDialogService.Object);

                subject.Object.BuildUp(this.Container);

                subject.Object.RefreshCommand.Execute();

                while (!subject.Object.RefreshCommand.CanExecute()) { }

                subject.Protected().Verify<Task<bool>>("DisplayErrorAlertAsync", Times.Once(), exception);
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

    [TestFixture]
    public class ItemTappedCommandTest : ViewModelTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [TestCase]
        public void ItShouldNavigateToWebViewPage()
        {
            var viewModel = new SubjectViewModel(this.NavigationService.Object);

            viewModel.ItemTappedCommand.Execute(new Information { Url = "http://example.com/informations/1" });

            while (!viewModel.ItemTappedCommand.CanExecute()) { }

            this.NavigationService.Verify(
                x => x.NavigateAsync(
                    "WebViewPage",
                    It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Url) as string == "http://example.com/informations/1"))
                , Times.Once());
        }
    }
}
