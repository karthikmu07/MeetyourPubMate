using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoupleEntry.Models
{
    public class Message
    {
        public Int64 MessageId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }
    }
}