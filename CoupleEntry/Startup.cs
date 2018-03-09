using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(CoupleEntry.Startup))]
namespace CoupleEntry
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
         
        }
    }
}
