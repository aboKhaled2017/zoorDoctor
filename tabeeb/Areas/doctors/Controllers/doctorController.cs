using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using tabeeb.Areas.doctors.languages;
using tabeeb.Areas.doctors.Models;
using tabeeb.Controllers;
using tabeeb.Models;
namespace tabeeb.Areas.doctors.Controllers
{
    //this class handle all doctors operations
    public partial class doctorController : mainController
    {
        //function is executed before each time any function is executed in this class
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //this function here will set the current language each time before any function is executed
            string language = "";
            if (isDoctorAuthenticated)
            {//get language from authentication cookie of logged user
                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                language = ticket.UserData.Split(',')[0].Replace("lang=", "");
            }
            else
            {
                //get language from cookie of Anonymous user
                var cookie = Request.Cookies[doctorCookieDataName];
                language = (cookie != null) ? cookie.Values.Get("language") : "ar";
            }
            setCurrentLanguage(language);
        }
        /*action result functions*/  
        [AllowAnonymous]
        /*change language of doctors site*/
        public ActionResult changeLanguage(string language)
        {
            setCurrentLanguage(language);
            if (isDoctorAuthenticated)
            {//set doctor language to authenticated cookie of logged user
                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                var ticket = FormsAuthentication.Decrypt(cookie.Value);               
                var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, DateTime.Now.AddYears(100), ticket.IsPersistent, "lang=" + language, "/" + defaultPathForDoctorsArea);
                cookie.Value = FormsAuthentication.Encrypt(newTicket);
                if (ticket.IsPersistent) cookie.Expires = newTicket.Expiration;
                cookie.Path = newTicket.CookiePath;
                Response.Cookies.Set(cookie);
            }
            else
            {//create temporary cookie untill doctor will be authenricated to save language value
                //this cookie is created for Anonymous users that are not logged untill now
                var cookie = Request.Cookies[doctorCookieDataName];
                if (cookie != null)
                {//if cookie is already exists
                    cookie.Values.Set("language", language);
                    cookie.Path = "/" + defaultPathForDoctorsArea;
                    Response.Cookies.Set(cookie);
                }
                else
                {
                    cookie = new HttpCookie(doctorCookieDataName);
                    cookie.Path = "/" + defaultPathForDoctorsArea;
                    cookie.Values.Add("language", language);
                    Response.Cookies.Add(cookie);
                }
            }
            if (Request.UrlReferrer == null) return Redirect("/" + defaultPathForDoctorsArea);
            return Redirect(Request.UrlReferrer.ToString());
        }
        [AllowAnonymous]
        //[OutputCache(Duration =int.MaxValue)]
        //page of term of use for doctors site
        public ActionResult termOfUse()
        {
            return View();
        }      
        [AllowAnonymous]
        //[OutputCache(Duration =int.MaxValue)]
        //reset password page
        public ActionResult resetPassword()
        {           
            return View();
        }       
        [AllowAnonymous]
        [HttpPost]
        //reset password operation
        public ActionResult resetPassword(string userNameOrEmail)
        {
            try
            {
                doctor doctor = db.doctors.FirstOrDefault(d => d.username == userNameOrEmail || d.mail == userNameOrEmail);
                if(doctor!=null)
                {
                    //add reset password request record to database
                    //get old request for that user is founded
                    resetPasswordRequest oldRequest = db.resetPasswordRequests.FirstOrDefault(r => r.userID == doctor.id&&r.entity==true);
                    if (oldRequest == null)
                    {//user has no request at databse before
                        //add new request record to database
                        oldRequest = new resetPasswordRequest();
                        oldRequest.id = Guid.NewGuid();
                        oldRequest.userID = doctor.id;
                        oldRequest.entity = true;
                        oldRequest.requestTime = DateTime.Now;
                        db.resetPasswordRequests.Add(oldRequest);
                        db.SaveChanges();
                    }
                    else
                    {//request is already found
                        //request will be expired after 3 days
                        if (DateTime.Now.Day > oldRequest.requestTime.Day + 3)
                        {//request link is is expired
                            db.Entry(oldRequest).State = System.Data.Entity.EntityState.Deleted;
                            db.SaveChanges();
                            oldRequest = new resetPasswordRequest();
                            oldRequest.id = Guid.NewGuid();
                            oldRequest.userID = doctor.id;
                            oldRequest.requestTime = DateTime.Now;
                            db.resetPasswordRequests.Add(oldRequest);
                            db.SaveChanges();
                        }
                    }
                    //make verification link and send it to user account mail
                    var link = Request.getBaseUrl()+"/"+defaultPathForDoctorsArea+"/changePassword?uid=" + oldRequest.id;
                    string body = renderPartialViewToString("mailView", new Tuple<string, string>(doctor.username, link));
                    globalController.sendMailTo(doctor.mail,tabeeb.Areas.users.languages.Resource1.passwordRecoveryTitleToEmail, body, "async");
                    return View(model: doctor.mail);
                }
                else
                {
                    ViewBag.error =tabeeb.Areas.users.languages.Resource1.noDataAboutUsernameOrEmail;
                    return View();
                }
                
            } 
            catch(Exception)
            {
                ViewBag.error =tabeeb.Areas.users.languages.Resource1.problemOnSendingResetLink;
                return View();
            }
        }
        [AllowAnonymous]
        //change password function
        public ActionResult changePassword()
        {//when user click on verification link on his/her account mail,
            //this link will redirect to this function 
            //the function receive user id and redirect to change password page to reset his password
            try
            {
                string userID = Request.Url.Query.Replace("?uid=", "");
                if (userID == "") throw new Exception();
                //pass uid to change password page
                return View(model: userID);
            }
            catch(Exception)
            {
                return RedirectToAction("register");
            }
        }
        [AllowAnonymous]
        [HttpPost]
        //change password page
        public ActionResult changePassword(string userID,string password)
        {
            //this function show form that withen user can change his password
            try
            {
                Guid requestID=Guid.Parse(userID);
                resetPasswordRequest request = db.resetPasswordRequests.FirstOrDefault(r => r.id == requestID);
                if(request!=null)
                {
                    doctor doctor = db.doctors.FirstOrDefault(d=>d.id==request.userID);
                    MembershipUser docMember = Membership.GetUser(doctor.username);
                        var generatedPass = docMember.ResetPassword();
                        docMember.ChangePassword(generatedPass, password);
                        doctor.password = password;
                        db.Entry(doctor).State = System.Data.Entity.EntityState.Modified;
                        db.Entry(request).State = System.Data.Entity.EntityState.Deleted;
                    Membership.UpdateUser(docMember);
                    db.SaveChanges();
                    return View(model:Resource1.passwordChangedSuccessfully);
                }
                else
                {
                    ViewBag.error =tabeeb.Areas.users.languages.Resource1.linkExpired;
                    return View(model:userID);
                }
                
            }    
            catch(Exception)
            {
                ViewBag.error = tabeeb.Areas.users.languages.Resource1.problemOnDoingOperationAtServer;
                return View(model:userID);
            }
        }
        //doctor profile page
        public ActionResult doctorPage()
        {
            try
            {            
                doctor doctor = getCurrentDoctor();
                if (doctor.doctorInfo != null)                   
                    return View(doctor.getDoctorPageInfo());
                else
                    //if user's profile does not completed or user not accepted yet,then user will redirected to information page
                    return RedirectToAction("profile");
            }
            catch(Exception)
            {
                return RedirectToAction("profile");
            }
        }
        //appointements setting page 
        public ActionResult appointementPage()
        {
            try
            {
                doctor currentDoctor = getCurrentDoctor();
                doctorInfo currDocInfo = currentDoctor.doctorInfo;
                bool bookingtype = currDocInfo.reservingType;
                List<appointementData> doctorAppointements = currentDoctor.appointements.Select(a =>
                    new appointementData 
                    { interval = a.interval, dateOfBooking = a.dateOfBooking}).ToList();
                Tuple<bool, List<appointementData>,byte> data = new Tuple<bool, List<appointementData>,byte>
                    (bookingtype, doctorAppointements,currDocInfo.numberOfScedualedDayes);
                return View(data);
            }
            catch (Exception)
            {
                return Redirect("/"+globalController.defaultPathForDoctorsArea);
            }

        }
        [AllowAnonymous]
        //register page
        public ActionResult register()
        {
            if (isDoctorAuthenticated) return RedirectToAction("doctorPage");
            //send all specialities to account page
            ViewBag.specialities = db.specialities.getSpecialityDataList().ToList();
            //send all cities to account page
            ViewBag.cities = db.cities.getCityDataList().ToList();
            //intialize all data to default
            ViewBag.cityID = 0;
            ViewBag.destrictID =0;
            ViewBag.spID = 0;
            ViewBag.subSPID =new string[]{};
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        //submit register page information to create account
        public ActionResult register(doctorAccountData doctor)
        {
            if (ModelState.IsValid)
            {                              
                try
                {
                    doctor newDoctor = new doctor();
                    newDoctor.id = Guid.NewGuid();
                    string imageName="";
                    //check if image is valid
                    if (!validateProfessionImage(newDoctor.id.ToString(),ref imageName))
                    {//image is not valid
                        ModelState.AddModelError("proImage",Resource1.professionImgNotValid);
                        fillViewDataForDoctorRegistrationForm(doctor.cityID,doctor.destrictID, doctor.spID,doctor.subSpID);
                        return View();
                    }
                    newDoctor.dateOfJoin = DateTime.Now;
                    newDoctor.bookingType = doctor.bookingType;
                    newDoctor.mail = doctor.mail;
                    newDoctor.password = doctor.password;
                    newDoctor.phone = doctor.phone;
                    newDoctor.proImage =imageName;
                    newDoctor.username = doctor.username;
                    newDoctor.destrictID = doctor.destrictID;
                    newDoctor.specialities = new List<speciality>();
                    //if the selected specialiy has sub specialities
                    if(doctor.subSpID!=null&&db.specialities.Any(s=>s.id==doctor.spID&&s.subSpecialites.Count>0))
                    {//the doctor selected sub specialities from main speciality
                        foreach (string sp in doctor.subSpID.ToList())
                        {
                            byte spID = byte.Parse(sp);
                            newDoctor.specialities.Add(db.specialities.Find(spID));
                        }
                    }
                    else
                    {//the doctor selected only main speciality
                        newDoctor.specialities.Add(db.specialities.Find(doctor.spID));
                    }
                    db.doctors.Add(newDoctor);//add new doctor account
                    userVerification entry = new userVerification();
                    entry.userID = newDoctor.id;
                    entry.verificationCode = Guid.NewGuid();
                    db.userVerifications.Add(entry);//add verification record
                    db.SaveChanges();
                    //send email verification
                    sendEmailVerification(entry.verificationCode.ToString(), newDoctor.mail,newDoctor.username);
                    //add new doctor to role
                    Membership.CreateUser(newDoctor.username, newDoctor.password, newDoctor.mail);
                    Roles.AddUserToRole(newDoctor.username, "doctor");
                    if (isDoctorAuthenticated) logout();
                    addAuthenticationCookie(newDoctor.username, true);
                    return RedirectToAction("profile");
                }
                catch (Exception)
                {
                    fillViewDataForDoctorRegistrationForm(doctor.cityID, doctor.destrictID, doctor.spID, new string[] { "" }.ToList());
                    ModelState.AddModelError(string.Empty,Resource1.registerFaild);
                    return View();
                }
            }
            else
            {      //registeration data is not valid         
                fillViewDataForDoctorRegistrationForm(doctor.cityID,doctor.destrictID,doctor.spID,doctor.subSpID);
                ModelState.AddModelError(string.Empty, Resource1.registerFaild);
                return View();
            }
        }
        //account page
        public ActionResult account()
        {
            //send all specialities to account page
            ViewBag.specialities = db.specialities.getSpecialityDataList().ToList();
            //send all cities to account page
            ViewBag.cities = db.cities.getCityDataList().ToList();
            //get the current doctor
            doctor oldDoctor = db.doctors.Single(doc => doc.username == User.Identity.Name);
            //get the account page data
            doctorAccountData currentDoctor = new doctorAccountData();
            currentDoctor.username = oldDoctor.username;
            currentDoctor.bookingType = oldDoctor.bookingType;
            currentDoctor.cityID =db.destricts.FirstOrDefault(d=>d.id==oldDoctor.destrictID).cityID;
            currentDoctor.destrictID = oldDoctor.destrictID;
            currentDoctor.mail = oldDoctor.mail;
            currentDoctor.password = oldDoctor.password;
            currentDoctor.phone = oldDoctor.phone;
            currentDoctor.proImage = oldDoctor.proImage;
            if(oldDoctor.specialities.First().superSpecialityID!=null)
            {//doctor has sub specialities
                currentDoctor.spID =(byte)oldDoctor.specialities.First().superSpecialityID;
                currentDoctor.subSpID = oldDoctor.specialities.Select(s =>s.id.ToString()).ToList();
            }
            else
            {//doctor has main speciality and not subspeciality
                currentDoctor.spID = oldDoctor.specialities.First().id;
                currentDoctor.subSpID = new List<string>();
            }
            return View(currentDoctor);
        }
        [HttpPost]
        //update account page data
        public ActionResult account(doctorAccountData updatedDoctor)
        {
            //send all specialities to account page
            ViewBag.specialities = db.specialities.getSpecialityDataList().ToList();
            //send all cities to account page
            ViewBag.cities = db.cities.getCityDataList().ToList();
            doctor oldDoctor = getCurrentDoctor();
            string oldpassword = oldDoctor.password;
            string oldusername = oldDoctor.username;
            if (ModelState.IsValid)
            {
                try
                {
                    string newImageName =oldDoctor.proImage;
                    //check if image is valid
                    if (validateProfessionImage(oldDoctor.id.ToString(), ref newImageName,oldDoctor.proImage))
                    {
                        oldDoctor.proImage = newImageName;
                    }
                    else
                    {//image is not valid ,then image will not be updated
                        ModelState.AddModelError("proImage",Resource1.professionImgNotUpdated);
                        return View(updatedDoctor);
                    }//update all data
                    oldDoctor.bookingType = updatedDoctor.bookingType;
                    oldDoctor.mail = updatedDoctor.mail;
                    oldDoctor.phone = updatedDoctor.phone;
                    oldDoctor.username = updatedDoctor.username;
                    oldDoctor.destrictID = updatedDoctor.destrictID;
                    db.doctors.Attach(oldDoctor);
                    db.Entry(oldDoctor).Collection(s => s.specialities).Load();
                    oldDoctor.specialities.Clear();
                    oldDoctor.specialities=new List<speciality>();
                    if(updatedDoctor.subSpID==null||updatedDoctor.subSpID.Count()==0)
                    {//doctor has no sub specialities
                        oldDoctor.specialities.Add(db.specialities.Find(updatedDoctor.spID));
                    }
                    else
                    {
                         foreach(var spid in  updatedDoctor.subSpID)
                        {
                            byte spID = byte.Parse(spid);
                            oldDoctor.specialities.Add(db.specialities.Find(spID));                           
                        }                      
                    }
                    db.Entry(oldDoctor).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    MembershipUser docMember = Membership.GetUser();
                    docMember.Email = oldDoctor.mail;
                    if (oldusername != updatedDoctor.username)//if username has been changed
                    {
                        Guid userID = (Guid)docMember.ProviderUserKey;
                        aspnet_Users user = db.aspnet_Users.Where(us => us.UserId == userID).FirstOrDefault();
                        user.UserName = updatedDoctor.username;
                        user.LoweredUserName = updatedDoctor.username.ToLower();
                    }//update user role
                    Membership.UpdateUser(docMember);
                    db.SaveChanges();
                    if (oldusername != updatedDoctor.username)
                    {//logout if username has been changed
                       return logout();
                    }
                    return View(updatedDoctor);
                }
                catch (Exception)
                {                   
                    ModelState.AddModelError(string.Empty,Resource1.updateOperationFaild);
                    return View(updatedDoctor);
                }               
            }
            else
            {
                return View(updatedDoctor);
            }
        }
        [HttpPost]
        //update password
        public ActionResult updatePassword(string password)
        {
            try
            {
                MembershipUser docMember = Membership.GetUser();
                doctor oldDoctor = getCurrentDoctorByUserName(docMember.UserName);
                string oldpassword = oldDoctor.password;
                if (oldpassword != password)
                {
                    var generatedPass = docMember.ResetPassword();
                    docMember.ChangePassword(generatedPass, password);
                    oldDoctor.password = password;
                    //only update password
                    UpdateFields<doctor>(oldDoctor,db.Entry(oldDoctor), d => d.password);
                }
                Membership.UpdateUser(docMember);
                db.SaveChanges();
                return RedirectToAction("account");
            }
            catch
            {
                return RedirectToAction("account");
            }
        }
        //doctor information page
        public ActionResult profile()
        {
            /*
             this function show information page that contains all information that related with 
             * doctor's clinic ,clinic address,doctor profile ,and so on....
             * this info is required for doctor card that will be displayed for patients
             */
            //data needed for page to be displayed
            ViewBag.professions = (currentLanguage=="en")?engEducationsNames:arEducationsNames;
            if (!isDoctorAuthenticated)
            {
                return RedirectToAction("register");
            }
            doctorInfo currentDocInfo = null;
            try
            {
                doctor doc = getCurrentDoctor();
                currentDocInfo =doc.doctorInfo;
                if (currentDocInfo != null)
                {
                    //true if profile is completed or already created
                    ViewBag.profileStat = true;
                    return View(currentDocInfo);
                }
                else
                {
                    //false if profile is not completed or not created
                    ViewBag.profileStat = false;
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.profileStat = false;
                return View();
            }  
        }
        [HttpPost]
        //update doctor information page
        public ActionResult profile(doctorInfo docInfo)
        {
            //data needed for page to be dispalyed
            ViewBag.professions = (currentLanguage == "en") ? engEducationsNames : arEducationsNames;
            string operation = "add";
            try
            {
                if(ModelState.IsValid)
                {
                    doctor doc = getCurrentDoctor();
                    docInfo.doctorID = doc.id;
                    //get old doctorInfo record if exists
                    doctorInfo isfoundDocInfo = db.doctorInfoes.Find(doc.id);
                    if (isfoundDocInfo == null)
                    {//add newdoctor info record
                        string imageName = "";
                        //validate if image profile is not valid
                        if (validateProfileImage(doc.id.ToString(), ref imageName))
                        {
                            docInfo.image = imageName;
                        }
                        else
                        {
                            //false if profile is not completed or not created
                            ViewBag.profileStat = false;
                            ModelState.AddModelError("image",Resource1.imgNotValid);
                            ModelState.AddModelError(string.Empty,Resource1.profileRegisterAddingFailed);
                            return View(docInfo);
                        }
                        db.doctorInfoes.Add(docInfo);
                        db.SaveChanges();
                        //true if profile is completed or already created
                        ViewBag.profileStat = true;
                        return View(docInfo);
                    }//end of add record
                    else
                    {//doctorInfo record will be updated
                        operation = "update";
                        string oldImage = isfoundDocInfo.image;
                        if (!validateProfileImage(doc.id.ToString(), ref oldImage, oldImage, operation))
                        {//image is invalid
                            ModelState.AddModelError(string.Empty,Resource1.imgNotUpdated);  
                        }
                        else
                        {//image is updated
                            docInfo.image = oldImage;
                        }
                        docInfo.stat = isfoundDocInfo.stat;
                        docInfo.profileIsAccepted = isfoundDocInfo.profileIsAccepted;
                        ((IObjectContextAdapter)db).ObjectContext.Detach(isfoundDocInfo);
                        db.Entry(docInfo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //true if profile is completed or already created
                        ViewBag.profileStat = true;
                        return View(docInfo);
                    }//end of update record                  
                }
                else
                {//modelstate is invalid
                    if (operation == "add")
                    {
                        ModelState.AddModelError(string.Empty,Resource1.profileRegisterAddingFailed);
                        ViewBag.profileStat = false;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty,Resource1.profileNotUpdated);
                    }
                    return View(docInfo);
                }
            }
            catch (Exception)
            {
                ViewBag.profileStat = false;
                ModelState.AddModelError(string.Empty,Resource1.serverOperationProblem);
                return View(docInfo);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        //login operation
        public JsonResult logIn(string userName, string password, bool checkBox)
        {
            try{
                bool isPresistent = (checkBox == false) ? false : true;
                doctor doc = getCurrentDoctorByUserName(userName);
                //the email will registered as name
                if (Membership.ValidateUser(userName, password) == true && doc != null)
                {//user is validated 
                    addAuthenticationCookie(userName,isPresistent);
                    if (!db.doctorInfoes.Any(d => d.doctorID == doc.id))
                    {//user not founded at database
                        return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
                    } 
                    //doctor is founded at database
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {//user not validated
                    return Json(new { result = false }, JsonRequestBehavior.AllowGet);
                }
            }
             catch(Exception)
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }           
        }
        //log out operation
        public ActionResult logout()
        {
            //remove authentication cookie from client site
            var myCookie = new HttpCookie(FormsAuthentication.FormsCookieName);          
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            myCookie.Path = "/"+globalController.defaultPathForDoctorsArea;
            HttpContext.Response.Cookies.Add(myCookie);
            return RedirectToAction("register");
        }
        [HttpPost]
        [AllowAnonymous]
        //send verifiaction message to user account
        public void sendEmailVerification(string id, string email,string name)
        {
            //string link = Url.Action("verifyEmail", "doctor", new { code = id }, "http");
            var link = Request.getBaseUrl()+ "/" + defaultPathForDoctorsArea + "/verifyEmail?code=" + id;
            string subject = Resource1.verifyAccount;
            string body = "<html>" +
                        "<body>" +
                          "<h2>"+Resource1.welcom+" "+name+"</h2>" +
                            Resource1.thankForRegisterEmail+"<a href=" + link + ">"+Resource1.clickHere+"</a>" +
                        "</body>" +
                        "</html>";
            sendMailTo(email, subject, body, "sync");

        }
        //send verifiaction message gain to user account
        public ActionResult reSendEmailVerification(string id, string email,string name)
        {
            sendEmailVerification(id, email,name);
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
            return Redirect("/" + defaultPathForDoctorsArea);
        }
        //verify if email of registered user
        public ActionResult verifyEmail(string code)
        {
            try
            {
                Guid verificationCode = Guid.Parse(code);
                var entry = db.userVerifications.Find(verificationCode);
                db.Entry(entry).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("doctorPage");
            }
            catch (Exception)
            {
                return RedirectToAction("doctorPage");
            }
        }
        /*private functions[these functions used by other functions at this class]*/
        //this function used to pass some needed data for some pages before being displayed
        private void fillViewDataForDoctorRegistrationForm(byte cityID, byte destrictID, byte docSpID, IEnumerable<string> docSupSpID)
        {
            ViewBag.cityID = cityID;
            ViewBag.destrictID = destrictID;
            ViewBag.spID = docSpID;
            ViewBag.subSPID = docSupSpID;
            //send all specialities to account page
            ViewBag.specialities = db.specialities.getSpecialityDataList().ToList();
            //send all cities to account page
            ViewBag.cities = db.cities.getCityDataList().ToList();
        }
        //function to add cookie 
        private void addAuthenticationCookie(string userName, bool presistent)
        {
            var cookie = FormsAuthentication.GetAuthCookie(userName, presistent);
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            string language = popDcotorCookieDataBeforAuthentication().Single(a => a.Key == "language").Value;
            var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, DateTime.Now.AddYears(100), ticket.IsPersistent, "lang=" + language, "/" + globalController.defaultPathForDoctorsArea);
            cookie.Value = FormsAuthentication.Encrypt(newTicket);
            if (presistent) cookie.Expires = newTicket.Expiration;
            cookie.Path = newTicket.CookiePath;
            Response.Cookies.Add(cookie);
        }
        //function to validate profession image of doctor account
        private bool validateProfessionImage(string doctorID, ref string imageName, string oldImageFileName = null)
        {
            var supportedTypes = new string[] { "png", "jpg", "jpeg", "gif", "PNG", "JPG", "GIF", "JPEG" };
            var fileName = "";
            if (isDoctorAuthenticated)
            {
                if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0 || oldImageFileName == null) return true;
                //the doctor will want to change his/here image
            }
            if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0 || Request.Files[0].ContentLength > maxProfessionImgSize)
            {
                ModelState.AddModelError(string.Empty, "image size must at most be " + maxProfessionImgSize+" bits");
                return false;
            }
            var fileContent = Request.Files[0];//byte[] 
            var fileExt = System.IO.Path.GetExtension(fileContent.FileName).Substring(1);
            if (!supportedTypes.Contains(fileExt)) return false;
            fileName = doctorID + '.' + fileExt;
            if (isDoctorAuthenticated)//the image will be updated for doctor record 
            {
                try
                {       //delete old image if exists
                    string oldPath = Server.MapPath("~/Areas/doctors/profImages/") + oldImageFileName;

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    // get a stream
                    var stream = fileContent.InputStream;
                    // and optionally write the file to disk                       
                    var path = Path.Combine(Server.MapPath("~/Areas/doctors/profImages"), fileName);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        stream.CopyTo(fileStream);
                    }
                    imageName = fileName;
                    return true;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty,ex.Message);
                    return false;
                }
            }
            else//the image is for new doctor will be added
            {
                try
                {
                    // get a stream
                    var stream = fileContent.InputStream;
                    // and optionally write the file to disk
                    var path = Path.Combine(Server.MapPath("~/Areas/doctors/profImages"), fileName);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return false;
                }
                imageName = fileName;
                return true;
            }

        }
        //function to validate personal image of registered doctor
        private bool validateProfileImage(string doctorID, ref string imageName, string oldImageFileName = null, string operation = "add")
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            var supportedTypes = new string[] { "png", "jpg", "jpeg", "gif", "PNG", "JPG", "GIF", "JPEG","blob"};
            var fileName = "";
            if (operation == "update")
            {//the doctor will not change his/here image(return true)
                if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0 || oldImageFileName == null) return true;
            }
            //not valid image
            if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0) return false;
            var fileContent = Request.Files[0];//byte[] image contents
            //file extension
            var fileExt = System.IO.Path.GetExtension(fileContent.FileName).Substring(1);
            //not valid extension
            if (!supportedTypes.Contains(fileExt)) return false;
            //image width and height must be 512 px
            using (var img = Image.FromStream(fileContent.InputStream))
            {
                if (img.Width != img.Height || fileContent.ContentLength>maxPersonalImgSize)
                {
                    return (false || operation == "update");
                }
            }
            fileContent.InputStream.Position = 0;
            fileName = doctorID + '.' + fileExt;//image name that will be stored in db
            if (operation == "update")//the image will be updated for doctorinfo record 
            {
                try
                {//delete old image if exists
                    string oldPath = Server.MapPath("~/Areas/doctors/doctorImages/") + oldImageFileName;
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    // get a stream                        
                    var stream = fileContent.InputStream;
                    // and optionally write the file to disk                       
                    var path = Path.Combine(Server.MapPath("~/Areas/doctors/doctorImages"), fileName);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        stream.CopyTo(fileStream);
                    }
                    imageName = fileName;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else//new image is added for new doctorinfo record
            {
                try
                {
                    // get a stream
                    var stream = fileContent.InputStream;
                    // and optionally write the file to disk
                    var path = Path.Combine(Server.MapPath("~/Areas/doctors/doctorImages"), fileName);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return false;
                }
                imageName = fileName;
                return true;
            }

        }
        //return the language of logged user at doctors site
        private string getDcotorLanguageBeforAuthentication()
        {
            var cookie = Request.Cookies[doctorCookieDataName];
            return  (cookie != null) ? cookie.Values.Get("language") : "ar";
        }
        //this function remove the cookie of the anonymous user and send it's data to the newlly added 
        //authentication cookie
        private Dictionary<string, string> popDcotorCookieDataBeforAuthentication()
        {
            var cookieData = new Dictionary<string, string>();
            var cookie = Request.Cookies[doctorCookieDataName];
            if (cookie == null)
            {
                cookieData.Add("language", "ar");
                return cookieData;
            }
            string language = (cookie != null) ? cookie.Values.Get("language") : "ar";
            cookieData.Add("language", language);
            cookie.Expires = DateTime.Now.AddDays(-1d);
            cookie.Path = "/" + globalController.defaultPathForDoctorsArea;
            Response.Cookies.Add(cookie);
            return cookieData;
        }
   
    }
}