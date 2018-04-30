using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class ParticipantsPage : ContentPage
    {
        public ParticipantsPage()
        {
            InitializeComponent();

            this.ListView.ItemSelected += (s, e) => this.ListView.SelectedItem = null;
        }
    }
}

