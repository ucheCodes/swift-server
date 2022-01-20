using System;

namespace Adorable.Models
{
    public class Notifications
    {
        public string Id { get; set; }
        public string SenderId{ get; set; }
        public string ReceiverId{ get; set; }
        public string NotificationType { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }
}