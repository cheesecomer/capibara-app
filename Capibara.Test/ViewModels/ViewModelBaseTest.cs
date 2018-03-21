using Capibara.Models;
using Capibara.ViewModels;

using NUnit.Framework;

using Prism.Navigation;

namespace Capibara.Test.ViewModels.ViewModelBase
{

    public class StabModel : ModelBase<StabModel>
    {
        public string Name { get; set; }

        public override void Restore(StabModel model)
        {
            this.Name = model.Name;
        }
    }

    public class StabViewModel : ViewModelBase<StabModel>
    {
        public StabViewModel(INavigationService navigationService = null, StabModel model = null) 
            : base(navigationService, null, model) { }
    }

    namespace ModelTest
    {
        [TestFixture]
        public class WhenValueIsNull
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.NotNull(new StabViewModel().Model);
            }
        }

        [TestFixture]
        public class WhenValueIsPresent
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.NotNull(new StabViewModel(model: new StabModel()).Model);
            }
        }
    }

    namespace ContainerTest
    {
        [TestFixture]
        public class WhenValueIsNull
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().Container = null);
            }
        }

        [TestFixture]
        public class WhenValueIsPresent : TestFixtureBase
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {   
                Assert.DoesNotThrow(() => new StabViewModel().Container = this.GenerateUnityContainer());
            }
        }
    }

    namespace OnNavigatedFromTest
    {
        [TestFixture]
        public class WhenNavigationParametersIsNull
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatedFrom(null));
            }
        }

        [TestFixture]
        public class WhenNavigationParametersIsEmpty
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatedFrom(new NavigationParameters()));
            }
        }
    }

    namespace OnNavigatedToTest
    {
        [TestFixture]
        public class WhenNavigationParametersIsNull
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatedTo(null));
            }
        }

        [TestFixture]
        public class WhenNavigationParametersIsEmpty
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatedTo(new NavigationParameters()));
            }
        }
    }

    namespace OnNavigatingToTest
    {
        [TestFixture]
        public class WhenNavigationParametersIsNull
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatingTo(null));
            }
        }

        [TestFixture]
        public class WhenNavigationParametersIsEmpty
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatingTo(new NavigationParameters()));
            }
        }

        [TestFixture]
        public class WhenNavigationParametersIncludeModel
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                var model = new StabModel();
                var viewModel = new StabViewModel();
                var parameters = new NavigationParameters();
                parameters.Add(ParameterNames.Model, model);

                viewModel.OnNavigatingTo(parameters);

                Assert.That(viewModel.Model.Name, Is.EqualTo(model.Name));
            }
        }
    }
}

