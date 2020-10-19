using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using tabeeb.Areas.users.languages;
using tabeeb.Areas.users.Models;
using tabeeb.Controllers;
using tabeeb.Models;
namespace tabeeb.Areas.users.Controllers
{
    //this controller handle 3 pages for patient(user)[register,login,profile]
    //this pages only available for logged and registered patients
    [areaAuthorize("users", Roles = "user")]
    public class patientController : mainController
    {       
        //profile page of internal authenticated user
        public ActionResult profile()
        {
            try
            {//get patient record from database
                patient currentpatient = getCurrentpatient();
                return View(currentpatient);
            }
            catch(Exception)
            {//redirect to mail users site
                return Redirect("/"+defaultPathForUsersArea);
            }
        }
        [HttpPost]
        //update profile page
        public ActionResult profile(patient patient)
        {
           if (ModelState.IsValid)//if data is validated
            {
                try
                {                  
                    db.Entry(patient).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    string oldUserName = Request.getUserCookieData().username;
                    if(oldUserName!=patient.username)
                    {//if username is changed then logout
                       return logout();
                    }
                    return View(patient);
                }
                catch (Exception)
                {
                    return View(patient);
                }
            }
            else
            {
                return View(patient);
            }
        }
        //profile page of external authenticated user
        public ActionResult externalProfile()
        {
            patient patient = getCurrentpatient();
            externaluserProfile exprofile = new externaluserProfile();
            exprofile.birthDate = patient.birthDate;
            exprofile.type = patient.type;
            exprofile.phone = patient.phone;
            return View(exprofile);
        }
        [HttpPost]
        //update external profile page
        public ActionResult externalProfile(externaluserProfile externalPatient)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    patient currentPatient = getCurrentpatient();
                    currentPatient.birthDate = externalPatient.birthDate;
                    currentPatient.phone = externalPatient.phone;
                    currentPatient.type = externalPatient.type;
                    UpdateFields(currentPatient, db.Entry(currentPatient), p => p.type, p => p.birthDate, p => p.phone);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                }
                return View(externalPatient);
            }
            catch(Exception)
            {
                ModelState.AddModelError(string.Empty,Resource1.updateProfileFaild);
                return View(externalPatient);
            }
        }
        [AllowAnonymous]
        //register page
        public ActionResult register()
        {
            if (isUserAuthenticated())
            {
                return Redirect("/"+defaultPathForUsersArea);
            }
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        //update register page
        public ActionResult register(patient patient)
        {
            if (ModelState.IsValid)
            {
                try
                {//add new record to database
                    patient.id = Guid.NewGuid();
                    patient.dateOfJoin = DateTime.Now;
                    db.patients.Add(patient);
                    db.SaveChanges();//add patient data
                    Membership.CreateUser(patient.username, patient.password,patient.mail);
                    Roles.AddUserToRole(patient.username, "user");//add patient to role
                    //add patient cookie
                    addAuthCookie(patient,true);
                    return Redirect("/"+defaultPathForUsersArea);
                }
                catch (Exception)
                { //add error to html view
                    ModelState.AddModelError(string.Empty,tabeeb.Areas.users.languages.Resource1.registerFaild);
                    return View(patient);
                }
            }
            else
            {
                return View(patient);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        /*register for external user by [facebook|gmail]*/
        public JsonResult externalRegister(externalPatient exPatient)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    patient patient = null;
                    Task checkUserIsRegisteredAtDatabaseTask = new Task(() =>
                    {
                        patient = db.patients.FirstOrDefault
                            (p => p.providerName == exPatient.providerName && p.providerID == exPatient.providerID);
                    });
                    checkUserIsRegisteredAtDatabaseTask.Start();                   
                    string format = (exPatient.birthDate.Contains("-")) ? "yyyy-mm-dd" : "mm/dd/yyyy";
                    var birthDate = DateTime.ParseExact(exPatient.birthDate, format, System.Globalization.CultureInfo.InvariantCulture);
                    patient newPatient = new patient();
                    newPatient.dateOfJoin = DateTime.Now;
                    newPatient.id = Guid.NewGuid();
                    newPatient.username = exPatient.username;
                    newPatient.type = exPatient.type;
                    newPatient.providerName = exPatient.providerName;
                    newPatient.providerID = exPatient.providerID;
                    newPatient.phone = exPatient.phone;
                    newPatient.mail = exPatient.mail;
                    newPatient.birthDate = birthDate;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.patients.Add(newPatient);//ad patient
                    if(!checkUserIsRegisteredAtDatabaseTask.IsCompleted)
                    {
                        checkUserIsRegisteredAtDatabaseTask.Wait();
                    }
                    if(patient!=null)
                    {//this user is already registered before
                        addAuthCookie(patient, true);
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    //username of external user will be [providerName+providerID]->it is unique name
                    string uniqueNameForExternalUser=newPatient.providerName+""+newPatient.providerID;
                    Membership.CreateUser(uniqueNameForExternalUser, newPatient.providerID, newPatient.mail);
                    Roles.AddUserToRole(uniqueNameForExternalUser, "user");//add patient to role
                    db.SaveChanges();
                    //add patient cookie
                    addAuthCookie(newPatient, true);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        /*logout from user site*/
        public ActionResult logout()
        {
            //FormsAuthentication.SignOut();
            var myCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            myCookie.Path = "/" + defaultPathForUsersArea;
            HttpContext.Response.Cookies.Add(myCookie);
            return Redirect("/"+defaultPathForUsersArea);
        }
        [AllowAnonymous]
        [HttpPost]
        /*login for external user by [facebook|gmail]*/
        public JsonResult externalLogin(string providerID, string providerName)
        {
            try
            {
                patient patient = db.patients.FirstOrDefault(p=>p.providerName == providerName && p.providerID == providerID);
                if (patient != null)
                {
                    addAuthCookie(patient, true);
                    return Json(true,JsonRequestBehavior.AllowGet);
                }

                else
                {                   
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        /*login page*/
        public ActionResult login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        /*login internal user to website*/
        public ActionResult login(string name, string password, bool? rememberMe)
        {
            bool isPresist=(rememberMe == null) ?false: true;
            patient patientFounded = db.patients.FirstOrDefault(p =>p.password == password && p.username == name &&p.providerName==null);
            if (patientFounded != null)
            {
                 try
               {
                   addAuthCookie(patientFounded, isPresist);
                   return Redirect("/"+defaultPathForUsersArea);
               }
                catch(Exception)
               {
                   TempData["error"] = Resource1.userNameOrPasswordNotValid;
                    return View();
               }
            }
                
            else
            {
                TempData["error"] = Resource1.userNameOrPasswordNotValid;
                return View();
            }
        }
        [HttpPost]
        [AllowAnonymous]
        /*check if username or phone is already exists at database*/
        public JsonResult IsUserNameOrPhoneExists(string value, string propertyType)
        {
            if (propertyType == "username")
            {
                if (User.Identity.IsAuthenticated)
                    return Json(db.patients.Any(d => d.username == value && d.username != User.Identity.Name && d.providerName == null), JsonRequestBehavior.AllowGet);
                else
                    return Json(db.patients.Any(d => d.username == value), JsonRequestBehavior.AllowGet);
            }

            else
            {
                if (User.Identity.IsAuthenticated)
                    return Json(db.patients.Any(d => d.phone == value && d.username != User.Identity.Name && d.providerName == null), JsonRequestBehavior.AllowGet);
                else
                    return Json(db.patients.Any(d => d.phone == value), JsonRequestBehavior.AllowGet);
            }
        }
        /*get patient(user) appointements*/
        public ActionResult appointements(bool type=true)
        {
            /*
             *type=true->function will return the booked appointements of this user at future
             *type=false->function will return the booked appointements of this user at past
             */
            try
            {
                if (type) ViewBag.currentAppoint = "present";//present and future appointements
                else ViewBag.currentAppoint = "past";//previous appointements
                userCookieData userData = Request.getUserCookieData();
                Guid currentPtID = (userData.providerName == null)
                ? db.patients.SingleOrDefault(p => p.username == userData.username).id
                : db.patients.SingleOrDefault(p => p.providerName == userData.providerName && p.providerID == userData.providerID).id;
                List<reservingData> reservingData=null;
                if(currentLanguage=="en")
                {
                    reservingData=(type)
                        ?db.reservings
                        .Where(r => r.patientID == currentPtID&&r.reservingDate >= DateTime.Now)
                        .Select(res=>new reservingData{
                            name=res.doctor.doctorInfo.fnameEng+" "+res.doctor.doctorInfo.lnameEng,
                            clinicAddress=res.doctor.doctorInfo.clinicAddressEng,
                            patientName=res.patientName,
                            id=res.id,
                            reservingDate=res.reservingDate,
                            interval=res.interval}).ToList()
                        :db.reservings
                        .Where(r => r.patientID == currentPtID&&r.reservingDate < DateTime.Now)
                        .Select(res=>new reservingData{
                            name=res.doctor.doctorInfo.fnameEng+" "+res.doctor.doctorInfo.lnameEng,
                            clinicAddress=res.doctor.doctorInfo.clinicAddressEng,
                            patientName=res.patientName,
                            id=res.id,
                            reservingDate=res.reservingDate,
                            interval=res.interval}).ToList();
                }
                else
                {
                     reservingData=(type)
                        ?db.reservings
                        .Where(r => r.patientID == currentPtID&&r.reservingDate >= DateTime.Now)
                        .Select(res=>new reservingData{
                            name=res.doctor.doctorInfo.fnameAr+" "+res.doctor.doctorInfo.lnameAr,
                            clinicAddress=res.doctor.doctorInfo.clinicAddressAr,
                            patientName=res.patientName,
                            id=res.id,
                            reservingDate=res.reservingDate,
                            interval=res.interval}).ToList()
                        :db.reservings
                        .Where(r => r.patientID == currentPtID&&r.reservingDate < DateTime.Now)
                        .Select(res=>new reservingData{
                            name=res.doctor.doctorInfo.fnameAr+" "+res.doctor.doctorInfo.lnameAr,
                            clinicAddress=res.doctor.doctorInfo.clinicAddressAr,
                            patientName=res.patientName,
                            id=res.id,
                            reservingDate=res.reservingDate,
                            interval=res.interval}).ToList();
                }     
                return View(reservingData);
            }
            catch
            {
                return Redirect("/"+defaultPathForUserSite);
            }
        }
        [HttpPost]
        /*cancel booked appointement*/
        public JsonResult cancelPatientAppointement(Guid id)
        {
            try
            {              
                patient currentPatient= getCurrentpatient();
                reserving res = currentPatient.reservings.FirstOrDefault(r => r.id == id);
                if(res==null)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
               
               db.reservings.Remove(res);
               db.SaveChanges();             
               return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch(Exception)
            {
                return Json(false,JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        /*remember password page*/
        public ActionResult rememberPassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        /*submit remember password page data*/
        public ActionResult rememberPassword(bool verificationWay, string userNameOrEmail, string phone="")
        {
            try
            {
                if (verificationWay)
                {//send by email
                    if(userNameOrEmail.Length<3)
                    {
                        ViewBag.error = Resource1.enterValidData;
                        return View();
                    }
                    patient patient = db.patients.FirstOrDefault
                        (p => p.providerName==null&&(p.username==userNameOrEmail||p.mail==userNameOrEmail));
                    if (patient != null)
                    {
                        //check for request if it is founded,and then add new request if it's not founded or expired
                        checkForPatientRequest(patient,"email");                
                        return View(model: new Tuple<string, string>("email",patient.mail));
                    }
                    else
                    {
                        ViewBag.error = Resource1.noDataAboutUsernameOrEmail;
                        return View();
                    }
                }
                else
                {//send by phone
                    Regex rgxPhone = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{5})$");
                    if (!rgxPhone.IsMatch(phone))
                    {
                        ViewBag.error = Resource1.enterValidData;
                        return View();                       
                    }                    
                    patient patient = db.patients.FirstOrDefault(d =>d.providerName==null&&d.phone == phone);
                    if (patient!=null)
                    {
                        checkForPatientRequest(patient,"phone");
                        return View();
                    }
                    else
                    {
                        ViewBag.error = Resource1.noDataAboutPhone;
                        return View();
                    }
                    
                }
            }
            catch (Exception)
            {
                ViewBag.error =Resource1.problemOnSendingResetLink;
                return View();
            }
        }
        [AllowAnonymous]
        /*change password page*/
        public ActionResult changePassword()
        {
            try
            {
                string userID = Request.Url.Query.Replace("?uid=", "");
                if (userID == "") throw new Exception();
                return View(model: userID);
            }
            catch (Exception)
            {
                return RedirectToAction("/"+defaultPathForUserSite);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        /*submit change password page data*/
        public ActionResult changePassword(string userID, string password)
        {
            try
            {
                Guid requestID = Guid.Parse(userID);
                resetPasswordRequest request = db.resetPasswordRequests.FirstOrDefault(r => r.id == requestID);
                if (request != null)
                {
                    patient patient = db.patients.FirstOrDefault(d => d.id == request.userID);
                    MembershipUser docMember = Membership.GetUser(patient.username);
                    var generatedPass = docMember.ResetPassword();
                    docMember.ChangePassword(generatedPass, password);
                    patient.password = password;
                    db.Entry(patient).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(request).State = System.Data.Entity.EntityState.Deleted;
                    Membership.UpdateUser(docMember);
                    db.SaveChanges();
                    return RedirectToAction("login");
                }
                else
                {
                    ViewBag.error = Resource1.linkExpired;
                    return View(model: userID);
                }

            }
            catch (Exception)
            {
                ViewBag.error =Resource1.problemOnDoingOperationAtServer;
                return View(model: userID);
            }
        }
        /*this function check if this user have made request before
            * if user has alraedy requests ,then check if that request has been expired ot not
         */
        private void checkForPatientRequest(patient patient, string verificationWay)
        {
            //add reset password request 
            resetPasswordRequest oldRequest =
                db.resetPasswordRequests.FirstOrDefault(r => r.userID == patient.id && r.entity == false);
            if (oldRequest == null)
            {
                oldRequest = addPatientResetPasswordRequest(patient, verificationWay);
            }
            else
            {//request is found
                if (DateTime.Now.Day > oldRequest.requestTime.Day + 3)
                {//request link is is expired
                    db.Entry(oldRequest).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    oldRequest = addPatientResetPasswordRequest(patient, verificationWay);
                }
            }
            if (verificationWay == "email")
            {
                string resetLink = Request.getBaseUrl() + "/" + defaultPathForUserSite + "/changePassword?uid=" + oldRequest.id;
                string body = renderPartialViewToString("mailView", new Tuple<string, string>(patient.username, resetLink));
                sendMailTo(patient.mail, Resource1.passwordRecoveryTitleToEmail, body, "async");
            }
            else
            {
                //send phone message
            }

        }
        /*add patient request to remember password*/
        private resetPasswordRequest addPatientResetPasswordRequest(patient patient, string verificationWay)
        {
            resetPasswordRequest request = new resetPasswordRequest();
            request.id = Guid.NewGuid();
            request.userID = patient.id;
            request.entity = false;
            request.requestTime = DateTime.Now;
            if(verificationWay=="phone")
            {
                Random randNumber=new Random();
                byte[]code=new byte[5];
                randNumber.NextBytes(code);
                request.phoneCode = string.Join("", code);
            }
            db.resetPasswordRequests.Add(request);
            db.SaveChanges();
            return request;
        }
        [AllowAnonymous]
        /*get patient views of current logged in patient*/
        public JsonResult getCurrentPatientView(Guid docID)
        {
            try
            {
                Guid currentPatinetID = Guid.Empty;
                byte currentPatientRate = 0;
                string currentPatientComment = "";
                bool isAuthenticated=false;
                bool canPatientMakeRateForThisDoctor = false;
                if (isUserAuthenticated())
                {
                    isAuthenticated=true;
                    patient currentPatient = getCurrentpatient();
                    patientView currPtView = currentPatient.patientViews.FirstOrDefault(pv => pv.doctorID == docID);
                    if (currPtView != null)
                    {
                        currentPatientRate = currPtView.rate;
                        currentPatientComment = currPtView.comment;
                        currentPatinetID = currPtView.patientID;
                        canPatientMakeRateForThisDoctor = currentPatient.reservings.Any(r => r.doctorID == docID && r.done);
                    }
                }
                return Json(new { isAuthenticated = isAuthenticated,
                                  currentPtRate = currentPatientRate, 
                                  currentPtComment = currentPatientComment,
                                  ptID = (currentPatinetID==Guid.Empty)?"0":currentPatinetID.ToString(),
                                  canRate=canPatientMakeRateForThisDoctor
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        /*add rate that marked by current patient*/
        public JsonResult addPatientRate(byte rate,Guid doctorID)
        {
            if (rate > 5 || rate < 0) return Json(false, JsonRequestBehavior.AllowGet);
            try
            {
                if (!isUserAuthenticated())
                    return Json(false, JsonRequestBehavior.AllowGet);
                patient currentPatient = getCurrentpatient();
                if (!currentPatient.reservings.Any(r => r.doctorID == doctorID&&r.done))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                patientView foundedPatientView = currentPatient.patientViews.FirstOrDefault(pv=>pv.doctorID==doctorID);
                if (foundedPatientView!=null)
                {//make rate updating                 
                    foundedPatientView.rate = rate;
                    UpdateFields<patientView>(foundedPatientView, db.Entry(foundedPatientView), pv => pv.rate);
                }
                else
                {//add new rate
                    foundedPatientView = new patientView();
                    foundedPatientView.rate = rate;
                    foundedPatientView.doctorID = doctorID;
                    foundedPatientView.patientID = currentPatient.id;
                    db.patientViews.Add(foundedPatientView);                   
                }
                db.SaveChanges();//save update or add
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        /*add comment that written by current logged patient*/
        public JsonResult addPatientComment(Guid doctorID, string comment)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comment)||string.IsNullOrEmpty(comment))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                comment = comment.Trim();
                if (!isUserAuthenticated())
                    return Json(false, JsonRequestBehavior.AllowGet);
                patient currentPatient = getCurrentpatient();
                if (!currentPatient.reservings.Any(r => r.doctorID == doctorID && r.done))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                patientView foundedPatientView = currentPatient.patientViews.FirstOrDefault(pv => pv.doctorID == doctorID);
                if (foundedPatientView != null)
                {//make rate updating                 
                    foundedPatientView.comment = comment;
                    UpdateFields<patientView>(foundedPatientView, db.Entry(foundedPatientView), pv => pv.comment);
                    // db.Entry(foundedPatientView).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {//add new rate
                    foundedPatientView = new patientView();
                    foundedPatientView.comment = comment;
                    foundedPatientView.rate = 0;
                    foundedPatientView.doctorID = doctorID;
                    foundedPatientView.patientID = currentPatient.id;
                    db.patientViews.Add(foundedPatientView);
                }
                db.SaveChanges();//save update or add
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}