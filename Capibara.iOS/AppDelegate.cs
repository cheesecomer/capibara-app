using Capibara.iOS.Services;
using Capibara.Services;

using Foundation;

using Lottie.Forms.iOS.Renderers;

using Unity;

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
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App(new iOSInitializer()));

            AnimationViewRenderer.Init();

            UINavigationBar.Appearance.SetBackgroundImage(UIImage.FromBundle("bg_header"), UIBarMetrics.Default);
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White });

            return base.FinishedLaunching(app, options);
        }
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IIsolatedStorage>(new IsolatedStorage());
            containerRegistry.RegisterInstance<IProgressDialogService>(new ProgressDialogService());
            containerRegistry.RegisterInstance<IPickupPhotoService>(new PickupPhotoService());
        }
    }
}
