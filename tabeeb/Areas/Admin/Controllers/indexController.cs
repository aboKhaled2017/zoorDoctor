using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using tabeeb.Models;
namespace tabeeb.Areas.Admin.Controllers
{
    public class indexController : mainController
    {
        //
        // GET: /Admin/index/
        public ActionResult Index()
        {
            try
            {
                ViewBag.controller = "index";
                int numOfDoctors = db.doctorInfoes.Count();
                int numOfPatients = db.patients.Count();
                int numOfPendingDoctors = db.doctorInfoes.Count(doc => doc.stat == false);
                int numOfDayReservings = db.reservings.Count(res => res.reservingDate.Year == DateTime.Now.Year &&
                                                                   res.reservingDate.Month == DateTime.Now.Month &&
                                                                   res.reservingDate.Day == DateTime.Now.Day);
                int maxReservingDoc = db.doctorInfoes.
                Where(doc => db.reservings.Count(res => res.doctorID == doc.doctorID) > 0 && doc.stat == true).Count();
                /*int maxReservingPt = db.patients.
                Where(pt => db.reservings.Count(res => res.patientID == pt.id) > 0).Count();*/
                admin currentAdmin=db.admins.FirstOrDefault(ad=>ad.name==User.Identity.Name);
                int recentDoctorComplains = db.complains.Where
                    (c => c.phone != null && c.dateOfJoin > currentAdmin.lastDoctorComplainsViewedDate).Count();
                int recentPatientComplains = db.complains.Where
                    (c => c.phone == null &&c.general==true && c.dateOfJoin > currentAdmin.lastPatientComplainsVieweddate).Count();
                int recentGeneralComplains = db.complains.Where
                    (c => c.phone == null && c.general==false && c.dateOfJoin > currentAdmin.lastGeneralComplainViewedDate).Count();
                int[] statsInfo = new int[] { 
                    numOfDoctors, 
                    numOfPendingDoctors,
                    numOfPatients,
                    numOfDayReservings,
                    maxReservingDoc,
                    recentDoctorComplains,
                    recentPatientComplains,
                    recentGeneralComplains
                };
                return View(statsInfo);
            }
            catch(Exception)
            {
                return Redirect("/admin/account/logIn");
            }
        }
       
    }
}