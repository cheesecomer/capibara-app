using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();

            this.ListView.ItemSelected += (s, e) => this.ListView.SelectedItem = null;
        }
    }
}

