using System;
using System.Collections;
using System.Collections.Generic;

namespace Adorable.Models
{
    public class ChatManager
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public List<Chat> Chats { get; set; }
        public string ChatId { get; set; }
        public string ReceiverLastSeen { get; set; }
        public string ReceiverImagePath { get; set; }
        public string ReceiverLastname { get; set; }
        public string ReceiverFirstname { get; set; }
    }
}