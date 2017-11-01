using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Capibara.Services;
using Capibara.Models;
using Capibara.ViewModels;
using Capibara.Net;

using Moq;
using Microsoft.Practices.Unity;
using NUnit.Framework;

using Prism.Navigation;

namespace Capibara.Test.ViewModels.ViewModelBaseTest
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

    public class TestHelper
    {
        public static IUnityContainer GenerateContainer()
        {
            // Environment のセットアップ
            var environment = new Mock<IEnvironment>();
            environment.SetupGet(x => x.ApiBaseUrl).Returns("http://localhost:3000/");

            // RestClient のセットアップ
            var restClient = new Mock<IRestClient>();
            restClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>()));

            // ISecureIsolatedStorage のセットアップ
            var secureIsolatedStorage = new Mock<ISecureIsolatedStorage>();
            secureIsolatedStorage.SetupAllProperties();

            var application = new Mock<ICapibaraApplication>();
            application.SetupGet(x => x.HasPlatformInitializer).Returns(true);

            var progressDialogService = new Mock<IProgressDialogService>();
            progressDialogService.Setup(x => x.DisplayAlertAsync(It.IsAny<Task>(), It.IsAny<string>())).Returns((Task task) => task);

            var container = new UnityContainer();
            container.RegisterInstance<IUnityContainer>(container);
            container.RegisterInstance<IEnvironment>(environment.Object);
            container.RegisterInstance<IRestClient>(restClient.Object);
            container.RegisterInstance<ISecureIsolatedStorage>(secureIsolatedStorage.Object);
            container.RegisterInstance<ICapibaraApplication>(application.Object);
            container.RegisterInstance<IProgressDialogService>(progressDialogService.Object);

            return container;
        }
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
        public class WhenValueIsPresent
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {   
                Assert.DoesNotThrow(() => new StabViewModel().Container = TestHelper.GenerateContainer());
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

