using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.iOS.Services;
using Capibara.Services;

using Foundation;

using Lottie.Forms.iOS.Renderers;

using Unity;
using Unity.Attributes;

using Prism;
using Prism.Ioc;

using UIKit;

namespace Capibara.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        [Dependency]
        public IIsolatedStorage IsolatedStorage { get; set; }

        /// <summary>
        /// Finisheds the launching.
        /// </summary>
        /// <returns><c>true</c>, if launching was finisheded, <c>false</c> otherwise.</returns>
        /// <param name="uiApplication">App.</param>
        /// <param name="launchOptions">Options.</param>
        [Export("application:didFinishLaunchingWithOptions:")]
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();

            var application = new App(new iOSInitializer());

            this.BuildUp(application.Container.Resolve<IUnityContainer>());

            LoadApplication(application);

            AnimationViewRenderer.Init();

            UINavigationBar.Appearance.SetBackgroundImage(UIImage.FromBundle("bg_header"), UIBarMetrics.Default);
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White });

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            if (url == null) return false;
            var uri = new Uri(url.AbsoluteString);
            if (uri.Scheme == "com.cheesecomer.capibara" && uri.Host.StartsWith("oauth", StringComparison.Ordinal))
            {
                var query = uri.Query
                   .Replace("?", string.Empty).Split('&')
                   .Select(x => x.Split('='))
                   .Where(x => x.Length == 2)
                   .ToDictionary(x => x.First(), x => x.Last());
                this.IsolatedStorage.AccessToken = query["access_token"];
                this.IsolatedStorage.UserId = query["id"].ToInt();
            }

            return false;
        }

        [Export("window")]
        public UIWindow GetWindow() => UIApplication.SharedApplication.KeyWindow;
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IIsolatedStorage>(new IsolatedStorage());
            containerRegistry.RegisterInstance<IProgressDialogService>(new ProgressDialogService());
            containerRegistry.RegisterInstance<IPickupPhotoService>(new PickupPhotoService());
            containerRegistry.RegisterInstance<IScreenService>(new ScreenService());
        }
    }
}
