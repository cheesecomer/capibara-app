#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using NUnit.Framework;
using Prism.Navigation;
using Prism.Services;
using Moq;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class ViewModelBaseTest
    {
        class StubViewModel : ViewModelBase
        {
            public StubViewModel(
                INavigationService navigationService = null,
                IPageDialogService pageDialogService = null)
                : base(navigationService, pageDialogService) { }
        }

        [Test]
        public void OnResume()
        {
            Assert.DoesNotThrow(new StubViewModel().OnResume);
        }

        [Test]
        public void OnSleep()
        {
            Assert.DoesNotThrow(new StubViewModel().OnSleep);
        }

        [Test]
        public void OnNavigatingTo()
        {
            Assert.DoesNotThrow(() => new StubViewModel().Initialize(new NavigationParameters()));
        }
    }

    public class ViewModelBaseWithModelTest
    {
        public class StubModel : Domain.Models.ModelBase<StubModel>
        {
            public override StubModel Restore(StubModel other) => this;
        }

        class StubViewModel : ViewModelBase<StubModel>
        {
            public StubViewModel(
                INavigationService navigationService = null,
                IPageDialogService pageDialogService = null,
                StubModel model = null)
                : base(navigationService, pageDialogService, model) { }
        }

        [Test]
        public void NewInstance_WhenModelIsNull()
        {
            Assert.NotNull(new StubViewModel().Model);
        }

        [Test]
        public void NewInstance_WhenModelIsPresent()
        {
            var expected = new StubModel();
            Assert.That(new StubViewModel(model: expected).Model, Is.EqualTo(expected));
        }

        [Test]
        public void OnResume()
        {
            Assert.DoesNotThrow(new StubViewModel().OnResume);
        }

        [Test]
        public void OnSleep()
        {
            Assert.DoesNotThrow(new StubViewModel().OnSleep);
        }

        [Test]
        public void OnNavigatingTo_WhenParametersNull()
        {
            Assert.DoesNotThrow(() => new StubViewModel().Initialize(null));
        }

        [Test]
        public void OnNavigatingTo_WhenParametersWithModel()
        {
            var model = new Mock<StubModel>();
            var expected = new StubModel();
            new StubViewModel(model: model.Object).Initialize(new NavigationParameters { { ParameterNames.Model, expected } });
            model.Verify(x => x.Restore(expected), Times.Once);
        }

        [Test]
        public void OnNavigatingTo_WhenParametersWithoutModel()
        {
            var model = new Mock<StubModel>();
            new StubViewModel(model: model.Object).Initialize(new NavigationParameters());
            model.Verify(x => x.Restore(model.Object), Times.Once);
        }
    }
}
