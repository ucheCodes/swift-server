using System;
using System.Collections;
using System.Collections.Generic;

namespace Adorable.Models
{
    public class Reply
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string CommenterId { get; set; }
        public string CommentId { get; set; }
        public string ReplierId { get; set; }
        public string Response { get; set; }
        public string Date { get; set; }
    }
}