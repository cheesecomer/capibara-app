using System;

using Capibara.ViewModels;

using Xamarin.Forms;

namespace Capibara.Selectors
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OwnMessageTemplate { get; set; }

        public DataTemplate OthersMessageTemplate { get; set; }

        public DataTemplate OwnImageTemplate { get; set; }
        
        public DataTemplate OthersImageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = item as MessageViewModel;

            var hasImage = message.Model.ImageThumbnailUrl.IsNullOrEmpty();

            var othersTemplate = hasImage ? this.OthersMessageTemplate : this.OthersImageTemplate;

            var ownTemplate = hasImage ? this.OwnMessageTemplate : this.OwnImageTemplate;

            return message.IsOwn.Value ? ownTemplate : othersTemplate;

        }
    }
}
