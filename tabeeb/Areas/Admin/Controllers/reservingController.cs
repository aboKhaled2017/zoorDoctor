using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tabeeb.Models;
namespace tabeeb.Areas.Admin.Controllers
{
    public class reservingController:mainController
    {
        private static bool dayReservingsResquest = false;
        // GET: /Admin/reserving/
        public ActionResult reserving(string getRes="")
        {
            if (getRes != "")dayReservingsResquest = true;
            else dayReservingsResquest = false;
            ViewBag.controller = "reserving";
            return View();
        }
        public JsonResult List(int pageNumber)
        {
            var doctorData = db.doctors.Join(db.doctorInfoes, doc => doc.id, df => df.doctorID, (doc, df) =>
               new { doc.id, name = df.fnameAr + " " + df.lnameAr, specialties = doc.specialities.Select(s => new {id=s.id,name=s.nameAr})});
            var data = db.reservings.Join(doctorData, res => res.doctorID, doc => doc.id, (res, doc) =>
                new {doc.name,ptName=res.patientName, res.reservingDate,doc.specialties}).ToList();
            var filteredData = (dayReservingsResquest == false) ? data : data
                .Where(res => res.reservingDate.Year == DateTime.Now.Year &&
                                                          res.reservingDate.Month == DateTime.Now.Month &&
                                                          res.reservingDate.Day == DateTime.Now.Day).ToList();
            int count = filteredData.Count;
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
            filteredData = filteredData.GetRange(start, end);
            return Json(new { data = filteredData, isLastPage = isLastPage }, JsonRequestBehavior.AllowGet);
        }
	}
}