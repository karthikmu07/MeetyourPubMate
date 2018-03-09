using CoupleEntry;
using CoupleEntry.Models;
using System;
using System.Web;
using System.Web.Mvc;
using static CoupleEntry.SessionService;

namespace UxWeb.Controllers
{
    public class LoginController : Controller
    {

        [HttpGet]
        public ActionResult Login()
        {
            RemoveCookiesAndSession();
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {

            if (loginModel != null)
            {
                SetProperty(SessionVariableNames.Login_Model, loginModel);
                SetProperty(SessionVariableNames.Email_Id, loginModel.Email);
                bool exists = DALayer.IsEmailPresentInDB(loginModel.Email);
                if (exists)
                {
                    DALayer.UpsertTokenValue(loginModel.Token, loginModel.Email);
                    DALayer.UpdateImageUrl(loginModel.Email,loginModel.ImageUrl);
                    SetCookies(loginModel);
                    return Json(new { result = "Redirect", url = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "Add", url = Url.Action("AddUserDetails", "Login", JsonRequestBehavior.AllowGet) });
                }

            }

            return Json(new { result = "Error" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddUserDetails()
        {
            User userModel = GetProperty(SessionVariableNames.Current_User) as User;
            if (userModel != null)
                return RedirectToAction("Index","Home");
            LoginModel model = GetProperty(SessionVariableNames.Login_Model) as LoginModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddUserToDB(LoginModel model)
        {
            SetCookies(model);
            User userDetails = DALayer.AddNewUser(model);
            DALayer.UpsertTokenValue(model.Token, model.Email);
            SetProperty(SessionVariableNames.Current_User, userDetails);
            return RedirectToAction("Index", "Home");
        }

        private void SetCookies(LoginModel loginModel)
        {
            HttpCookie AuthCookie = new HttpCookie("Authorization", loginModel.Token);
            AuthCookie.Expires = DateTime.Now.AddSeconds(loginModel.Expiry);
            Response.Cookies.Add(AuthCookie);

            HttpCookie EmailCookie = new HttpCookie("UserMail", loginModel.Email);
            EmailCookie.Expires = DateTime.Now.AddSeconds(loginModel.Expiry);
            Response.Cookies.Add(EmailCookie);

        }
        private void RemoveCookiesAndSession()
        {
            if (Request.Cookies["Authorization"] != null)
            {
                var c = new HttpCookie("Authorization");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            if (Request.Cookies["UserMail"] != null)
            {
                var c = new HttpCookie("UserMail");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            Session.Clear();
        }
    }
}