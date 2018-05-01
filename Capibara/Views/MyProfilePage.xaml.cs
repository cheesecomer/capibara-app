using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class MyProfilePage : ContentPage
    {
        public string TabbedPageTitle { get; } = "プロフィール";

        public MyProfilePage()
        {
            InitializeComponent();  

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView(this.GetType().Name);
        }
    }
}

