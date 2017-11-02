using System;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Services;
using Capibara.Views;

using Microsoft.Practices.Unity;

using Prism.Unity;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using UnityDependency = Microsoft.Practices.Unity.DependencyAttribute;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Capibara
{
    public partial class App : PrismApplication, IApplication, ICapibaraApplication
    {
        public App(IPlatformInitializer initializer) : base(initializer)
        {
            this.HasPlatformInitializer = initializer.IsPresent();
        }

        public App() : base((IPlatformInitializer)null) { }

        public bool HasPlatformInitializer { get; private set; }

        /// <summary>
        /// 環境設定
        /// </summary>
        /// <value>The environment.</value>
        public IEnvironment Environment { get; } = new EnvironmentLocal();

        [UnityDependency]
        public IIsolatedStorage IsolatedStorage { get; set; }

        protected override void OnInitialized()
        {
            this.InitializeComponent();

            this.BuildUp(this.Container);

            this.NavigationService.NavigateAsync("SplashPage");
        }

        protected override void RegisterTypes()
        {
            this.Container.RegisterInstance<ICapibaraApplication>(this);
            this.Container.RegisterInstance<IUnityContainer>(this.Container);
            this.Container.RegisterInstance<IRestClient>(new RestClient());
            this.Container.RegisterInstance<IEnvironment>(this.Environment);
            this.Container.RegisterInstance<IIsolatedStorage>(new IsolatedStorageStub());
            this.Container.RegisterInstance<IProgressDialogService>(new ProgressDialogServiceStub());
            this.Container.RegisterInstance<IWebSocketClientFactory>(new WebSocketClientFactory());

            this.Container.RegisterTypeForNavigation<MainPage>();
            this.Container.RegisterTypeForNavigation<NavigationPage>();
            this.Container.RegisterTypeForNavigation<SplashPage>();
            this.Container.RegisterTypeForNavigation<SignInPage>();
            this.Container.RegisterTypeForNavigation<SignUpPage>();
            this.Container.RegisterTypeForNavigation<FloorMapPage>();
            this.Container.RegisterTypeForNavigation<RoomPage>();
            this.Container.RegisterTypeForNavigation<ParticipantsPage>();
            this.Container.RegisterTypeForNavigation<SettingPage>();
            this.Container.RegisterTypeForNavigation<MyProfilePage>();
        }

        private class IsolatedStorageStub : IIsolatedStorage
        {
            public string UserNickname { get; set; }
            public string AccessToken { get; set; }
            public int UserId { get; set; }

            public void Save()
            {
                throw new NotImplementedException();
            }
        }

        private class ProgressDialogServiceStub : IProgressDialogService
        {
            public Task DisplayAlertAsync(Task task, string message = null)
                => throw new NotImplementedException();
        }
    }
}

