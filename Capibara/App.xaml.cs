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
using Plugin.GoogleAnalytics.Abstractions;
using Plugin.GoogleAnalytics.Abstractions.Model;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Capibara
{
    public partial class App : PrismApplication, IApplication, ICapibaraApplication
    {
        public App(IPlatformInitializer initializer) : base(initializer)
        {
            this.HasPlatformInitializer = initializer.IsPresent();
        }

        public App() : base(null) { }

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
            containerRegistry.RegisterInstance(containerRegistry);
            containerRegistry.RegisterInstance<IRestClient>(new RestClient());
            containerRegistry.RegisterInstance(this.Environment);
            containerRegistry.RegisterInstance<IWebSocketClientFactory>(new WebSocketClientFactory());
            containerRegistry.RegisterInstance<IRequestFactory>(new RequestFactory());
            containerRegistry.RegisterInstance<ITaskService>(new TaskService());
            containerRegistry.RegisterInstance<IOverrideUrlService>(new OverrideUrlService());

            if (this.Container.TryResolve<ITracker>() == null)
                containerRegistry.RegisterInstance<ITracker>(new TrackerStub());

            if (this.Container.TryResolve<IIsolatedStorage>() == null)
                containerRegistry.RegisterInstance<IIsolatedStorage>(new IsolatedStorageStub());
            
            if (this.Container.TryResolve<IProgressDialogService>() == null)
                containerRegistry.RegisterInstance<IProgressDialogService>(new ProgressDialogServiceStub());

            if (this.Container.TryResolve<IPickupPhotoService>() == null)
                containerRegistry.RegisterInstance<IPickupPhotoService>(new PickupPhotoServiceStub());

            if (this.Container.TryResolve<IScreenService>() == null)
                containerRegistry.RegisterInstance<IScreenService>(new ScreenServiceStub());

            if (this.Container.TryResolve<IApplicationService>() == null)
                containerRegistry.RegisterInstance<IApplicationService>(new ApplicationServiceStub());

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
            containerRegistry.RegisterForNavigation<InquiryPage>();
            containerRegistry.RegisterForNavigationOnIdiom<MyProfilePage, UserViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<UserProfilePage, UserViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<EditProfilePage, UserViewModel>();
        }

        private class ApplicationServiceStub : IApplicationService
        {
            string IApplicationService.StoreUrl { get; } = string.Empty;

            string IApplicationService.Platform { get; } = string.Empty;

            string IApplicationService.AppVersion { get; } = string.Empty;

            void IApplicationService.Exit()
                => throw new NotImplementedException();
        }

        private class IsolatedStorageStub : IIsolatedStorage
        {
            public string UserNickname { get; set; }
            public string AccessToken { get; set; }
            public int UserId { get; set; }
            public Uri OAuthCallbackUrl { get; set; }

            public void Save()
                => throw new NotImplementedException();
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

        private class ScreenServiceStub : IScreenService
        {
            public Size Size => new Size();
        }

        private class TaskService : ITaskService
        {
            Task ITaskService.Delay(int millisecondsDelay) => Task.Delay(millisecondsDelay);
        }

        private class TrackerStub : ITracker
        {
            string ITracker.TrackingId => throw new NotImplementedException();

            bool ITracker.IsAnonymizeIpEnabled { get; set; }
            string ITracker.AppName { get; set; }
            string ITracker.AppVersion { get; set; }
            string ITracker.AppId { get; set; }
            string ITracker.AppInstallerId { get; set; }
            Dimensions ITracker.AppScreen { get; set; }
            string ITracker.CampaignName { get; set; }
            string ITracker.CampaignSource { get; set; }
            string ITracker.CampaignMedium { get; set; }
            string ITracker.CampaignKeyword { get; set; }
            string ITracker.CampaignContent { get; set; }
            string ITracker.CampaignId { get; set; }
            string ITracker.Referrer { get; set; }
            string ITracker.DocumentEncoding { get; set; }
            string ITracker.GoogleAdWordsId { get; set; }
            string ITracker.GoogleDisplayAdsId { get; set; }
            string ITracker.IpOverride { get; set; }
            string ITracker.UserAgentOverride { get; set; }
            string ITracker.DocumentLocationUrl { get; set; }
            string ITracker.DocumentHostName { get; set; }
            string ITracker.DocumentPath { get; set; }
            string ITracker.DocumentTitle { get; set; }
            string ITracker.LinkId { get; set; }
            string ITracker.ExperimentId { get; set; }
            string ITracker.ExperimentVariant { get; set; }
            string ITracker.DataSource { get; set; }
            string ITracker.UserId { get; set; }
            string ITracker.GeographicalId { get; set; }
            float ITracker.SampleRate { get; set; }
            bool ITracker.IsUseSecure { get; set; }
            bool ITracker.IsDebug { get; set; }
            bool ITracker.ThrottlingEnabled { get; set; }

            void ITracker.SendEvent(string category, string action, string label, long value)
                => throw new NotImplementedException();

            void ITracker.SendEvent(string category, string action, string label, int value)
                => throw new NotImplementedException();

            void ITracker.SendEvent(string category, string action, string label)
                => throw new NotImplementedException();

            void ITracker.SendEvent(string category, string action)
                => throw new NotImplementedException();

            void ITracker.SendException(string description, bool isFatal)
                => throw new NotImplementedException();

            void ITracker.SendException(Exception exception, bool isFatal)
                => throw new NotImplementedException();

            void ITracker.SendSocial(string network, string action, string target)
                => throw new NotImplementedException();

            void ITracker.SendTiming(TimeSpan time, string category, string variable, string label)
                => throw new NotImplementedException();

            void ITracker.SendTransaction(Transaction transaction)
                => throw new NotImplementedException();
            
            void ITracker.SendTransactionItem(TransactionItem transactionItem)
                => throw new NotImplementedException();

            void ITracker.SendView(string screenName)
                => throw new NotImplementedException();

            void ITracker.SetCustomDimension(int index, string value)
                => throw new NotImplementedException();

            void ITracker.SetCustomMetric(int index, long value)
                => throw new NotImplementedException();

            void ITracker.SetEndSession(bool value)
                => throw new NotImplementedException();

            void ITracker.SetStartSession(bool value)
                => throw new NotImplementedException();
        }
    }
}

