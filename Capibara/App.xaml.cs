using System;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Services;
using Capibara.ViewModels;
using Capibara.Views;

using Unity;

using Prism;
using Prism.Unity;
using Prism.Ioc;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        protected override void OnInitialized()
        {
            this.InitializeComponent();

            this.NavigationService.NavigateAsync("SplashPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<ICapibaraApplication>(this);
            containerRegistry.RegisterInstance<IContainerRegistry>(containerRegistry);
            containerRegistry.RegisterInstance<IRestClient>(new RestClient());
            containerRegistry.RegisterInstance<IEnvironment>(this.Environment);
            containerRegistry.RegisterInstance<IWebSocketClientFactory>(new WebSocketClientFactory());
            containerRegistry.RegisterInstance<IRequestFactory>(new RequestFactory());
            containerRegistry.RegisterInstance<ITaskService>(new TaskService());

            if (this.Container.TryResolve<IIsolatedStorage>() == null)
                containerRegistry.RegisterInstance<IIsolatedStorage>(new IsolatedStorageStub());
            
            if (this.Container.TryResolve<IProgressDialogService>() == null)
                containerRegistry.RegisterInstance<IProgressDialogService>(new ProgressDialogServiceStub());

            if (this.Container.TryResolve<IPickupPhotoService>() == null)
                containerRegistry.RegisterInstance<IPickupPhotoService>(new PickupPhotoServiceStub());

            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<SplashPage>();
            containerRegistry.RegisterForNavigation<SignInPage>();
            containerRegistry.RegisterForNavigation<SignUpPage>();
            containerRegistry.RegisterForNavigation<FloorMapPage>();
            containerRegistry.RegisterForNavigation<RoomPage>();
            containerRegistry.RegisterForNavigation<ParticipantsPage>();
            containerRegistry.RegisterForNavigation<SettingPage>();
            containerRegistry.RegisterForNavigation<BlockUsersPage>();
            containerRegistry.RegisterForNavigation<InformationsPage>();
            containerRegistry.RegisterForNavigation<UnsubscribePage>();
            containerRegistry.RegisterForNavigation<ReportPage>();
            containerRegistry.RegisterForNavigation<WebViewPage>();
            containerRegistry.RegisterForNavigation<AcceptPage>();
            containerRegistry.RegisterForNavigationOnIdiom<MyProfilePage, UserViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<UserProfilePage, UserViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<EditProfilePage, UserViewModel>();
        }

        private class IsolatedStorageStub : IIsolatedStorage
        {
            public string UserNickname { get; set; }
            public string AccessToken { get; set; }
            public int UserId { get; set; }
            public Uri OAuthCallbackUrl { get; set; }

            public void Save()
            {
                throw new NotImplementedException();
            }
        }

        private class ProgressDialogServiceStub : IProgressDialogService
        {
            public Task DisplayProgressAsync(Task task, string message = null)
                => throw new NotImplementedException();
        }

        private class PickupPhotoServiceStub : IPickupPhotoService
        {
            public Task<byte[]> DisplayAlbumAsync()
                => throw new NotImplementedException();
        }

        private class TaskService : ITaskService
        {
            Task ITaskService.Delay(int millisecondsDelay) => Task.Delay(millisecondsDelay);
        }
    }
}

