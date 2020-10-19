using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using tabeeb.Models;
namespace tabeeb.Controllers
{
    [HandleError]
    public class globalController : Controller
    {
        public static tabeebekEntities db { get { return new tabeebekEntities(); } set { } }
        public static string defaultPathForDoctorsArea = "doctor-site";
        public static string defaultPathForUsersArea = "";
        public static string defaultPathForUserSite = "user";
        public static string defaultPathSpecialityPart = "All-specialities";
        public static string defaultPathSearchPart = "Advanced-search";
        public static string defaultPathAdvicesPart = "advices";
        public static string defaultPathLanguagePart = "changeLanguage";
        public static string defaultPathDoctorPage = "doctor-page";
        public static string defaultPathDoctorReservingPage = "reserving/doctor";
        public static string defaultPathReservation = "reserving";
        public static byte maxNumberOfCardsForEachpage=9;
        public static byte maxNumberOfAdvicesCardsForEachPage=5;
        public static int maxProfessionImgSize = 2 * 1024 * 1024;
        public static int maxPersonalImgSize = 1024 *1024;
        public static string doctorCookieDataName = "doctorCookieData";
        public static string userCookieDataName = "userCookieData";
        /*protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            var excep = filterContext.Exception;
            //Redirect or return a view, but not both.
            //filterContext.Result = RedirectToAction("Index", "ErrorHandler");
            // OR 
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Areas/users/Views/index/errorAction.cshtmldex"
            };
        }*/
        public static string currentLanguage
        {
            set { }
            get
            {
                return (Thread.CurrentThread.CurrentUICulture.Name == "en-US") ? "en" : "ar";
            }
        }
        public static void setCurrentLanguage(string language)
        {
            if (language == "en")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-EG");
            }
        }
        public static List<string> monthsArabicName = new List<string> { "يناير", "فبراير", "مارس", "إبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };
        public static List<string> monthsEnglishName = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "Novermber", "December" };
        public static List<string> DaysArabicName = new List<string> { "اﻷحد", "اﻷثنين", "الثلاثاء", "اﻷربعاء", "الخميس", "الجمعة", "السبت" };
        public static List<string> DaysEnglishName = new List<string> {"saturday","sunday","monday","wednsday","tuesday","thursday","friday"};
        public static Dictionary<byte, string> englishProfessions { private set { } get { return getEngDoctorProfessions(); } }
        public static Dictionary<byte, string> arabicProfessions { private set { } get { return getArDoctorProfessions(); } }
        public static Dictionary<byte, string> engEducationsNames { private set { } get { return getEngEducationNames(); } }
        public static Dictionary<byte, string> arEducationsNames { private set { } get { return getArEducationNames(); } }
        private static void sendMail(string email, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("mkhaled2511995@gmail.com");
                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = body; ;
                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("mkhaled2511995@gmail.com", "AAbb123123");
                SmtpServer.EnableSsl = true;
                SmtpServer.SendAsyncCancel();
                SmtpServer.Send(mail);
            }
            catch (Exception)
            {

            }
        }
        public static void sendMailTo(string email, string subject, string body, string sync)
        {
            if (sync == "async") sendMail(email, subject, body);
            else
            {
                Task send = new Task(() => SendEmail(email, subject, body, true));
                send.Start();
            }

        }
        private static void SendEmail(string email, string subject, string body, bool priority)
        {
            bool sent = true;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("mkhaled2511995@gmail.com");
            mail.To.Add(email);
            mail.Subject = subject;
            mail.Body = body; ;
            mail.IsBodyHtml = true;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("mkhaled2511995@gmail.com", "AAbb123123");
            SmtpServer.EnableSsl = true;
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            if (priority) mail.Priority = MailPriority.High;
            try
            {
                SmtpServer.Send(mail);
                SmtpServer.Dispose();
                mail.Dispose();
            }
            catch (SmtpFailedRecipientException ex)
            {
                new Task(() =>
                {
                    SmtpServer.Send(mail);
                    SmtpServer.Dispose();
                    mail.Dispose();
                }).Start();
            }
            catch (Exception)
            {
            }
        }
        private static void SmtpClient_OnCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //Get the Original MailMessage object
            MailMessage mail = (MailMessage)e.UserState;

            //write out the subject
            string subject = mail.Subject;

            if (e.Cancelled)
            {
                //  return false;
            }
            if (e.Error != null)
            {
                // return false;
            }
            else
            {
                string s = "";
                // return true;
            }
        }
        private static Dictionary<byte, string> getEngDoctorProfessions()
        {
            var professions = new Dictionary<byte, string>();
            professions[0] = "Professor of";
            professions[1] = "Lecturer of";
            professions[2] = "Consultant of";
            professions[3] = "Specialist in";
            professions[4] = "Lecturer Assistant in";
            professions[5] = "Professor Assistant in";
            return professions;
        }
        private static Dictionary<byte, string> getArDoctorProfessions()
        {
            var professions = new Dictionary<byte, string>();
            professions[0] = "استاذ فى";
            professions[1] = "محاضر فى";
            professions[2] = "استشارى";
            professions[3] = "اخصائى";
            professions[4] = "مدرس مساعد فى";
            professions[5] = "استاذ مساعد فى";
            return professions;
        }
        private static Dictionary<byte, string> getEngEducationNames()
        {
            var professions = new Dictionary<byte, string>();
            professions[0] = "Professor";
            professions[1] = "Lecturer";
            professions[2] = "Consultant";
            professions[3] = "Specialist";
            professions[4] = "Lecturer Assistant";
            professions[5] = "Professor Assistant";
            return professions;
        }
        private static Dictionary<byte, string> getArEducationNames()
        {
            var professions = new Dictionary<byte, string>();
            professions[0] = "استاذ";
            professions[1] = "محاضر";
            professions[2] = "استشارى";
            professions[3] = "اخصائى";
            professions[4] = "مدرس مساعد";
            professions[5] = "استاذ مساعد";
            return professions;
        }
        protected string renderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewData.Model = model;
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
        public static void UpdateFields<T>(T entity, System.Data.Entity.Infrastructure.DbEntityEntry<T> dbEntityEntry, params Expression<Func<T, object>>[] updatedProperties)
            where T : class
        {
            //var dbEntityEntry =db.Entry(entity);
            dbEntityEntry.State = System.Data.Entity.EntityState.Modified;
            if (updatedProperties.Any())
            {
                //update explicitly mentioned properties
                foreach (var property in updatedProperties)
                {
                    dbEntityEntry.Property(property).IsModified = true;
                }
            }
            else
            {
                //no items mentioned, so find out the updated entries
                foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
                {
                    var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (original != null && !original.Equals(current))
                        dbEntityEntry.Property(property).IsModified = true;
                }
            }
        }
    }
}