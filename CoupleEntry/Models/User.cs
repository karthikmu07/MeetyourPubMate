using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoupleEntry.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Username { get; set; }
        public string ImageUrl { get; set; }
        public string EmailId { get; set; }
        public int Age { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime LastSeen { get; set; }
        public string LastSeenDiff { get; set; }
        public List<string> Likes { get; set; }
        public List<string> Matches { get; set; }
    }
}