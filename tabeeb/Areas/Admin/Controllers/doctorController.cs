using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tabeeb.Models;
using tabeeb.Controllers;
using System.Threading;
namespace tabeeb.Areas.Admin.Controllers
{
    public class doctorController : mainController
    {
        private static bool requestOfPendingDoctors = false;
        private static bool requestOfNonCompltetedDoctors = false;
        private static int numOfDocsRequested = 0;
        public ActionResult doctor(string getDocs="")
        {
            int num = 0;
            try { num = int.Parse(getDocs); }
            catch (Exception) { num = 0; }
            if (num > 0) numOfDocsRequested = num;
            else if (getDocs == "pending")
            { requestOfPendingDoctors = true; numOfDocsRequested = 0; }
            else if (getDocs == "notComplete")
            { requestOfNonCompltetedDoctors = true; numOfDocsRequested = 0; }
            else
            {
                requestOfPendingDoctors = false;
                requestOfNonCompltetedDoctors = false;
                numOfDocsRequested = 0;
            }
            ViewBag.controller = "doctor";
            return View();
        }
        public JsonResult List(int pageNumber)
        {           
            var doctors = db.doctors.Where(d=>d.doctorInfo!=null).Select(d => new
            {
                d.id,
                d.mail,
                d.phone,
                d.bookingType,
                d.dateOfJoin,
                name = d.doctorInfo.fnameAr + " " + d.doctorInfo.lnameAr,
                d.proImage,
                d.doctorInfo.stat,
                d.doctorInfo.examinFees,
                d.doctorInfo.education,
                city = d.destrict.city.nameAr,
                destName = d.destrict.nameAr,
                allReservings = d.reservings.Count,
                dayReservings = d.reservings.Where(res => res.doctorID == d.id && (res.reservingDate).Year == DateTime.Now.Year && (res.reservingDate).Month == DateTime.Now.Month && (res.reservingDate).Day == DateTime.Now.Day).ToList().Count(),
                specialities = d.specialities.Select(s => new {name=s.nameAr}),
                specialitiesParentID =d.specialities.FirstOrDefault().superSpecialityID
            }).ToList();
            if(numOfDocsRequested>0)
            {//get doctors that have the most reservings and heighly requested
                var mostReservingDoc = doctors.Where(doc => doc.stat == true)
                    .OrderByDescending(or => or.allReservings).Where(doc=>doc.allReservings>0).ToList();
                int ResCount = mostReservingDoc.Count;
                numOfDocsRequested = (numOfDocsRequested > ResCount) ? ResCount : numOfDocsRequested;
                mostReservingDoc = mostReservingDoc.GetRange(0, numOfDocsRequested).ToList();
                return Json(new { data = mostReservingDoc, isLastPage = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var filteredData = (requestOfPendingDoctors == false)
                    ? doctors.ToList()
                    : doctors.Where(doc => doc.stat == false).ToList();
                int count = filteredData.Count;
                int start = 10 * (pageNumber - 1);
                start = (count > start) ? start : 0;
                bool isLastPage = false;
                int end=0;
                if (count >= 10 * pageNumber)
                {
                    end = 10;
                }
                else
                {
                    end = count - (10 * (pageNumber - 1));
                    isLastPage = true;
                }
                filteredData = filteredData.GetRange(start, end);
                return Json(new { data = filteredData, isLastPage = isLastPage }, JsonRequestBehavior.AllowGet);
            }
           
        }
        public JsonResult activation(Guid id)
        {/*this method activate doctor profile to be accepted for seeing by patients on site*/
            try
            {
                doctorInfo doctorInfo = db.doctorInfoes.Single(doc => doc.doctorID == id);
                //only set at first time function is called
                if (!doctorInfo.profileIsAccepted) doctorInfo.profileIsAccepted = true;
                doctorInfo.stat = !doctorInfo.stat;
                db.Entry(doctorInfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                string to = doctorInfo.doctor.mail;
                string subject = "your profile is accepted by tabeebek website adminstration";
                string messageBody = 
                        "<html>"+
                        "<body>"+
                        "<h2>hi " + doctorInfo.doctor.username+ "</h2>" +
                        "<p>"+
                        "Your profile is revisited and accepted by tabeebak website adminstration and"+
                        "<br>from now you can start setting your appointements of examins and bookings,then start your work on tabeebek</p>"+
                        "<a href=localhost/doctors/doctor/doctorPage'>Go to doctor page</a>" +
                        "</body>"+
                        "</html>";
                globalController.sendMailTo(to,subject,messageBody,"sync");
                return Json(new {result=true},JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getCitiesAndDestrictsAndSpecialities()
        {
            return Json(new
            {
                cities = db.cities.Select(c => new { c.id, name=c.nameAr }).ToList(),
                destricts = db.destricts.Select(d => new { d.cityID, name=d.nameAr, d.id }).ToList(),
                specialities = db.specialities.Select(sp => new { sp.id,sp.nameAr,sp.nameEng})
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetbyID(int ID)
        {
            var doctor = db.doctors.Find(ID);
            return Json(doctor, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Update(patient doc)
        {
            try
            {
                db.Entry(doc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Delete(int ID)
        {
            try
            {
                db.doctors.Remove(db.doctors.Find(ID));
                db.doctorInfoes.Remove(db.doctorInfoes.Find(ID));
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
	}
}