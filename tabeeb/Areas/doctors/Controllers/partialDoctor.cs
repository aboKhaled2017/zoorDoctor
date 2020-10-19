using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using tabeeb.Models;
using tabeeb.Areas.doctors.languages;
using tabeeb.Controllers;
using System.Web;
using System.Drawing;
namespace tabeeb.Areas.doctors.Controllers
{
    //this class handle the other operations of doctors site
    public partial class doctorController 
    {                
        [AllowAnonymous]
        /*partial view result functions*/
        //get navbar menu html as string
        public MvcHtmlString navBarMenu()
        {
            string language = currentLanguage;
            //mark active page
            string activePage = Request.Path;
            activePage = activePage.Replace("/" + defaultPathForDoctorsArea + "/", "");
            StringBuilder menueLinksBuilder = new StringBuilder();
            //contact us li
            TagBuilder contacUsLi = new TagBuilder("li");
            contacUsLi.MergeAttribute("onclick", "$('#ContactUS').modal('show');");
            contacUsLi.InnerHtml = "<a href=\"#\" style=\"pointer-events:none\">" + Resource1.contactUs + "</a>";
            menueLinksBuilder.Append(contacUsLi);//li added
            //language li
            TagBuilder languageLi = new TagBuilder("li");
            string targetlang = (language == "en") ? "ar" : "en";
            string langText = (language == "en") ? "العربية" : "English";
            string langHref = "/" + defaultPathForDoctorsArea + "/changeLanguage?language=" + targetlang;
            TagBuilder langLink = new TagBuilder("a");
            langLink.MergeAttribute("href", langHref);
            langLink.SetInnerText(langText);
            languageLi.InnerHtml = langLink.ToString();
            menueLinksBuilder.Append(languageLi); //li added        
            if (isDoctorAuthenticated)//li for menu
            {//if user is authenticated this menu will added
                //menu of username links
                //two parts[a + ul]
                TagBuilder userNameLi = new TagBuilder("li");//li of menu
                userNameLi.AddCssClass("dropdown");
                StringBuilder usernameLiBuilder = new StringBuilder();
                //username link [a]=>first part of li
                TagBuilder usernameLink = new TagBuilder("a");
                usernameLink.AddCssClass("dropdown-toggle");
                Dictionary<string, string> linkeAttributes = new Dictionary<string, string>();
                linkeAttributes.Add("data-toggle", "dropdown");
                linkeAttributes.Add("role", "button");
                linkeAttributes.Add("aria-haspopup", "true");
                linkeAttributes.Add("aria-expanded", "false");
                usernameLink.MergeAttributes<string, string>(linkeAttributes);
                usernameLink.InnerHtml = User.Identity.Name + " <span class=\"caret\"></span>";
                usernameLiBuilder.Append(usernameLink.ToString());//link added
                /*start dropdown menu links
                 * secod part of li
                 */
                TagBuilder dropDownMenue = new TagBuilder("ul");
                dropDownMenue.AddCssClass("dropdown-menu");
                StringBuilder drobdownMenuBuilder = new StringBuilder();
                //account li
                TagBuilder accountLi = new TagBuilder("li");
                if (activePage == "account") accountLi.AddCssClass("active");
                accountLi.InnerHtml = "<a href=/" + defaultPathForDoctorsArea + "/account>" + Resource1.myAccount + "</a>";
                drobdownMenuBuilder.Append(accountLi);//li added
                //information li
                TagBuilder infoLi = new TagBuilder("li");
                if (activePage == "profile") infoLi.AddCssClass("active");
                infoLi.InnerHtml = "<a href=/" + defaultPathForDoctorsArea + "/profile>" + Resource1.myProfile + "</a>";
                drobdownMenuBuilder.Append(infoLi);//li added
                //doctor page li
                TagBuilder doctorpageLi = new TagBuilder("li");
                if (activePage == "doctorPage") doctorpageLi.AddCssClass("active");
                doctorpageLi.InnerHtml = "<a href=/" + defaultPathForDoctorsArea + "/doctorPage>" + Resource1.doctorPage + "</a>";
                drobdownMenuBuilder.Append(doctorpageLi);//li added
                //appointement page li
                TagBuilder appointementLi = new TagBuilder("li");
                if (activePage == "appointementPage") appointementLi.AddCssClass("active");
                appointementLi.InnerHtml = "<a href=/" + defaultPathForDoctorsArea + "/appointementPage>" + Resource1.myAppointement + "</a>";
                drobdownMenuBuilder.Append(appointementLi);//li added
                //logout li
                TagBuilder logoutLi = new TagBuilder("li");
                logoutLi.InnerHtml = "<a href=/" + defaultPathForDoctorsArea + "/logout>" + Resource1.logOut + "</a>";
                drobdownMenuBuilder.Append(logoutLi);//li added
                dropDownMenue.InnerHtml = drobdownMenuBuilder.ToString();
                /*end of dropdown menu elements*/
                usernameLiBuilder.Append(dropDownMenue.ToString());//ul added
                userNameLi.InnerHtml = usernameLiBuilder.ToString();
                menueLinksBuilder.Append(userNameLi);//li added
            }
            else
            {//if user is not rauthenticated
                //login li
                TagBuilder loginLi = new TagBuilder("li");
                loginLi.InnerHtml = "<a style=\"pointer-events:none\">" + Resource1.login + "</a>";
                loginLi.MergeAttribute("onclick", "$('#LogIn').modal('show');");
                //register li
                TagBuilder registerLi = new TagBuilder("li");
                if (activePage == "register") registerLi.AddCssClass("active");
                registerLi.InnerHtml = "<a href=/" + defaultPathForDoctorsArea + "/register>" + Resource1.register + "</a>";
                menueLinksBuilder.Append(loginLi);//li added
                menueLinksBuilder.Append(registerLi);//li added
            }
            TagBuilder mainUl = new TagBuilder("ul");
            mainUl.AddCssClass("nav");
            mainUl.AddCssClass("navbar-nav");
            mainUl.AddCssClass("navbar-right");
            mainUl.InnerHtml = menueLinksBuilder.ToString();
            StringBuilder mainHtml = new StringBuilder();
            mainHtml.Append(mainUl);
            return MvcHtmlString.Create(mainHtml.ToString());
        }
        [AllowAnonymous]
        //return partial html view that display doctor status
        public PartialViewResult doctorStatus()
        {
            var currentDoctor = getCurrentDoctor();
            string status = "notRegistered";
            Tuple<string, string, string,bool> data;
            if (isDoctorAuthenticated)
            {
                var entry = db.userVerifications.FirstOrDefault(v => v.userID == currentDoctor.id);
                if (entry != null)
                {
                    status = "emailNotVerified";
                    data = new Tuple<string, string, string,bool>(status, currentDoctor.mail, entry.verificationCode.ToString(),currentDoctor.isProfileCreated());
                    return PartialView(data);
                }
                else if (currentDoctor.doctorInfo != null && !currentDoctor.doctorInfo.profileIsAccepted)
                {
                    status = "notAccepted";
                }
                else if (currentDoctor.doctorInfo != null &&currentDoctor.doctorInfo.profileIsAccepted && !getCurrentDoctor().doctorInfo.stat)
                {
                    status = "notAllowed";
                }

            }
            data = new Tuple<string, string, string,bool>(status, db.websiteInformations.First().serviceNumber, null,(currentDoctor!=null&&currentDoctor.isProfileCreated()));
            return PartialView(data);
        }       
        /*json result Functions for ajax requests*/
        [AllowAnonymous]
        //get doctor information string
        public JsonResult InfoAboutDoc(Guid docID)
        {
            var data = (currentLanguage == "en")
            ? db.doctorInfoes.Select(d => new
            {
                aboutDoctor=d.aboutDocEng,
                d.doctorID,
                services=d.servicesEng,
                d.urls
            }).SingleOrDefault(d => d.doctorID == docID)
            : db.doctorInfoes.Select(d => new
            {
                aboutDoctor=d.aboutDocAr,
                d.doctorID,
                services=d.servicesAr,
                d.urls
            }).SingleOrDefault(d => d.doctorID == docID);
            return Json(new { aboutDocor= data.aboutDoctor, services = data.services, urls = data.urls }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        //get doctor appointements as json string
        public JsonResult getDoctorAppointement(Guid id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.appointements.Where(a => a.doctorID == id)
                .Select(a => new
                {
                    a.id,
                    a.doctorID,
                    a.interval,
                    a.dateOfBooking
                }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        //change booking type for doctor
        public JsonResult chnageBookingType()
        {
            try
            {
                var currentDoctor = getCurrentDoctor();
                //get all appointement of the old booking type for this doctor
                var doctorAppointements = currentDoctor.appointements.ToList();
                //modify current doctor booking type
                currentDoctor.doctorInfo.reservingType = !currentDoctor.doctorInfo.reservingType;
                db.Entry(currentDoctor.doctorInfo).State = System.Data.Entity.EntityState.Modified;
                db.appointements.RemoveRange(doctorAppointements);
                db.SaveChanges();
                return Json(new {status=true},JsonRequestBehavior.AllowGet);
            }
            catch(Exception)
            {
                  return Json(new {status=false},JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        //save doctor appointement
        public JsonResult saveAppointement(string appointementInterval, DateTime appointementDate, bool bookingType)
        {          
            appointementDate=appointementDate.Date;
            bool isdefaultAppointement=appointementDate<DateTime.Now.Date;
            /*
             booking type[based on priority(true)|based on prescriped time(false)]
             */
            try
            {
                doctor currentDoctor = getCurrentDoctor();
                //appointement will be validated based on booking type
                bool oldBookingtype = currentDoctor.bookingType;
                /*if the appointementDate id previous date
                this indicated that it is Default appointement fro all days*/
                appointement isFoundAppoint = (isdefaultAppointement)
                    ?currentDoctor.appointements.FirstOrDefault(a=>a.dateOfBooking== null)
                    :currentDoctor.appointements.FirstOrDefault(a=>a.dateOfBooking== appointementDate);
                /*
                 scenario will one of the 2 cases
                 * 1-if the appointement will added
                 * 2-if appointement will updated
                 * case(1)[check if added appointement is default one or normal one]
                 * case(2)[check if updated appointement is default or normal one]
                 * case(1,2)[if appointement id default one ,so delete all appointements that are equal to default one]
                 */
                //case(1)
                if(isFoundAppoint == null)
                {//add new appointement
                    //check that added appointements not equal to default appointements
                    appointement defaultAppointement = currentDoctor.appointements.FirstOrDefault(a => a.dateOfBooking == null);
                    if (defaultAppointement == null || defaultAppointement.interval != appointementInterval)
                    {
                        appointement p = new appointement();
                        p.doctorID = currentDoctor.id;
                        p.interval = appointementInterval;
                        if (isdefaultAppointement) p.dateOfBooking = null;
                        else p.dateOfBooking = appointementDate;
                        p.id = Guid.NewGuid();
                        db.appointements.Add(p);
                        db.SaveChanges();
                        if(isdefaultAppointement)
                        {//delete those appointements that are equal to default one
                            IEnumerable<appointement> equalAppointements = currentDoctor.appointements.Where(a => a.interval == appointementInterval&&a.dateOfBooking!=null);
                            db.appointements.RemoveRange(equalAppointements);
                        }
                        db.SaveChanges();
                    }                    
                }
                else//case(2)
                    /*
                     *case(2.1)[appointement is normal( not default) ,but it is equal to default,so remove it]
                     *case(2.2)[appointement is default one,so edit it,and remove any appointements that equal to default one]
                     *case(2.3)[appointement is normal(not default),and it is not equal to default,so edit it]
                     */
                {//update current appointement
                    //check that added appointements not equal to default appointements
                    appointement defaultAppointement = currentDoctor.appointements.FirstOrDefault(a => a.dateOfBooking == null);
                    if (!isdefaultAppointement && defaultAppointement != null && defaultAppointement.interval == appointementInterval)
                    {//remove this appointement if it is normal appointement and it is equal to default
                        db.Entry(isFoundAppoint).State = System.Data.Entity.EntityState.Deleted;
                    }
                    else if(isdefaultAppointement)
                    {
                        //edit if this appointement is default
                        isFoundAppoint.interval = appointementInterval;
                        db.Entry(isFoundAppoint).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //delete those appointements that are equal to default one
                        IEnumerable<appointement> equalAppointements = currentDoctor.appointements.Where(a => a.interval == appointementInterval&&a.dateOfBooking!=null);
                        //db.appointements.RemoveRange(equalAppointements);
                        db.appointements.RemoveRange(equalAppointements);                 
                    }
                    else
                    {
                        //edit if this appointement is default
                        isFoundAppoint.interval = appointementInterval;
                        db.Entry(isFoundAppoint).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        //change number of scedualed dayes for doctor
        public JsonResult changeScedualedDaysNumber(byte daysNumber)
        {
            try
            {
                if(daysNumber<5||daysNumber>255)
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                var currentDoctor = getCurrentDoctor();
                byte oldNumberOfScedualedDayes = currentDoctor.doctorInfo.numberOfScedualedDayes;
                currentDoctor.doctorInfo.numberOfScedualedDayes = daysNumber;
                db.Entry(currentDoctor.doctorInfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //if scedualed days will be increased
                if(daysNumber>oldNumberOfScedualedDayes)
                {
                    List<string> dates = new List<string>();
                    //return the array of dates of added scedualed dayes
                    for (byte i = oldNumberOfScedualedDayes; i < daysNumber; i++)
                    {
                        dates.Add(DateTime.Now.AddDays(i).ToShortDateString());
                    }
                    return Json(new { status = true, data = dates }, JsonRequestBehavior.AllowGet);
                }//if scedualed days will be decreased
                else if(daysNumber<oldNumberOfScedualedDayes)
                {
                    //this is maximum date of scedualed dayes
                    DateTime maxdate=DateTime.Now.AddDays(daysNumber-1);
                    //remove any appointement >max appointements
                    db.appointements.RemoveRange(currentDoctor.appointements.Where(a => a.dateOfBooking > maxdate&&a.dateOfBooking!=null));
                    db.SaveChanges();
                    return Json(new { status = true}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        //get doctor patient views as json string
        public JsonResult doctorPatientViews(int pageNumber = 1, byte pageSize = 10, string doctorID = null)
        {
            try
            {
                doctor doctor = null;
                if (doctorID == null) doctor = getCurrentDoctor();
                else
                {
                    Guid id = Guid.Parse(doctorID);
                    doctor=db.doctors.Single(d => d.id == id);
                }
                var patientViews = doctor.patientViews.Select(pv =>
                    new
                    {
                        comment=(pv.comment==null)?"":pv.comment,
                        patientName = pv.patient.username,
                        pv.rate
                    })
                    .OrderByDescending(a=>a.rate)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                    .ToList();
                var count=doctor.patientViews.Count;
                return Json(new { status = true, data = patientViews ,total=count}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new{doctorStatus=false}, JsonRequestBehavior.AllowGet);
            }
        }
        //get doctor advices as json string
        public JsonResult doctorAdvices(int pageNumber=1,byte pageSize=10,string doctorID = null)
        {
            try
            {
                doctor doctor = null;
                if (doctorID == null) doctor = getCurrentDoctor();
                else
                {
                    Guid id = Guid.Parse(doctorID);
                    db.doctors.Single(d => d.id == id);
                }
                var doctorAdvices = doctor.advices.Select(ad =>
                    new
                    {
                        date = ad.dateOfJoin,
                        content = ad.adviceContent,
                        ad.id,
                        comments = ad.commentOnAdvices.Count(c => c.comment != null),
                        likes = ad.commentOnAdvices.Count(c => c.like == true),
                        shares = ad.commentOnAdvices.Count(c => c.share == true),
                        seens = ad.commentOnAdvices.Count(c => c.like == true || c.comment != null || c.share == true)
                    })
                    .OrderByDescending(c => c.date)
                    .Skip((pageNumber-1)*pageSize).Take(pageSize)
                    .Select(c => new {c.id,c.content,c.comments,c.likes,c.seens,c.shares}).ToList();
                return Json(new { status = true, data = doctorAdvices,total=doctor.advices.Count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        //add new doctor advice
        public JsonResult addDoctorAdvice(string adviceContent)
        {          
            try
            {
                advice newAdvice = new advice();
                newAdvice.adviceContent = adviceContent;
                newAdvice.id = Guid.NewGuid();
                newAdvice.dateOfJoin = DateTime.Now;
                newAdvice.doctorID = getCurrentDoctor().id;
                db.advices.Add(newAdvice);
                db.SaveChanges();
                return Json(new { status = true, newID = newAdvice.id }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        //delete doctor advice
        public JsonResult deleteDoctorAdvice(string ID)
        {
            try
            {
                Guid adviceID = Guid.Parse(ID);
                db.advices.Remove(db.advices.Find(adviceID));
                db.SaveChanges();
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        //update doctor advice
        public JsonResult updateDoctorAdvice(string ID, string adviceContent)
        {
            try
            {
                Guid adviceID = Guid.Parse(ID);
                advice updatedAdvice = db.advices.Find(adviceID);
                updatedAdvice.adviceContent = adviceContent;
                db.Entry(updatedAdvice).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
        //get doctor resrevings as json string
        public JsonResult doctorReservings(char day='0')
        {
            //day[1,0,2](0=>today reservings,1=>nextday reservings,2=>prev day reservings)
            int reservingDay;//the day of reservings
            if (day == '0') reservingDay = DateTime.Now.Day;
            else if (day == '1') reservingDay = DateTime.Now.Day + 1;
            else reservingDay = DateTime.Now.Day-1;
                var reservings = getCurrentDoctor().reservings.Where(res => res.reservingDate.Year == DateTime.Now.Year &&
                             res.reservingDate.Month == DateTime.Now.Month &&
                             res.reservingDate.Day ==reservingDay).Select(res => new 
                             { ptName = res.patientName, res.phone, type = res.visitType }).ToList();
                return Json(reservings, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AllowAnonymous]
        //validate if selected speciality id is valid
        public JsonResult isValidSpecialityID(byte spID, string subSpID)
        {
            /*string[] subspecialities = { };
            if (subSpID != "null" && !subSpID.Contains(','))
            { subspecialities = new string[1]; subspecialities[0] = subSpID; }
            else if (subSpID != "null" && subSpID.Contains(',')) subspecialities = subSpID.Split(',');
            if (db.specialities.Any(s => s.id == spID && s.subSpecialites.Count > 0))
            {
                if (subspecialities.Length == 0)
                {
                    ModelState.AddModelError("subSpID", "please select at least one sub speciality");
                }
            }*/
            //check if selected specialityID is valid
            return Json(spID != 0 && db.specialities.Any(s => s.id == spID), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AllowAnonymous]
        //check if userbame or phone already exists at database
        public JsonResult IsUserNameOrPhoneExists(string value, string propertyType)
        {
            if (propertyType == "username")
            {
                if (isDoctorAuthenticated)
                    return Json(db.doctors.Any(d => d.username == value && d.username != User.Identity.Name), JsonRequestBehavior.AllowGet);
                else
                    return Json(db.doctors.Any(d => d.username == value), JsonRequestBehavior.AllowGet);
            }

            else
            {
                if (isDoctorAuthenticated)
                    return Json(db.doctors.Any(d => d.phone == value && d.username != User.Identity.Name), JsonRequestBehavior.AllowGet);
                else
                    return Json(db.doctors.Any(d => d.phone == value), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        //check if doctor username is unique
        public JsonResult checkDoctorUserNameIfUnique(string username)
        {
            if (isDoctorAuthenticated)
            {
                if (username == User.Identity.Name)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json(!db.doctors.Any(d => d.username == username && d.username != User.Identity.Name), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(!db.doctors.Any(d => d.username == username), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        //check if doctor username or phone is unique
        public JsonResult checkDoctorPhoneNumberIfUnique(string phone)
        {
            if (isDoctorAuthenticated)
            {
                return Json(!db.doctors.Any(d => d.phone == phone && d.username != User.Identity.Name), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(!db.doctors.Any(d => d.phone == phone), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        //check if selected subspecialities is valid for selected main speciality
        public JsonResult checkSubSpeciality(string subSpID, byte spID)
        {
            List<byte> subspecialities = new List<byte>();
            if (subSpID != "null" && !subSpID.Contains(',')) subspecialities.Add(byte.Parse(subSpID));
            else if (subSpID != "null" && subSpID.Contains(','))
            {
                List<string>s=subSpID.Split(',').ToList();
                foreach (var item in s)
	            {
		         subspecialities.Add(byte.Parse(item));
	            }
            }
            bool isValid = true;
            if (db.specialities.Any(s => s.id == spID))
            {//spID is valid and exists
                if (db.specialities.Any(s => s.id == spID && s.subSpecialites.Count > 0))
                {//spID has subSpecialities
                    if (subSpID.Length > 0)
                    {//there are selected sub specialities
                        /*for (int i = 0; i < subspecialities.Count; i++)
                        {
                            byte subSpecialityID = byte.Parse(subspecialities[i]);
                            if (!db.specialities.Any(sub => sub.id == subSpecialityID && sub.superSpecialityID == spID))
                            {
                                isValid = false;
                                break;
                            }
                        }*/
                        if (!db.specialities.Any(sub => subspecialities.Contains(sub.id) && sub.superSpecialityID == spID))
                        {
                            isValid = false;
                        }
                    }
                    else 
                    {//no selected sub specialities
                      //  isValid = false;
                    }
                }
                else
                {//spID has no subSpecialities
                    if (subSpID.Length != 0) isValid = false;
                }
            }
            else
            {//spID not valid and not exists
                isValid = false;
            }
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //update personal image
        public JsonResult updateDoctorImage(HttpPostedFileBase file, string extension)
        {
            string newImageName="";
            try
            {
                var doctorInfo = getCurrentDoctor().doctorInfo;
                if (validateProfileImageUpdate(doctorInfo.doctorID.ToString(), ref newImageName,file, doctorInfo.image,extension))
                {
                    if (newImageName == doctorInfo.image)
                        return Json(true, JsonRequestBehavior.AllowGet);
                    doctorInfo.image = newImageName;
                    UpdateFields<doctorInfo>(doctorInfo, db.Entry(doctorInfo), d => d.image);
                    db.SaveChanges();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        //validate if updated personal image is valid
        private bool validateProfileImageUpdate(string doctorID, ref string imageName, HttpPostedFileBase fileContent, string oldImageFileName,string extension)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            var supportedTypes = new string[] { "png", "jpg", "jpeg", "gif", "PNG", "JPG", "GIF", "JPEG" };
            var fileName = "";
            if (fileContent == null || fileContent.ContentLength == 0 || oldImageFileName == null) return true;
            //not valid extension
            if (!supportedTypes.Contains(extension)) return false;
            //image width and height must be 512 px
            using (var img = Image.FromStream(fileContent.InputStream))
            {
                if (img.Width != img.Height || fileContent.ContentLength > maxPersonalImgSize)
                {
                    return false;
                }
            }
            fileContent.InputStream.Position = 0;
            fileName = doctorID + '.' + extension;//image name that will be stored in db
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
    }
}