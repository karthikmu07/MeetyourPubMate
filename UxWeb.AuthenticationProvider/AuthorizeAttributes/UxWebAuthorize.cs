using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace CoupleEntry.AuthenticationProvider
{
    /// <summary>
    /// Provides a custom Authorize Attribute which ensured the user is authoirzed before allowing access to externally facing APIs.
    /// </summary>
    public class UxWebAuthorize : AuthorizeAttribute
    {
        /// <summary>
        /// Checks windows authentication or verifies the access token presented.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            HttpCookieCollection cookies = httpContext.Request.Cookies;
            HttpCookie authCookie = cookies.Get("Authorization");
            HttpCookie userMailCookie = cookies.Get("UserMail");
            if (authCookie != null && userMailCookie != null)
            {
                //validation of cookie from google to be done
                return true;
            }

            return false;
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Login" }));
        }


    }
}
