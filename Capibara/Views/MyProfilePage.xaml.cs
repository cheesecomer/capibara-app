using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class MyProfilePage : ContentPage
    {
        public MyProfilePage()
        {
            InitializeComponent();  

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView(this.GetType().Name);
        }
    }
}

