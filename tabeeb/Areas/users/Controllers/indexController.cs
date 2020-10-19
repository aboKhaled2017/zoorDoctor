using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.Security;
using tabeeb.Areas.users.languages;
using tabeeb.Models;
using tabeeb.Controllers;
using tabeeb.Areas.users.Models;
using System.Web.Script.Serialization;
namespace tabeeb.Areas.users.Controllers
{
    [AllowAnonymous]
    public class indexController : mainController
    {/*this is main controller of website*/
        /*the main page of website*/
        public ViewResult Index()
        {
                return View();
        }
        [HttpPost]
        /*function used by javascript to get data about complains*/
        public JsonResult getComplains(complain userComplain)
        {
            if (userComplain.mail.Length < 5 || userComplain.name.Length<3||userComplain.message.Length<15)
            {
                return Json(false,JsonRequestBehavior.AllowGet);
            }
            try
            {
                userComplain.id = Guid.NewGuid();
                userComplain.dateOfJoin = DateTime.Now;
                if (isUserAuthenticated()) userComplain.general = true;
                else userComplain.general = false;
                db.complains.Add(userComplain);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        /*add new complain for users*/
        public JsonResult contactUs(complain userComplain)
        {
            try
            {//no validation at server untill now
                userComplain.id = Guid.NewGuid();
                userComplain.dateOfJoin = DateTime.Now;
                if (User.Identity.IsAuthenticated) userComplain.general = true;
                db.complains.Add(userComplain);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { result=false},JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }
        /*privacy page*/
        public ViewResult privacy()
        {
            return View();
        }        
	    [AllowAnonymous]       
        /*this function called when error occure*/
        public ViewResult errorAction()
        {
            return View();
        }
        /*to change language*/
        public ActionResult changeLanguage(string lang)
        {
            setCurrentLanguage(lang);
            if (isUserAuthenticated())
            {//set doctor language to authenticated cookie
                userCookieData userData=Request.getUserCookieData();
                userData.lang=lang;
                string userCookiedata = new JavaScriptSerializer().Serialize(userData);
                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, DateTime.Now.AddYears(100), ticket.IsPersistent, userCookiedata, "/" + defaultPathForUsersArea);
                cookie.Value = FormsAuthentication.Encrypt(newTicket);
                if (ticket.IsPersistent) cookie.Expires = newTicket.Expiration;
                cookie.Path = newTicket.CookiePath;
                Response.Cookies.Set(cookie);
            }
            else
            {//create temporary cookie untill doctor will be authenricated to save language value
                var cookie = Request.Cookies[userCookieDataName];
                if (cookie != null)
                {
                    cookie.Values.Set("language", lang);
                    cookie.Path = "/" +defaultPathForUsersArea;
                    Response.Cookies.Set(cookie);
                }
                else
                {
                    cookie = new System.Web.HttpCookie(userCookieDataName);
                    cookie.Path = "/" + defaultPathForUsersArea;
                    cookie.Values.Add("language", lang);
                    Response.Cookies.Add(cookie);
                }
            }
            if (Request.UrlReferrer == null) return Redirect("/"+defaultPathForUsersArea);
            return Redirect(Request.UrlReferrer.ToString());

        }
        /*footer of users website*/
        public PartialViewResult footer()
        {
            List<string> specialities = (currentLanguage == "en")
                ?db.specialities.OrderByDescending(s=>s.doctors.Count).Select(s => s.nameEng).ToList()
                :db.specialities.OrderByDescending(s=>s.doctors.Count).Select(s => s.nameAr).ToList();
            return PartialView(specialities);
        }
        /*rules and conditions page*/
        public ActionResult rulesAndCondition()
        {
            return View();
        }
        /*get menu links of user website as html string*/
        public MvcHtmlString patientMenuLinks()
        {
            StringBuilder lists = new StringBuilder();
            TagBuilder liProfile = new TagBuilder("li");
            if(Request.getUserCookieData().providerName==null)
            {
                liProfile.InnerHtml = "<li><a href=/" +
                defaultPathForUserSite + "/profile><i class='fa fa-user'></i>" +
                Resource1.profilePage + "</a>";
            }
            else
            {
                liProfile.InnerHtml = "<li><a href=/" +
                defaultPathForUserSite + "/externalProfile><i class='fa fa-user'></i>" +
                Resource1.profilePage + "</a>";
            }            
            TagBuilder liAppointement = new TagBuilder("li");
            liAppointement.InnerHtml = "<li><a href=/" + defaultPathForUserSite +
                "/appointements><i class='fa fa-calendar-check-o fa-lg'></i>" +
                Resource1.myAppointement + "</a></li>";
            TagBuilder liLogOut = new TagBuilder("li");
            liLogOut.InnerHtml = "<li><a href=/" + defaultPathForUserSite +
                "/logout><i class='glyphicon glyphicon-log-out'></i>" +
                Resource1.logOut + "</a></li>";
            lists.Append(liProfile);
            lists.Append(liAppointement);
            lists.Append(liLogOut);
            return (MvcHtmlString.Create(lists.ToString()));
        }
        /*this function get doctor page when user click on doctor card*/
        public ActionResult doctorPage(Guid id)
        {
            try
            {
                 var doctorData = db.doctors.activatedDoctors().Single(d=>d.id==id).getDoctorPageInfo(true);
                 return View(doctorData);              
            }
            catch
            {
                return Redirect("/" + defaultPathForUsersArea);
            }
        }
    }
}