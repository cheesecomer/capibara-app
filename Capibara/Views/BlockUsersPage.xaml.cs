using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class BlockUsersPage : ContentPage
    {
        public BlockUsersPage()
        {
            InitializeComponent();

            this.ListView.ItemSelected += (s, e) => this.ListView.SelectedItem = null;
        }
    }
}
