using RootMobile.Models;

namespace RootMobile.Views.Templates
{
    internal class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserMessageItemTemplate { get; set; }
        public DataTemplate BotMessageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = (Message)item;

            return message.IsUserMessage ? UserMessageItemTemplate : BotMessageTemplate;
        }
    }
}