using System;

namespace Adorable.Models
{
    public class Feedbacks
    {
        public string Id { get; set; }
        public int Star { get; set; }
        public string UserId { get; set; }
        public string Feedback { get; set; }
        public string Date { get; set; }
    }
}