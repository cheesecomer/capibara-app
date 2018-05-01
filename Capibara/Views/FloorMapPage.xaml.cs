using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class FloorMapPage : ContentPage
    {
        public string TabbedPageTitle { get; } = "チャットルーム";

        public FloorMapPage()
        {
            InitializeComponent();

            this.ListView.ItemSelected += (s, e) => this.ListView.SelectedItem = null;
        }
    }
}

