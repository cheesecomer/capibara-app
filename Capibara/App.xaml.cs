using System;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Net.OAuth;
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
            containerRegistry.RegisterInstance<ITwitterOAuthService>(new TwitterOAuthService(this.Environment));
            containerRegistry.RegisterInstance<IWebSocketClientFactory>(new WebSocketClientFactory());

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
            containerRegistry.RegisterForNavigationOnIdiom<MyProfilePage, UserProfilePageViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<UserProfilePage, UserProfilePageViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<EditProfilePage, UserProfilePageViewModel>();
        }

        private class IsolatedStorageStub : IIsolatedStorage
        {
            public string UserNickname { get; set; }
            public string AccessToken { get; set; }
            public TokenPair OAuthRequestTokenPair { get; set; }
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

        private class TwitterOAuthService : ITwitterOAuthService
        {
            private IEnvironment environment;

            public TwitterOAuthService(IEnvironment environment)
            {
                this.environment = environment;
            }

            async Task<Session> ITwitterOAuthService.AuthorizeAsync()
            {
                var session = await CoreTweet.OAuth.AuthorizeAsync(
                    this.environment.TwitterConsumerKey,
                    this.environment.TwitterConsumerSecretKey,
                    "capibara://cheese-comer.com/oauth/twitter");

                return new Session
                {
                    AuthorizeUri = session.AuthorizeUri,
                    RequestToken = new TokenPair
                    {
                        Token = session.RequestToken,
                        TokenSecret = session.RequestTokenSecret
                    }
                };
            }

            async Task<TokenPair> ITwitterOAuthService.GetAccessTokenAsync(TokenPair requestTokenPair, string oauthVerifier)
            {
                var session = new CoreTweet.OAuth.OAuthSession
                {
                    ConsumerKey = this.environment.TwitterConsumerKey,
                    ConsumerSecret = this.environment.TwitterConsumerSecretKey,
                    RequestToken = requestTokenPair.Token,
                    RequestTokenSecret = requestTokenPair.TokenSecret
                };

                var tokens = await CoreTweet.OAuth.GetTokensAsync(session, oauthVerifier);

                return new TokenPair
                {
                    Token = tokens.AccessToken,
                    TokenSecret = tokens.AccessTokenSecret
                };
            }
        }
    }
}

