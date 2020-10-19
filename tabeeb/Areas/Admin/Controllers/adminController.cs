using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using tabeeb.Models;
using System.Security;
using System.Web.Security;
using System.Data.Entity.Validation;
using System.IO;
using System.Threading.Tasks;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
namespace tabeeb.Areas.Admin.Controllers
{
    public class adminController : mainController
    {
        //
        // GET: /Admin/admin/
        public ActionResult admin()
        {
            ViewBag.controller = "admin";
            return View();
        }
        public JsonResult List(int pageNumber)
        {
            var admins=db.admins.Select(a=>new {a.name,a.mail,a.phone,a.periority,a.dateOfJoin,a.type,a.image}).ToList();
            int count = admins.Count;
            int start = 10 * (pageNumber - 1);
            start = (count > start) ? start : 0;
            bool isLastPage = false;
            int end = 0;
            if (count >= 10 * pageNumber)
            {
                end = 10;
            }
            else
            {
                end = count - (10 * (pageNumber - 1));
                isLastPage = true;
            }
            admins=admins.GetRange(start,end);
            return Json(new { data = admins, isLastPage = isLastPage }, JsonRequestBehavior.AllowGet);
        }
        public ViewResult myProfile()
        {
            ViewBag.controller = "admin";
            admin adm = db.admins.Single(ad => ad.name == User.Identity.Name);
            if (adm.type != "male") ViewBag.maleChecked = "checked";
            else ViewBag.femaleChecked = "checked";
            @ViewBag.type = adm.type;
            return View(adm);
        }
        [HttpPost]
        public async Task<JsonResult> saveImage()
    {
        try
        {
           
                var fileContent = Request.Files[0];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    // get a stream
                    var stream = fileContent.InputStream;
                    // and optionally write the file to disk
                    var fileName = Request.Files[0].FileName;
                    var path = Path.Combine(Server.MapPath("~/userImages"), fileName);
                    using (var fileStream =System.IO.File.Create(path))
                    {
                        stream.CopyTo(fileStream);
                    }

                    admin adm = db.admins.Single(ad => ad.name == User.Identity.Name);
                    string oldFileName = adm.image;
                    string oldPath = Server.MapPath("~/") + oldFileName.Replace("/", "\\");
                    if (oldFileName != "/userImages/default.png")
                    {
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }

                    }
                    if (adm != null)
                    {
                        adm.image = "/userImages/" + fileName;
                        db.Entry(adm).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    HttpCookie oldAuthCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    FormsAuthenticationTicket mytiket = FormsAuthentication.Decrypt(oldAuthCookie.Value);
                    var newTicket = new FormsAuthenticationTicket
                       (mytiket.Version,
                        mytiket.Name, 
                        mytiket.IssueDate,
                        mytiket.Expiration, 
                        mytiket.IsPersistent,
                        "/userImages/" + fileName,"/admin");
                    FormsAuthentication.SignOut();
                    Session.Abandon();
                    var isPersistent = FormsAuthentication.Decrypt(oldAuthCookie.Value).IsPersistent;
                    FormsAuthenticationTicket oldticket = FormsAuthentication.Decrypt(oldAuthCookie.Value);
                    string oldCookieName = oldticket.Name;
                    var newticket = new FormsAuthenticationTicket
                        (oldticket.Version, oldticket.Name, oldticket.IssueDate, oldticket.Expiration, oldticket.IsPersistent, "/userImages/" + fileName, "/admin");
                    oldAuthCookie.Value = FormsAuthentication.Encrypt(newticket);
                    Request.Cookies.Remove(oldCookieName);
                    if (isPersistent) oldAuthCookie.Expires = newticket.Expiration;
                    oldAuthCookie.Path = newticket.CookiePath;
                    Response.Cookies.Add(oldAuthCookie); 
                    return Json(true,JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            
        }
        catch (Exception)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
           return Json(false,JsonRequestBehavior.AllowGet);

        }

    }
        public JsonResult Add(admin p)
        {
            if (p.name.Length < 3 || p.name.Length > 15 || p.password.Length < 8 || p.password.Length > 30)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            try
            {               
                if(db.admins.Any(ad=>ad.mail==p.mail ||ad.name==p.name))
                {
                    return Json(new { result = false }, JsonRequestBehavior.AllowGet);
                }
                int count = db.admins.Count();
                p.periority =(byte)(count + 1);
                p.dateOfJoin = DateTime.Now;
                p.lastDoctorComplainsViewedDate = DateTime.Now;
                p.lastPatientComplainsVieweddate = DateTime.Now;
                p.lastGeneralComplainViewedDate = DateTime.Now;
                p.image = "/userImages/default.png";
                db.admins.Add(p);
                Membership.CreateUser(p.name,p.password,p.mail);
                Roles.AddUserToRole(p.name,"admin");
                db.SaveChanges();
                return Json(new {result=true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult validateUserNameProfile(string userName)
        {
            userName = userName.Trim();
            if (userName==User.Identity.Name)
            {
                bool isFound = db.admins.Any(ad => ad.name == userName);
                if (isFound)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = false }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult validateUserName(string userName, string type, string operation)
        {

                bool isFound=true;
                userName = userName.Trim();
                if(type=="name")
                {
                    if(operation=="add")
                    {
                        isFound = db.admins.Any(ad => ad.name == userName);
                    }
                    else
                    {
                        isFound = db.admins.Any(ad => ad.name == userName) && !(User.Identity.Name == userName);
                    }
                }
                else if (type == "email")
                {
                    if (operation == "add")
                    {
                        isFound = db.admins.Any(ad => ad.mail == userName);
                    }
                    else
                    {
                        string currentMail = db.admins.SingleOrDefault(a => a.name == User.Identity.Name).mail;
                        isFound = db.admins.Any(ad => ad.mail == userName)&&!(currentMail==userName);
                    }
                }
                if (isFound)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

        }
        public JsonResult Update(admin p)
        {
            try
            {
                if(p.name.Length<3 ||p.name.Length>30 ||p.password.Length<8||p.password.Length>25)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                 admin oldAdmin = db.admins.FirstOrDefault(d => d.name == User.Identity.Name);
                ((IObjectContextAdapter)db).ObjectContext.Detach(oldAdmin);
                string oldPassword = oldAdmin.password;
                p.dateOfJoin = oldAdmin.dateOfJoin;
                p.lastPatientComplainsVieweddate= oldAdmin.lastPatientComplainsVieweddate;
                p.lastDoctorComplainsViewedDate = oldAdmin.lastDoctorComplainsViewedDate;
                p.lastGeneralComplainViewedDate = oldAdmin.lastGeneralComplainViewedDate;
                db.Entry(p).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                MembershipUser adminMember = Membership.GetUser();
                adminMember.Email = p.mail;
                if (p.password !=oldPassword)
                {
                    var generatedPass = adminMember.ResetPassword();
                    adminMember.ChangePassword(generatedPass, p.password);
                }
                if (p.name != oldAdmin.name)
                {
                    Guid userID = (Guid)adminMember.ProviderUserKey;
                    aspnet_Users user = db.aspnet_Users.Where(us => us.UserId == userID).FirstOrDefault();
                    user.UserName = p.name;
                    user.LoweredUserName = p.name.ToLower();
                }
                Membership.UpdateUser(adminMember);
                db.SaveChanges(); ;
                
                 if(p.name!=User.Identity.Name||p.password!=oldPassword)
                 {
                     FormsAuthentication.SignOut();
                     Session.Abandon();
                     var oldAuthCookie = Request.Cookies[FormsAuthentication.FormsCookieName];//my authenticated cookie
                     var isPersistent = FormsAuthentication.Decrypt(oldAuthCookie.Value).IsPersistent;
                     string userImage = FormsAuthentication.Decrypt(oldAuthCookie.Value).UserData;
                     FormsAuthenticationTicket oldticket = FormsAuthentication.Decrypt(oldAuthCookie.Value);
                     string oldCookieName = oldticket.Name;
                     var cookie = FormsAuthentication.GetAuthCookie(p.name, isPersistent);
                     var currentticket = FormsAuthentication.Decrypt(cookie.Value);
                     var newticket = new FormsAuthenticationTicket
                         (oldticket.Version, currentticket.Name, oldticket.IssueDate, oldticket.Expiration, oldticket.IsPersistent, userImage,"/admin");
                     cookie.Value = FormsAuthentication.Encrypt(newticket);
                     Request.Cookies.Remove(oldCookieName);
                     if (isPersistent) cookie.Expires = newticket.Expiration;
                     cookie.Path = newticket.CookiePath;
                     Response.Cookies.Add(cookie);
                 }
                return Json(new { mess = "your profile is updated successfully" ,result=true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false,JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Delete(byte ID)
        {
            try
            {
                admin p = db.admins.Single(ad => ad.periority == ID);
                byte deleter = (db.admins.Single(ad => ad.name == User.Identity.Name)).periority;
                byte deleted = p.periority;
                if(deleted<deleter)
                {
                    return Json(new { result = false, mess = "عفوا ,يمكنك فقط حذف المسؤلين الذين لهم اولوية اقل منك" }, JsonRequestBehavior.AllowGet);
                }
                else if (deleted ==deleter)
                {
                    return Json(new { result = false,confirm="confirm", mess = "لاحظ انه فى هذه الحالة سوف تقوم بالغاء حسابك تماما وبالتالى سوف تقوم بالخروخ تماما من الموقع" }, JsonRequestBehavior.AllowGet);
                }
                Membership.DeleteUser(p.name);
                db.deleteAdmin(ID);
                db.SaveChanges();
                db.rearrangeID("admins","periority");
                db.SaveChanges();
                string fileName = p.image;
                string path = Server.MapPath("~/") + fileName.Replace("/", "\\");
                if (fileName != "/userImages/default.png")
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult ConfirmDelete(byte ID)
        {
            try
            {
                admin p = db.admins.Single(ad => ad.periority == ID);
                byte deleter = (db.admins.Single(ad => ad.name == User.Identity.Name)).periority;
                byte deleted = p.periority;

                if (deleted == deleter)
                {
                    Membership.DeleteUser(p.name);
                    db.deleteAdmin(ID);
                    db.SaveChanges();
                    string fileName = p.image;
                    string path = Server.MapPath("~/") + fileName.Replace("/", "\\");
                    if (fileName != "/userImages/default.png")
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }

                    }
                    HttpContext.Response.Cookies.Clear();
                    FormsAuthentication.SignOut();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
    }
}