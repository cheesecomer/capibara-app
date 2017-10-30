using System;

using Capibara.Models;

using Xamarin.Forms;

namespace Capibara.Selectors
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OwnMessageTemplate { get; set; }

        public DataTemplate OthersMessageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = item as Message;
            
            return message.IsOwn ? this.OwnMessageTemplate : this.OthersMessageTemplate;
        }
    }
}
