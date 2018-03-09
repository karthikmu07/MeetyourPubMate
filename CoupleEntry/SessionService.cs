using CoupleEntry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoupleEntry
{
   
    public class SessionService
    {
        public class SessionVariableNames
        {
            public static string Login_Model { get { return "loginModel"; } }
            public static string Email_Id { get { return "emailId"; } }
            public static string Current_User { get { return "currentUser"; } }

        }
      
   
        public static object GetProperty(string propertyName)
        {
            return HttpContext.Current.Session[propertyName];
          
        }

        public static void SetProperty(string propertyName, object propertyValue)
        {
            HttpContext.Current.Session[propertyName] = propertyValue;
        }
    }
}