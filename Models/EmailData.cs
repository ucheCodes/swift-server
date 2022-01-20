using System;

namespace Adorable.Models
{
    public class EmailData
    {
        public string FromAddress { get; set; }
        public string  ToAddress { get; set; }
        public string Link { get; set; }
        public string Message { get; set; }
        public string Password { get; set; }
        public string FromAddressPassword { get; set; }
        public string  Title { get; set; }
    }
}