using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tabeeb.Models;
using tabeeb.Areas.users.Models;
using tabeeb.Controllers;
using tabeeb.Areas.doctors.Models;
namespace tabeeb.Areas.users.Controllers
{
    //this class handle all search operations
    public class searchController : mainController
    {
        /**this function resturn number of doctors cards that match search page items
         this type of search is advanced search based on more categorization*/
        public ActionResult search(searchItems searchObject)
        {
            try
            {
                //this page need some data before displaying it
                //this data include cities,destricts,and so on...
                var data = new Tuple<List<specialityData>, List<cityData>>
                (db.specialities.getSpecialityDataList().ToList(),
                 db.cities.getCityDataList().ToList());
                //page data to viewData
                ViewBag.neededData = data;
                //if no parameters in request ,then return empty html view without any data
                if (Request.QueryString.Count ==0) return View();
                //get list of doctors that matched search items
                var matchedDoctors = (string.IsNullOrEmpty(searchObject.q))
                ? db.specialities.searchForDoctors(searchObject).ToList()
                : generalSearch(searchObject.q).ToList();
                //total number of matched search data
                var totalMatchedSearch = matchedDoctors.Count;
                //get cards data that will be displayed on doctors cards
                List<doctorCard> cards = matchedDoctors
                    .getDoctorCards(currentLanguage)
                    .OrderByDescending(c => c.rate)//order by rate
                    .Skip((searchObject.pageNumber - 1) * maxNumberOfCardsForEachpage)
                    .Take(maxNumberOfCardsForEachpage).ToList();
                //configure the pagination of search
                paginationInfo paging = new paginationInfo();
                paging.currentPage = searchObject.pageNumber;
                paging.itemsPerPage = maxNumberOfCardsForEachpage;
                paging.totalItems = totalMatchedSearch;
                return View(new Tuple<paginationInfo, List<doctorCard>>(paging, cards));
            }
            catch (Exception)
            {
                return View();
            }
        }
        /*this function resturn number of doctors cards that match search of main page
         search is based on categorization of the search bar*/
        public ViewResult searchFor(searchItems searchObject)
        {
            //if there are no parameters at request ,then return empty html view without any data
            if (Request.QueryString.Count == 0) return View();
            /*
             check if search is based on categorization(search bar) or based on search keywork(search input)
             * if searchObject.q is null then this is first type of search(search by categorization of search bar)
             * if searchobject.q contains data then this is second type of search(search by keyword)
             */
            var matchedDoctors = (string.IsNullOrEmpty(searchObject.q))
            ? db.specialities.searchForDoctors(searchObject).ToList()//search by categorization
            : generalSearch(searchObject.q).ToList();//search by keyword
            //total number of doctors list that matched search
            var totalMatchedSearch = matchedDoctors.Count;
            //get cards data that will be displayed on doctors cards
            List<doctorCard> cards = matchedDoctors
                .getDoctorCards(currentLanguage)
                .OrderByDescending(c => c.rate)//order by rate
                .Skip((searchObject.pageNumber - 1) * maxNumberOfCardsForEachpage)
                .Take(maxNumberOfCardsForEachpage).ToList();
            //configuration of paginations of search
            paginationInfo paging = new paginationInfo();
            paging.currentPage = searchObject.pageNumber;
            paging.itemsPerPage = maxNumberOfCardsForEachpage;
            paging.totalItems = totalMatchedSearch;
            //pass data to html view to be displayed on page
            return View(new Tuple<paginationInfo, List<doctorCard>>(paging, cards));
        }
        /*this function search for doctors based on keywork search that
         entered by user at search engin*/
        private  IEnumerable<doctor> generalSearch(string value)
        {
            //intialize empty list of doctors
            IEnumerable<doctor>emptyDoctorsData=new List<doctor>();
            if (value == null) value = "";
            value = value.Trim();
            //neglect all letters that is not important
            if (value.Contains("'") ||
                value.Contains("\"") ||
                value.Contains("=") ||
                value.Contains("\\") ||
                value.Contains("-") ||
                value.Contains("(") ||
                value.Contains(")") ||
                value.Contains("[") ||
                value.Contains("]")
                ) { value = "";}
            if (string.IsNullOrEmpty(value)) return emptyDoctorsData;
            try
            {//get doctors that match search key word
                var matchedDoctors = db.getDocorsOfGeneralSearch(value).ToList();
                return matchedDoctors;
            }
            catch (Exception)
            {
                return emptyDoctorsData;
            }
        }
        /*
         * this function return html string that represent search bar at the main users site
         */
        public PartialViewResult searchBar()
        {
            //search bar need fetched data from database about speciality,city,destrict,and doctors data
            var data = new Tuple<List<specialityData>, List<cityData>, List<doctorDataList>>
            (db.specialities.getSpecialityDataList().ToList(),
             db.cities.getCityDataList().ToList(),
             db.doctorInfoes.getDoctorsDataList().ToList()
            );
            return PartialView(data);
        }

    }
}