using System;

using Capibara.ViewModels;

using Xamarin.Forms;
namespace Capibara.Selectors
{
    public class DirectMessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OwnMessageTemplate { get; set; }

        public DataTemplate OthersMessageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = item as DirectMessageViewModel;

            return message.IsOwn.Value ? this.OwnMessageTemplate : this.OthersMessageTemplate;

        }
    }
}
