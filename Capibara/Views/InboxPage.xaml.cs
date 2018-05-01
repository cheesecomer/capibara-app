using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class InboxPage : ContentPage
    {
        public string TabbedPageTitle { get; } = "ダイレクトメッセージ";

        public InboxPage()
        {
            InitializeComponent();

            this.ListView.ItemSelected += (s, e) => this.ListView.SelectedItem = null;
        }
    }
}
