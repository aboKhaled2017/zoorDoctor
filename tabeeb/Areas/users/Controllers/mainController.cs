using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using tabeeb.Areas.users.Models;
using tabeeb.Controllers;
using tabeeb.Models;
namespace tabeeb.Areas.users.Controllers
{
    /*this is main class of users site
     * this is parent class for all users classes
       this class contains shared functions among all sub classes
     */
    public class mainController : tabeeb.Controllers.globalController
    {
        /*this is database variable that contains all database tables and view and so on...*/
        protected tabeebekEntities db = new tabeebekEntities();
        /*this function is called after execution any function at this class*/
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string language = "";
            if (isUserAuthenticated())
            {//if user is logged in
                //get current language[ar|en]
                language = Request.getUserCookieData().lang;
            }
            else
            {//user is not logged in
                //get language from general cooke if that cookie is founded
                //default language is arabic
                var cookie = Request.Cookies[userCookieDataName];
                language = (cookie != null) ? cookie.Values.Get("language") : "ar";
            }
            setCurrentLanguage(language);
        }
        //return if user is authenticated
        public bool isUserAuthenticated()
        {
            return User.Identity.IsAuthenticated && Roles.IsUserInRole("user");
        }
        //return the current logged patient(user) at users website
        public patient getCurrentpatient()
        {
            try
            {
                if (!isUserAuthenticated()) return null;//user not logged in
                //get cookie data of the logged user
                var userCookieData = Request.getUserCookieData();
                //user may be internal or external authenticated
                /*
                 * external authenticated->mean that registered user used facebook or gmail to sign in,
                 * internal authenticated->mean that registered user,used normal registration method to sign in
                 */
                if (userCookieData.providerName == null)
                {//user is internal authenticated
                    /*
                     * providerName=[facebook|gmail|null]
                     */
                    //return patient record from database
                    return db.patients.FirstOrDefault(p => p.providerName == null && p.username == userCookieData.username);
                }
                else
                {//user is external authenticated
                    //return patient record from database
                    return db.patients.FirstOrDefault(
                        p => p.providerName == userCookieData.providerName && p.providerID == userCookieData.providerID);
                }
            }
            catch(Exception)
            {
                return null;
            }
        }
        //this function used to add new cookie for patient(user)
        protected internal void addAuthCookie(patient patient, bool isPresist)
        {
            string patientCookieData = new JavaScriptSerializer().Serialize(new userCookieData { providerName = patient.providerName, providerID = patient.providerID, username = patient.username,lang=currentLanguage});
            var cookie = (patient.providerName == null)
            ? FormsAuthentication.GetAuthCookie(patient.username, isPresist)
            : FormsAuthentication.GetAuthCookie(patient.providerName + "" + patient.providerID, isPresist);
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, DateTime.Now.AddYears(100), ticket.IsPersistent, patientCookieData, "/" + defaultPathForUsersArea);
            cookie.Value = FormsAuthentication.Encrypt(newTicket);
            if (isPresist) cookie.Expires = newTicket.Expiration;
            cookie.Path = newTicket.CookiePath;
            //Response.SetCookie(cookie);
            Response.Cookies.Add(cookie);
        }
	}
}