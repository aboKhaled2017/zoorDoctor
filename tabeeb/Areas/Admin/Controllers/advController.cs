using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tabeeb.Areas.Admin.Models;
using tabeeb.Models;
namespace tabeeb.Areas.Admin.Controllers
{
    public class advController : mainController
    {
        //
        // GET: /Admin/adv/
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            db.Configuration.ProxyCreationEnabled = false;
        }
        //
        // GET: /Admin/doctor/
        public ActionResult adv()
        {
            ViewBag.controller = "adv";
            ViewBag.selectPages = db.pages;
            return View();
        }
        public JsonResult List(int pageNumber)
        {
            var advs=db.advs.ToList();
                int count = advs.Count;
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
                advs = advs.GetRange(start, end);
                return Json(new { data = advs, isLastPage = isLastPage }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult activation(int id)
        {
            try
            {
                adv activatedAdv = db.advs.Single(adv=>adv.id==id);
                if (activatedAdv.stat) activatedAdv.stat = false;
                else activatedAdv.stat = true;
                db.Entry(activatedAdv).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Add(advPages data)
        {
            try
            {
            adv newAdv = new adv();
            int count=db.advs.Count();
            newAdv.id = (byte)(count + 1);
            newAdv.description = data.description;
            newAdv.content = data.content;
            newAdv.isViewAgain = data.isViewAgain;
            newAdv.amount = data.amount;
            newAdv.startDate = data.startDate;
            newAdv.stat = data.stat;
            newAdv.waitingTime = data.waitingTime;
            newAdv.waitingAfterClosed = data.waitingAfterClosed;
            newAdv.image = data.image;
            advRelatedPage newAdvPage = null;
            List<byte> pageIDs = data.pagesID;             
            db.advs.Add(newAdv);
            count = db.advRelatedPages.Count();
                foreach (var id in pageIDs)
                {                   
                    newAdvPage = new advRelatedPage();
                    newAdvPage.id = ++count;
                    newAdvPage.advID = newAdv.id;
                    newAdvPage.pageID = id;
                    db.advRelatedPages.Add(newAdvPage);
                }
            db.SaveChanges();
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
        }
        public JsonResult GetbyID(int ID)
        {
            var advertise = db.advs.Find(ID);
            var pages = db.advRelatedPages.Where(p=>p.advID==advertise.id).Select(p => p.pageID).ToList();
            return Json(new { adv = advertise, pagesID = pages }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Update(advPages data)
        {
            adv newAdv = new adv();
            newAdv.id =data.id;//identity value will be changed automatic
            newAdv.description = data.description;
            newAdv.content = data.content;
            newAdv.isViewAgain = data.isViewAgain;
            newAdv.amount = data.amount;
            newAdv.startDate = data.startDate;
            newAdv.stat = data.stat;
            newAdv.waitingTime = data.waitingTime;
            newAdv.waitingAfterClosed = data.waitingAfterClosed;
            newAdv.image = data.image;
            advRelatedPage newAdvPage = null;
            List<byte> pageIDs = data.pagesID;
            List<advRelatedPage> pgs = db.advRelatedPages.Where(a => a.advID == newAdv.id).ToList();
            try
            {
                db.Entry(newAdv).State = System.Data.Entity.EntityState.Modified;
                foreach (var item in pgs)
                {
                    db.advRelatedPages.Remove(item);
                }
                db.SaveChanges();
                db.rearrangeID("advRelatedPage","id");
                db.SaveChanges();
                int count = db.advRelatedPages.Count();
                foreach (var id in pageIDs)
                {
                    newAdvPage = new advRelatedPage();
                    newAdvPage.id = ++count;
                    newAdvPage.advID = newAdv.id;
                    newAdvPage.pageID = id;
                    db.advRelatedPages.Add(newAdvPage);
                }
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Delete(int ID)
        {
            try
            {
                db.advs.Remove(db.advs.Find(ID));              
                db.SaveChanges();
                db.rearrangeID("advs", "id");
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