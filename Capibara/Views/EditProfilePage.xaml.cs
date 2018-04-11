using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class EditProfilePage : ContentPage
    {
        public EditProfilePage()
        {
            InitializeComponent();

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView(this.GetType().Name);
        }
    }
}

