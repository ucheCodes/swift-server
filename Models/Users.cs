using System;

namespace Adorable.Models
{
    public class Users
    {
        public string UserId { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }  
        public bool IsAdmin { get; set; }    
        public bool IsSuperAdmin { get; set; } 
        public bool IsAffiliate { get; set; }
        public string ImagePath { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public string Date { get; set; }
    }
}