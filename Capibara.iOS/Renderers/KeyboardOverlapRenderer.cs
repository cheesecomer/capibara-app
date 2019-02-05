using Xamarin.Forms.Platform.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;

using Capibara.iOS.Renderers;

[assembly: ExportRenderer(typeof(Page), typeof(KeyboardOverlapRenderer))]
namespace Capibara.iOS.Renderers
{
    [Preserve(AllMembers = true)]
    public class KeyboardOverlapRenderer : PageRenderer
    {
        NSObject willChangeFrameObserver;

        NSObject willShowObserver;

        NSObject willHideObserver;

        private bool isKeyboardShown;

        private View ContentView => (Element as ContentPage)?.Content;

        private double beforeHeight;

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (Element is ContentPage page)
            {
                RegisterForKeyboardNotifications();
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            UnregisterForKeyboardNotifications();
        }

        private void RegisterForKeyboardNotifications()
        {

            if (this.willChangeFrameObserver == null)
                this.willChangeFrameObserver =
                    UIKeyboard.Notifications.ObserveWillChangeFrame(this.OnKeyboardWillChangeFrame);

            if (this.willHideObserver == null)
                this.willHideObserver =
                    UIKeyboard.Notifications.ObserveWillHide(this.OnKeyboardWillHide);

            if (this.willShowObserver == null)
                this.willShowObserver =
                    UIKeyboard.Notifications.ObserveWillShow(this.OnKeyboardWillShow);
        }

        private void UnregisterForKeyboardNotifications()
        {
            this.isKeyboardShown = false;
            if (this.willChangeFrameObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(this.willChangeFrameObserver);
                this.willChangeFrameObserver.Dispose();
                this.willChangeFrameObserver = null;
            }

            if (this.willHideObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(this.willHideObserver);
                this.willHideObserver.Dispose();
                this.willHideObserver = null;
            }

            if (this.willShowObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(this.willShowObserver);
                this.willShowObserver.Dispose();
                this.willShowObserver = null;
            }
        }

        private void OnKeyboardWillChangeFrame(object sender, UIKeyboardEventArgs args)
        {
            if (!this.isKeyboardShown)
            {
                return;
            }

            this.AdjustViewSize(args.Notification);
        }

        private void OnKeyboardWillShow(object sender, UIKeyboardEventArgs args)
        {
            if (!IsViewLoaded || this.isKeyboardShown)
                return;

            this.isKeyboardShown = true;
            this.beforeHeight = this.ContentView.Bounds.Height;
            this.AdjustViewSize(args.Notification);
        }

        private void OnKeyboardWillHide(object sender, UIKeyboardEventArgs args)
        {
            if (!IsViewLoaded | !this.isKeyboardShown)
                return;

            this.isKeyboardShown = false;
            var pageFrame = this.ContentView.Bounds;
            this.ContentView.LayoutTo(new Rectangle(pageFrame.X, pageFrame.Y, pageFrame.Width, this.beforeHeight));
        }

        private void AdjustViewSize(NSNotification notification)
        {
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
            var pageFrame = this.ContentView.Bounds;
            this.ContentView.LayoutTo(new Rectangle(pageFrame.X, pageFrame.Y, pageFrame.Width, this.beforeHeight - keyboardFrame.Height));
        }
    }
}
