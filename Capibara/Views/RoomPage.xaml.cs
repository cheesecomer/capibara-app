using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class RoomPage : ContentPage
    {
        public RoomPage()
        {
            InitializeComponent();

            this.ListView.ItemSelected += (s, e) => this.ListView.SelectedItem = null;
        }
    }
}

