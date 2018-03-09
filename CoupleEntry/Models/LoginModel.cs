using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoupleEntry.Models
{
    public class LoginModel
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string ImageUrl { get; set; }
        public string Token { get; set; }
        public int Expiry { get; set; }
    }
}