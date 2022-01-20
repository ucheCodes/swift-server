using System;

namespace Adorable.Models
{
    public class LastChat
    {
        public string PartnerId { get; set; }
        public string DomainId { get; set; }
        public string ChatId { get; set; }
        public string LastMessage { get; set; }
        public string Date { get; set; }
        public int NumberOfPendingChats { get; set; }
    }
}