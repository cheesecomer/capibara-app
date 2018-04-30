using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class InformationsPage : ContentPage
    {
        public InformationsPage()
        {
            InitializeComponent();

            this.ListView.ItemSelected += (s, e) => this.ListView.SelectedItem = null;
        }
    }
}
