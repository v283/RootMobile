using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RootMobile
{
    public class NotificationItemMessage : ValueChangedMessage<String>
    {
        public NotificationItemMessage(string value) : base(value)
        {

        }
    }
}

