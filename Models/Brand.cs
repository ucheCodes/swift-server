using System;

namespace Adorable.Models
{
    public class Brand
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description{ get; set; }
        public string ImagePath{ get; set; }
        public string Category { get; set; }  
        public string Date { get; set; }     
    }
}