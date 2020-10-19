using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tabeeb.Models;
namespace tabeeb.Areas.Admin.Controllers
{
    public class patientController : mainController
    {
        private static int numOfPtRequested = 0;
       // GET: Home
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            db.Configuration.ProxyCreationEnabled = false;
        }
        //
        // GET: /Admin/doctor/
        public ActionResult patient(string getPt="")
        {
            int num = 0;
            try
            {
                num = int.Parse(getPt);
            }
            catch (Exception) { num = 0; }
            if (num > 0) numOfPtRequested = num;
            else numOfPtRequested = 0;
            ViewBag.controller = "patient";
            return View();
        }
        public JsonResult List(int pageNumber)
        {
            /*var patientsWithReservings =db.patients.Select(pt => new
            {
                allReservings = db.reservings.Count(res => res.patientID == pt.id),
                pt.id,
                pt.mail,
                pt.name,
                pt.password,
                pt.phone,
                pt.type,
                pt.dateOfJoin
            }).ToList();
            var data = (numOfPtRequested == 0) ? patientsWithReservings : patientsWithReservings.Where(pt=>pt.allReservings>0).ToList().GetRange(0, numOfPtRequested);*/
            var data = db.patients.ToList();
            int count = data.Count;
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
            data = data.GetRange(start, end);
            return Json(new { data = data, isLastPage = isLastPage }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Add(patient p)
        {
            try
            {
                db.patients.Add(p);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetbyID(int ID)
        {
            var patient = db.patients.Find(ID);
            return Json(patient, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Update(patient p)
        {
            try
            {
                db.Entry(p).State = System.Data.Entity.EntityState.Modified;
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
                db.patients.Remove(db.patients.Find(ID));
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