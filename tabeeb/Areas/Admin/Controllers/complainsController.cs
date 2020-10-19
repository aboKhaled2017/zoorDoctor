using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tabeeb.Models;
namespace tabeeb.Areas.Admin.Controllers
{
    public class complainsController : mainController
    {
        internal static string currernController = "";
        internal static bool getRecentDoctorComplainsdata = false;
        internal static bool getRecentPatientComplainsdata = false;
        internal static bool getRecentGeneralComplainsdata = false;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            db.Configuration.ProxyCreationEnabled = false;
        }
        public ActionResult doctorComplains(string getRecent= null)
        {
            if (getRecent == null) getRecentDoctorComplainsdata = false;
            else
            {
                getRecentDoctorComplainsdata = true;
            }
            ViewBag.controller = "doctorComplains";
            currernController = "doctor";
            return View();
        }
        public ActionResult patientComplains(string getRecent=null)
        {
            if (getRecent == null) getRecentPatientComplainsdata = false;
            else
            {
                getRecentPatientComplainsdata = true;
            }
            ViewBag.controller = "patientComplains";
            currernController = "patient";
            return View();
        }
        public ActionResult generalComplains(string getRecent = null)
        {
            if (getRecent == null) getRecentGeneralComplainsdata = false;
            else
            {
                getRecentGeneralComplainsdata = true;
            }
            ViewBag.controller = "generalComplains";
            currernController = "general";
            return View();
        }
        public JsonResult List(int pageNumber)
        {
            admin currentAdmin;
            if(currernController == "doctor")
            {
                var doctorComplains = db.complains.Where(doc=>doc.phone!=null);
                var recentDoctorComplains = doctorComplains.ToList();
                if(getRecentDoctorComplainsdata==true) 
                {
                    currentAdmin = db.admins.FirstOrDefault(ad => ad.name == User.Identity.Name);
                    recentDoctorComplains = (doctorComplains.Where(cm => cm.dateOfJoin >currentAdmin.lastDoctorComplainsViewedDate)).ToList();
                    currentAdmin.lastDoctorComplainsViewedDate = DateTime.Now;
                    db.Entry(currentAdmin).State = System.Data.Entity.EntityState.Modified; 
                    db.SaveChanges();
                }
                int count = recentDoctorComplains.Count;
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
                if (start > end || end < 0 || start < 0)
                { start = 0; end = 0; }
                recentDoctorComplains = recentDoctorComplains.GetRange(start, end);
                return Json(new { data = recentDoctorComplains, isLastPage = isLastPage }, JsonRequestBehavior.AllowGet);
            }
            else if (currernController== "patient")
            {
                var patientComplains = db.complains.Where(doc => doc.phone == null && doc.general==true);
                var recentPatientComplains = patientComplains.ToList();
                if(getRecentPatientComplainsdata==true)
                {
                    currentAdmin = db.admins.FirstOrDefault(ad => ad.name == User.Identity.Name);
                    recentPatientComplains = patientComplains.Where(doc => doc.dateOfJoin >= (currentAdmin.lastPatientComplainsVieweddate)).ToList();
                    currentAdmin.lastPatientComplainsVieweddate = DateTime.Now;
                    db.Entry(currentAdmin).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                int count = recentPatientComplains.Count;
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
                if (start > end || end < 0 || start < 0)
                { start = 0; end = 0; }
                recentPatientComplains = recentPatientComplains.GetRange(start, end);
                return Json(new { data = recentPatientComplains, isLastPage = isLastPage }, JsonRequestBehavior.AllowGet);
            }
            else if (currernController == "general")
            {
                var patientComplains = db.complains.Where(doc => doc.phone == null && doc.general == false);
                var recentPatientComplains = patientComplains.ToList();
                if (getRecentGeneralComplainsdata == true)
                {
                    currentAdmin = db.admins.FirstOrDefault(ad => ad.name == User.Identity.Name);
                    recentPatientComplains = patientComplains.Where(doc => doc.dateOfJoin >= (currentAdmin.lastGeneralComplainViewedDate)).ToList();
                    currentAdmin.lastGeneralComplainViewedDate = DateTime.Now;
                    db.Entry(currentAdmin).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                int count = recentPatientComplains.Count;
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
                if (start > end || end < 0 || start < 0)
                { start = 0; end = 0; }
                recentPatientComplains = recentPatientComplains.GetRange(start, end);
                return Json(new { data = recentPatientComplains, isLastPage = isLastPage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Delete(int ID)
        {
            try
            {
                db.complains.Remove(db.complains.Find(ID));
                db.SaveChanges();
                db.rearrangeID("complain","id");
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
    }
}