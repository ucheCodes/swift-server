using System;
using System.Collections;
using System.Collections.Generic;

namespace Adorable.Models
{
    public class Products
    {
        public string UserId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Search{ get; set; }
        public double Shipping { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }   
        public string Category { get; set; } 
        public string ImagePath { get; set; }  
        public List<string> RelatedImages { get; set; }
        public string Date { get; set; } 
    }
}