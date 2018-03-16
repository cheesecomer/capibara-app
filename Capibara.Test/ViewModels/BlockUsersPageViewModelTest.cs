using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;
using Prism.Services;

namespace Capibara.Test.ViewModels.BlockUsersPageViewModelTest
{
    namespace RefreshCommandTest
    {
        public abstract class WhenSuccessBase : ViewModelTestBase
        {
            protected BlockUsersPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                this.ViewModel = new BlockUsersPageViewModel();

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
                => "{\"blocks\": [" +
                    "{ \"id\": 1, \"target\":  { \"id\": 11, \"nickname\": \"ABC1\" } }," +
                    "{ \"id\": 2, \"target\":  { \"id\": 12, \"nickname\": \"ABC2\" } }," +
                    "{ \"id\": 3, \"target\":  { \"id\": 13, \"nickname\": \"ABC3\" } }," +
                    "{ \"id\": 4, \"target\":  { \"id\": 14, \"nickname\": \"ABC4\" } }," +
                    "{ \"id\": 5, \"target\":  { \"id\": 15, \"nickname\": \"ABC5\" } }," +
                    "{ \"id\": 6, \"target\":  { \"id\": 16, \"nickname\": \"ABC6\" } }," +
                    "{ \"id\": 7, \"target\":  { \"id\": 17, \"nickname\": \"ABC7\" } }," +
                    "{ \"id\": 8, \"target\":  { \"id\": 18, \"nickname\": \"ABC8\" } }," +
                    "{ \"id\": 9, \"target\":  { \"id\": 19, \"nickname\": \"ABC9\" } }," +
                    "{ \"id\": 10, \"target\": { \"id\": 20, \"nickname\": \"ABC0\" } }" +
                "] }";

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new BlockComparer();
                var expect = new List<Block>
                {
                    new Block { Id = 1, Target = new User { Id = 11, Nickname = "ABC1"} },
                    new Block { Id = 2, Target = new User { Id = 12, Nickname = "ABC2"} },
                    new Block { Id = 3, Target = new User { Id = 13, Nickname = "ABC3"} },
                    new Block { Id = 4, Target = new User { Id = 14, Nickname = "ABC4"} },
                    new Block { Id = 5, Target = new User { Id = 15, Nickname = "ABC5"} },
                    new Block { Id = 6, Target = new User { Id = 16, Nickname = "ABC6"} },
                    new Block { Id = 7, Target = new User { Id = 17, Nickname = "ABC7"} },
                    new Block { Id = 8, Target = new User { Id = 18, Nickname = "ABC8"} },
                    new Block { Id = 9, Target = new User { Id = 19, Nickname = "ABC9"} },
                    new Block { Id = 10, Target = new User { Id = 20, Nickname = "ABC0"} },
                };
                Assert.That(this.ViewModel.Blocks, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenSuccess1 : WhenSuccessBase
        {
            protected override string HttpStabResponse
            => "{\"blocks\": [{ \"id\": 10, \"target\": { \"id\": 10, \"nickname\": \"ABC\" } }] }";

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new BlockComparer();
                var expect = new List<Block> { new Block { Id = 10, Target = new User { Id = 10, Nickname = "ABC"} } };
                Assert.That(this.ViewModel.Blocks, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenUnauthorizedWithService : ViewModelTestBase
        {
            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            protected bool IsShowDialog { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var pageDialogService = new Mock<IPageDialogService>();
                pageDialogService
                    .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.Run(() => true))
                    .Callback(() => this.IsShowDialog = true);

                var viewModel = new BlockUsersPageViewModel(
                    this.NavigationService,
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
    }

    public class BlockComparer: IEqualityComparer<Block>
    {
        public bool Equals(Block x, Block y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else
                return
                    x.Id == y.Id
                 && x.Target?.Id == y.Target?.Id
                 && x.Target?.Nickname == y.Target?.Nickname;
        }

        public int GetHashCode(Block room)
        {
            return room.GetHashCode();
        }
    }
}
