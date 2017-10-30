using Xamarin.Forms.Platform.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;

using Capibara.iOS;

[assembly: ExportRenderer(typeof(Page), typeof(KeyboardOverlapRenderer))]
namespace Capibara.iOS
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

            var page = Element as ContentPage;

            if (page != null)
            {
                var contentScrollView = page.Content as ScrollView;

                if (contentScrollView != null)
                    return;

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
                    NSNotificationCenter.DefaultCenter
                        .AddObserver(UIKeyboard.WillChangeFrameNotification, this.OnKeyboardWillChangeFrame);

            if (this.willHideObserver == null)
                this.willHideObserver = 
                    NSNotificationCenter.DefaultCenter
                        .AddObserver(UIKeyboard.WillHideNotification, this.OnKeyboardWillHide);
            
            if (this.willShowObserver == null)
                this.willShowObserver = 
                    NSNotificationCenter.DefaultCenter
                        .AddObserver(UIKeyboard.WillShowNotification, this.OnKeyboardWillShow);
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

        private void OnKeyboardWillChangeFrame(NSNotification notification)
        {
            if (!this.isKeyboardShown)
            {
                return;
            }

            this.AdjustViewSize(notification);
        }

        private void OnKeyboardWillShow(NSNotification notification)
        {
            if (!IsViewLoaded || this.isKeyboardShown)
                return;

            this.isKeyboardShown = true;
            this.beforeHeight = this.ContentView.Bounds.Height;
            this.AdjustViewSize(notification);
        }

        private void OnKeyboardWillHide(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            this.isKeyboardShown = false;
            var pageFrame = this.ContentView.Bounds;
            this.ContentView.LayoutTo(new Rectangle(pageFrame.X, pageFrame.Y, pageFrame.Width, this.beforeHeight));
        }

        private void AdjustViewSize(NSNotification notification)
        {
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
            var pageFrame = this.ContentView.Bounds;
            this.ContentView.LayoutTo(new Rectangle(pageFrame.X, pageFrame.Y, pageFrame.Width, pageFrame.Height - keyboardFrame.Height));
        }
    }
}
