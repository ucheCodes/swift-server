using System;

namespace Adorable.Models
{
    public class Chat
    {
        public string Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Body { get; set; }
        public string Date { get; set; }
        public string ChatId {get; set;}
        public string SenderSignalrId { get; set; }
    }
}