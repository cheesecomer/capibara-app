using System;

using Capibara.ViewModels;

using Xamarin.Forms;

namespace Capibara.Selectors
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OwnMessageTemplate { get; set; }

        public DataTemplate OthersMessageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = item as MessageViewModel;
            
            return message.IsOwn.Value ? this.OwnMessageTemplate : this.OthersMessageTemplate;
        }
    }
}
