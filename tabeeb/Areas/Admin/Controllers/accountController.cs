using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security;
using tabeeb.Models;
namespace tabeeb.Areas.Admin.Controllers
{
    public class accountController : mainController
    {
        [AllowAnonymous]
        public ActionResult logIn()
        {
            if (User.Identity.IsAuthenticated && Roles.IsUserInRole("admin"))
                return Redirect("/admin");
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult logIn(string name, string password, byte? remmember)
        {
            bool rememberMe = false;
            //the email will registered as name
            if (Membership.ValidateUser(name, password) && Roles.IsUserInRole(name, "admin"))
           {
               try
               {
                   admin p = db.admins.Single(ad => ad.name == name && ad.password == password);
                   if (remmember != null) rememberMe = true;
                   var cookie = FormsAuthentication.GetAuthCookie(name, rememberMe);
                   var ticket = FormsAuthentication.Decrypt(cookie.Value);
                   string userImage = p.image;
                   var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, DateTime.Now.AddYears(100), ticket.IsPersistent, userImage,"/admin");
                   cookie.Value = FormsAuthentication.Encrypt(newTicket);
                   if (rememberMe) cookie.Expires = newTicket.Expiration;
                   cookie.Path = newTicket.CookiePath;
                   Response.Cookies.Add(cookie);
                   return Redirect("/admin");
               }
                catch(Exception)
               {
                   TempData["errorMessage"] = "please enter valid username and password";
                   return RedirectToAction("logIn");
               }
           }
            else
            {
               TempData["errorMessage"] = "please enter valid username and password";
                return RedirectToAction("logIn");
            } 
        }     
        public ActionResult logOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            var myCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            myCookie.Path = "/admin";
            HttpContext.Response.Cookies.Add(myCookie);
           return Redirect("/admin/account/login");
        }

	}
}