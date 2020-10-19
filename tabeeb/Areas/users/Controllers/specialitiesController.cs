using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tabeeb.Models;
using tabeeb.Areas.users.Models;
using tabeeb.Controllers;
namespace tabeeb.Areas.users.Controllers
{
    //this class handle all operations on specialities pages
    public class specialitiesController : mainController
    {
        //specialities page
        public ActionResult specialities()
        {
            //return all data of specialities that page need to be displayed
            List<specialityDescription> spdata = (currentLanguage == "ar")
                ? db.specialities.Where(sp => sp.subSpecialites.Count == 0)//if language is arabic
                .Select(s => new specialityDescription { id = s.id, image = s.img, name = s.nameAr, description = null }).ToList()
                : db.specialities.Where(sp => sp.subSpecialites.Count == 0)//if language is english
                .Select(s => new specialityDescription { id = s.id, image = s.img, name = s.nameEng, description = null }).ToList();                
            return View(spdata);        
        }
        //this function get speciality discription when user click on particular speciality
        public JsonResult getSpDescription(string name)
        {
            try
            {//return discription based on language of site
                string specialityDescription = (currentLanguage == "en")
                    ? db.specialities.FirstOrDefault(s => s.nameEng == name ||s.nameAr==name).descriptionEng
                    : db.specialities.FirstOrDefault(s => s.nameEng == name ||s.nameAr==name).descriptionAr;
                specialityDescription = (string.IsNullOrEmpty(specialityDescription)) ? "" : specialityDescription;
                var a = Request;
                return Json(specialityDescription, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false,JsonRequestBehavior.AllowGet);
            }
        }        
        //return specialities of particular doctors
        public ActionResult getSpecialityDoctors(string name,int pageNumber=1)
        {
            try
            {                
                var speciality = db.specialities.FirstOrDefault(s => s.nameEng == name || s.nameAr == name);
                specialityDescription spData = new specialityDescription();
                if(currentLanguage=="en")
                {
                    spData.id=speciality.id;
                    spData.image=speciality.img;
                    spData.name=speciality.nameEng;
                    spData.description=speciality.descriptionEng;
                }
                else
                {
                    spData.id=speciality.id;
                    spData.image=speciality.img;
                    spData.name=speciality.nameAr;
                    spData.description=speciality.descriptionAr;
                }                   
                //get list of doctor card with defined languages for this speciality
                var matchedDoctors= speciality.allDoctors().activatedDoctors();
                spData.totalSpecializedDoctors = matchedDoctors.Count();
                List<doctorCard> doctorCards = matchedDoctors
                    .getDoctorCards(currentLanguage)
                    .OrderByDescending(c => c.rate)//order by rate
                    .Skip((pageNumber - 1) * maxNumberOfCardsForEachpage)
                    .Take(maxNumberOfCardsForEachpage).ToList();
                paginationInfo paging = new paginationInfo();
                paging.currentPage = pageNumber;
                paging.itemsPerPage =maxNumberOfCardsForEachpage;
                paging.totalItems = matchedDoctors.Count();
                return View(new Tuple<specialityDescription, List<doctorCard>, paginationInfo>(spData, doctorCards, paging));
            }
            catch(Exception)
            {
                return Redirect("/"+defaultPathSpecialityPart);
            }
        }
    }
}