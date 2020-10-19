using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using tabeeb.Areas.doctors.Models;
using tabeeb.Areas.users.Models;
using tabeeb.Models;
namespace tabeeb.Controllers
{
    public static class extentions
    {
        /*this function convert array of appointements to string of concatanated items*/
        public static string getAppointementString(this List<appointementData> appointements)
        { 
            string appointementsString = "{";
            for (int i = 0; i < appointements.Count; i++)
            {
                appointementsString += "\"" + appointements[i].dateOfBooking.Value.DayOfYear + "\":" + appointements[i].interval + ",";
            }
            if (appointements.Count > 0)
            {//this line will remove the last comma[,] from string
                appointementsString = appointementsString.Remove(appointementsString.Length - 1, 1);
            }
            appointementsString += "}";
            return appointementsString;
        }
        public static string getDefaultPathFor(this HtmlHelper helper,string area,string controller=null)
        {
            if (area == "doctors")
                return globalController.defaultPathForDoctorsArea;
            else if (area == "users"&&controller=="patient")
                return globalController.defaultPathForUserSite;
            else if (area == "users" && controller == "specialities")
                return globalController.defaultPathSpecialityPart;
            else if (area == "users" && controller == "search")
                return globalController.defaultPathSearchPart;
            else if (area == "users" && controller == "advices")
                return globalController.defaultPathAdvicesPart;
            else if (area == "users" && controller == "language")//not controller
                return globalController.defaultPathLanguagePart;
            else if (area == "users" && controller == "doctor")//not controller
                return globalController.defaultPathDoctorPage;
            else if (area == "users" && controller == "reservingDetails")//not controller
                return globalController.defaultPathDoctorReservingPage;
            else if (area == "users" && controller == "reservation")
                return globalController.defaultPathReservation;
            else return "";
        }
        public static string getDefaultPathFor(string area, string controller = null)
        {
            if (area == "doctors")
                return globalController.defaultPathForDoctorsArea;
            else if (area == "users" && controller == "patient")
                return globalController.defaultPathForUserSite;
            else if (area == "users" && controller == "specialities")
                return globalController.defaultPathSpecialityPart;
            else if (area == "users" && controller == "search")
                return globalController.defaultPathSearchPart;
            else if (area == "users" && controller == "advices")
                return globalController.defaultPathAdvicesPart;
            else if (area == "users" && controller == "language")//not controller
                return globalController.defaultPathLanguagePart;
            else if (area == "users" && controller == "doctor")//not controller
                return globalController.defaultPathDoctorPage;
            else if (area == "users" && controller == "reservingDetails")//not controller
                return globalController.defaultPathDoctorReservingPage;
            else if (area == "users" && controller == "reservation")
                return globalController.defaultPathReservation;
            else return "";
        }
        public static byte getMaxNumberOfCardsForPage(this HtmlHelper helper)
        {
            return globalController.maxNumberOfCardsForEachpage;
        }
        public static string getCurrentlanguage(this HtmlHelper helper)
        {
           return (System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "en-US") ? "en" : "ar"; 
        }
        public static string getUsersSiteLayout(this HtmlHelper helper)
        {
            return "~/Areas/users/Views/Shared/layout.cshtml";
        }
        public static string getMonthName(this DateTime date,string lang="en")
        {
            return (lang == "en") ?globalController.monthsEnglishName[date.Month - 1] :globalController.monthsArabicName[date.Month-1];
        }
        public static string getDayName(byte dayIndex, string lang = "en")
        {
            return (lang == "en") ? globalController.monthsEnglishName[dayIndex-1] : globalController.monthsArabicName[dayIndex-1];
        }
        public static string converToTime(this string time, string lang = "en")
        {
            if (lang == "en") return time;
            else
            {
                return time.Replace("from", "من").Replace("to", "حتى").Replace("AM", "صباحا").Replace("PM", "مساءا");
            }
        }
        public static MvcHtmlString paginationLinks(this HtmlHelper html,paginationInfo pageInfo,Func<int,string>pageUrl)
        {
            if (pageInfo.totalPages < 2) return MvcHtmlString.Empty;
            StringBuilder result = new StringBuilder();
            if (pageInfo.totalPages > 10)
            {//first edge
                TagBuilder firsEdgeLink = new TagBuilder("a");
                firsEdgeLink.AddCssClass("first edge btn btn-default");
                result.Append(firsEdgeLink.ToString());
            }
            for (int i = 1; i <=pageInfo.totalPages; i++)
            {//pages links
                TagBuilder a = new TagBuilder("a");
                a.MergeAttribute("href",pageUrl(i));
                a.InnerHtml = i.ToString();
                if (i == pageInfo.currentPage) a.AddCssClass("btn btn-primary selected");
                else a.AddCssClass("btn btn-default");
                result.Append(a.ToString());
            }           
            if(pageInfo.totalPages>10)
            {//last edge
                TagBuilder lastEdgeLink = new TagBuilder("a");
                lastEdgeLink.AddCssClass("last edge btn btn-default");
                result.Append(lastEdgeLink.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }
        public static IHtmlString pageActionLink(
        this HtmlHelper html,
        string areaName,
        string controllerName,
        List<Tuple<string, string>> parameters,
        Action<List<Tuple<string, string>>>delegat)
        {
            delegat(parameters);
            string link ="/"+getDefaultPathFor(areaName, controllerName)+"?";
            link += parameters[0].Item1+"="+parameters[0].Item2;
            for (int i = 1; i <parameters.Count; i++)
            {
                if (string.IsNullOrEmpty(parameters[i].Item2) || string.IsNullOrWhiteSpace(parameters[i].Item2)) continue;
                link += "&"+parameters[i].Item1+"="+parameters[i].Item2;
            }
            return new HtmlString(link);
        }
        public static string getQueryStringValue(this HttpRequestBase request,string key)
        {
            return request.QueryString.Get(key);
        }
        public static List<Tuple<string, string>> getSearchParametersList(this HtmlHelper html, HttpRequestBase Request)
        {
            var allParams = new List<Tuple<string, string>>();
            allParams.Add(new Tuple<string, string>("cityID", Request.getQueryStringValue("cityID")));
            allParams.Add(new Tuple<string, string>("destrictID", Request.getQueryStringValue("destrictID")));
            string 
                educationValues = Request.getQueryStringValue("education"),
                typeValues = Request.getQueryStringValue("type"),
                priceValues = Request.getQueryStringValue("price"),
                specialityIdValues = Request.getQueryStringValue("specialityIDs");
            string[] educations =(string.IsNullOrEmpty(educationValues))?new string[]{}:educationValues.Split(',');
            foreach (var item in educations)
            {
                allParams.Add(new Tuple<string, string>("education", item));
            }
            string[] types = (string.IsNullOrEmpty(typeValues)) ? new string[] { } : typeValues.Split(',');
            foreach (var item in types)
            {
                allParams.Add(new Tuple<string, string>("type", item));
            }
            string[] specialityIDs = (string.IsNullOrEmpty(specialityIdValues)) ? new string[] { } : specialityIdValues.Split(',');
            foreach (var item in specialityIDs)
            {
                allParams.Add(new Tuple<string, string>("specialityIDs", item));
            }
            string[] price = (string.IsNullOrEmpty(priceValues)) ? new string[] { } : priceValues.Split(',');
            foreach (var item in price)
            {
                allParams.Add(new Tuple<string, string>("price", item));
            }
            return allParams;
        }
        public static Dictionary<byte, string> getDoctorProfessionsNames(this HtmlHelper helper)
        {
            if (helper.getCurrentlanguage() == "en")
                return globalController.engEducationsNames;
            else return globalController.arEducationsNames;
        }
        public static userCookieData getUserCookieData(this HttpRequestBase request)
        {
            string cookieData = FormsAuthentication.Decrypt(request.Cookies[FormsAuthentication.FormsCookieName].Value).UserData;
            userCookieData userData = new JavaScriptSerializer().Deserialize<userCookieData>(cookieData);
            return userData;
        }
        public static string getBaseUrl(this HttpRequestBase request)
        {
            return request.Url.GetLeftPart(UriPartial.Authority);
        }
        public static doctorPageInfo getDoctorPageInfo(this doctor doctor,bool isGlobalPage=false)
        {
                bool isDocInfoFounded = doctor.doctorInfo != null;
                doctorPageInfo doctorPageData =new  doctorPageInfo();
                doctorPageData.id = doctor.id;
                doctorPageData.image = doctor.doctorInfo.image;
                var patientViewsCount = doctor.patientViews.Count;
                double rate = (patientViewsCount > 0) ?doctor.patientViews.Average(p => p.rate) :0;
                doctorPageData.rate = rate;                
                doctorPageData.examinFee = doctor.doctorInfo.examinFees;                
                if(!isGlobalPage)
                {
                    doctorPageData.phone = doctor.phone;
                    doctorPageData.urls = doctor.doctorInfo.urls;
                }
                if(globalController.currentLanguage=="en")
                {//english profile
                doctorPageData.services = doctor.doctorInfo.servicesEng;
                doctorPageData.name = doctor.doctorInfo.fnameEng+" "+doctor.doctorInfo.lnameEng;
                doctorPageData.aboutDoc = doctor.doctorInfo.aboutDocEng;
                doctorPageData.availableTime = doctor.doctorInfo.availableTimeEng;
                doctorPageData.cityname = doctor.destrict.city.nameEng;
                doctorPageData.destrictName = doctor.destrict.nameEng;
                doctorPageData.professionName = globalController.englishProfessions.SingleOrDefault(p => p.Key == doctor.doctorInfo.education).Value;
                doctorPageData.spName = string.Join(" and ", doctor.specialities.Select(s => s.nameEng));
                }
                else
                {//arabic profile
                doctorPageData.services = doctor.doctorInfo.servicesAr;
                doctorPageData.name = doctor.doctorInfo.fnameAr+" "+doctor.doctorInfo.lnameAr;
                doctorPageData.aboutDoc = doctor.doctorInfo.aboutDocAr;
                doctorPageData.availableTime = doctor.doctorInfo.availableTimeAr;
                doctorPageData.cityname = doctor.destrict.city.nameAr;
                doctorPageData.destrictName = doctor.destrict.nameAr;
                doctorPageData.professionName = globalController.arabicProfessions.SingleOrDefault(p => p.Key == doctor.doctorInfo.education).Value;
                doctorPageData.spName = string.Join(" و ", doctor.specialities.Select(s => s.nameAr));
                }
                return doctorPageData;
        }
        public static doctorCard getDoctorCard(this doctorInfo docInfo, string language)
        {
        if (docInfo == null) return null;
            try
            {
                doctorCard ProfileCard = new doctorCard();
                var patientViewsCount = docInfo.doctor.patientViews.Count;
                ProfileCard.spName = docInfo.doctor.getSpecialityName(language);
                double rate = (patientViewsCount > 0) ?docInfo.doctor.patientViews.Average(p => p.rate):0;
                string image = docInfo.image;
                Guid doctorID = docInfo.doctorID;
                var examinFees = docInfo.examinFees;
                var viewers = docInfo.doctor.patientViews.Count();
                ProfileCard.waitingTime = docInfo.waitingTime;
                if (language == "ar")
                {
                    //this is for arabic profile
                    ProfileCard.rate = rate;
                    ProfileCard.viewers = viewers;
                    ProfileCard.timing = docInfo.availableTimeAr;                   
                    ProfileCard.price = examinFees;
                    ProfileCard.fname = docInfo.fnameAr;
                    ProfileCard.lname = docInfo.lnameAr;
                    ProfileCard.image = image;
                    ProfileCard.id = doctorID;
                    ProfileCard.professionTitle = docInfo.profTitleAr;
                    ProfileCard.clinicAddress = docInfo.clinicAddressAr;
                }
                else
                {
                    //this is for english profile
                    ProfileCard.rate = rate;
                    ProfileCard.viewers = viewers;
                    ProfileCard.timing = docInfo.availableTimeEng;
                    ProfileCard.price = examinFees;
                    ProfileCard.fname = docInfo.fnameEng;
                    ProfileCard.lname = docInfo.lnameEng;
                    ProfileCard.image = image;
                    ProfileCard.id = doctorID;
                    ProfileCard.professionTitle = docInfo.profTitleEng;
                    ProfileCard.clinicAddress = docInfo.clinicAddressEng;                 
                }
                return ProfileCard;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static IEnumerable<doctorCard>getDoctorCards(this IEnumerable<doctor> doctors,string language)
        {
            List<doctorCard> cards = new List<doctorCard>();
            foreach (var doc in doctors)
            {
                cards.Add(doc.doctorInfo.getDoctorCard(language));
            }
            return cards;
        }
        public static string getSpecialityName(this doctor doctor,string lang)
        {
            var specialities = (lang == "en")
            ? doctor.specialities.Select(s => new { name = s.nameEng }).ToList()
            : doctor.specialities.Select(s => new { name = s.nameAr }).ToList();
            string spNameFirstPart = "";
            spNameFirstPart = (lang == "en") ? "doctor of " : "دكتور ";
            if(specialities.Count>1)
            {
                string parentSpecialityName = (lang == "en")
                ? doctor.specialities.First().superSpeciality.nameEng + " specialized in "
                : doctor.specialities.First().superSpeciality.nameAr + " متخصص فى ";
                spNameFirstPart += parentSpecialityName;
            }
            string spNameSecondPart =(lang=="en")
            ?string.Join(" and ", specialities.Select(s => s.name))
            :string.Join(" و ", specialities.Select(s => s.name));
            return spNameFirstPart + spNameSecondPart;
        }
        public static IEnumerable<specialityData> getSpecialityDataList(this IEnumerable<speciality> specialities)
        {
            return (globalController.currentLanguage == "en")
            ? specialities
                .Where(s => s.superSpecialityID == null)
                .Select(s => new specialityData
                {
                    id = s.id,
                    name = s.nameEng,
                    subSpecialities = s.subSpecialites.Select(sub => new subSpecialityData
                    {
                        id = sub.id,
                        name = sub.nameEng,
                    })
                    .ToList()
                })
                .ToList()
            :specialities
                .Where(s => s.superSpecialityID == null)
                .Select(s => new specialityData
                {
                    id = s.id,
                    name = s.nameAr,
                    subSpecialities = s.subSpecialites.Select(sub => new subSpecialityData
                    {
                        id = sub.id,
                        name = sub.nameAr,
                    })
                    .ToList()
                })
                .ToList();
        }
        public static IEnumerable<specialityWithDoctorsData> getSpecialityWithDoctorsDataList(this IEnumerable<speciality> specialities)
        {
            return (globalController.currentLanguage == "en")
            ? specialities
                .Where(s => s.superSpecialityID == null)
                .Select(s => new specialityWithDoctorsData
                {
                    id = s.id,
                    name = s.nameEng,
                    subSpecialities = s.subSpecialites.Select(sub => new subSpecialityData
                    {
                        id = sub.id,
                        name = sub.nameEng,
                    })
                    .ToList(),
                    doctors = new List<Tuple<Guid, string>>(s.allDoctors().activatedDoctors()
                        .Select(d =>new Tuple<Guid, string>(d.id,d.doctorInfo.fnameEng + " " + d.doctorInfo.lnameEng)).ToList())
                })
                .ToList()
            : specialities
                .Where(s => s.superSpecialityID == null)
                .Select(s => new specialityWithDoctorsData
                {
                    id = s.id,
                    name = s.nameAr,
                    subSpecialities = s.subSpecialites.Select(sub => new subSpecialityData
                    {
                        id = sub.id,
                        name = sub.nameAr,
                    })
                    .ToList(),
                    doctors = new List<Tuple<Guid, string>>(s.allDoctors().activatedDoctors()
                        .Select(d =>new Tuple<Guid, string>(d.id,d.doctorInfo.fnameAr + " " + d.doctorInfo.lnameAr)).ToList())
                })
                .ToList();
        }
        public static IEnumerable<cityData>getCityDataList(this IEnumerable<city> cities)
        {
            return (globalController.currentLanguage == "en")
             ? cities
                 .Where(c => c.destricts.Count > 0)
                 .Select(c => new cityData
                 {
                     id = c.id,
                     name = c.nameEng,
                     destricts = c.destricts.Select(des => new destrictData
                     {
                         id = des.id,
                         name = des.nameEng,
                     })
                     .ToList()
                 })
                 .ToList()
             : cities
                 .Where(c => c.destricts.Count > 0)
                 .Select(c => new cityData
                 {
                     id = c.id,
                     name = c.nameAr,
                     destricts = c.destricts.Select(des => new destrictData
                     {
                         id = des.id,
                         name = des.nameAr,
                     })
                     .ToList()
                 })
                 .ToList();
        }
        public static bool existsIn<T>(this T obj,IEnumerable<T>objects)
        {
            return objects.Contains(obj);
        }    
        public static IEnumerable<doctor>searchForDoctors(this IEnumerable<speciality>specialities,searchItems searchObject)
        {
            if (!string.IsNullOrEmpty(searchObject.docName))
            {
                return globalController.db.doctors.activatedDoctors()
                    .getDoctorsOfName(searchObject.docName);
                        
            }
            IEnumerable<doctor> data
                 = specialities
                .findSpecialities(searchObject.specialityIDs)
                .groupingDoctros().activatedDoctors()
                .getDoctorsOfDestrict(searchObject.destrictID,searchObject.cityID)
                .getDoctorsOfGender(searchObject.type)
                .getDoctorsOfEducation(searchObject.education)
                .getDoctorsOfExaminFees(searchObject.price);
            return data;          
        }
        public static IEnumerable<doctorDataList>getDoctorsDataList(this IEnumerable<doctorInfo>doctorsInf)
        {
            return doctorsInf.Where(d=>d.stat==true)
                .Select(d => new doctorDataList { nameAr =d.fnameAr+" "+d.lnameAr,nameEng=d.fnameEng+" "+d.lnameEng });
        }
        public static IEnumerable<doctorDataList> getDoctorsDataList(this IEnumerable<doctor> doctors)
        {
            return doctors.activatedDoctors()
                .Select(d => new doctorDataList { nameAr = d.doctorInfo.fnameAr + " " + d.doctorInfo.lnameAr, nameEng = d.doctorInfo.fnameEng + " " + d.doctorInfo.lnameEng });
        }
        public static IEnumerable<doctor>getDocorsOfGeneralSearch(this tabeebekEntities db,string search)
        {
            /*
             1-search for doctor names
             * 2-search specialities names
             * 3-search for city names
             * 4-search for destrict names
             */
            var dataBySearchName = db.doctorInfoes.activatedDoctors().getDoctorsOfName(search);
            if (dataBySearchName.Count() > 0) return dataBySearchName;
            var dataBySearchSpecialityName = db.specialities
                .Where(s =>s.nameAr == search || s.nameEng == search)
                .groupingDoctros().activatedDoctors();
            if (dataBySearchSpecialityName.Count() > 0) return dataBySearchSpecialityName;
            var dataBySearchCityName = db.cities
                .Where(c =>c.nameAr == search || c.nameEng == search)
                .getDestricts().getDoctors().activatedDoctors();
            if (dataBySearchCityName.Count() > 0) return dataBySearchCityName;
            var dataBySearchDestrictName = db.destricts
                .Where(d => d.nameAr == search || d.nameEng == search)
                .getDoctors().activatedDoctors();
             return dataBySearchDestrictName;
        }
        public static IEnumerable<doctor> activatedDoctors(this IEnumerable<doctorInfo> doctorsInf)
        {
            return doctorsInf.Where(d=>d.stat==true).getDoctors();
        }
        public static bool isActivated(this doctor doctor)
        {
            return doctor.doctorInfo != null && doctor.doctorInfo.stat;
        }
        public static bool isProfileCreated(this doctor doctor)
        {
            return doctor.doctorInfo != null;
        }
        public static IEnumerable<doctor> activatedDoctors(this IEnumerable<doctor> doctors)
        {
            return doctors.Where(d => d.doctorInfo!=null&&d.doctorInfo.stat == true);
        }       
        public static IEnumerable<doctor> getDoctors(this IEnumerable<doctorInfo> doctorsInf)
        {
            List<doctor> doctors = new List<doctor>();
            foreach (var doctorInf in doctorsInf)
            {
                doctors.Add(doctorInf.doctor);
            }
            return doctors;
        }
        public static IEnumerable<doctor> getDoctors(this IEnumerable<destrict> destricts)
        {
            List<doctor> doctors = new List<doctor>();
            foreach (var destrict in destricts)
            {
                doctors.AddRange(destrict.doctors);
            }
            return doctors;
        }
        public static IEnumerable<doctor> allDoctors(this speciality speciality)
        {           
            if (speciality.subSpecialites.Count == 0) return speciality.doctors;
            var subPecialities = speciality.subSpecialites;
            List<doctor> doctors = new List<doctor>();
            foreach (var sp in subPecialities)
            {
                doctors.AddRange(sp.doctors);
            }
            doctors.AddRange(speciality.doctors);
            return doctors.Distinct();
        }
        public static IEnumerable<advice> allAdvices(this IEnumerable<doctor> doctors)
        {
            var advices=new List<advice>();
            foreach (var doctor in doctors)
            {
                advices.AddRange(doctor.advices);
            }
            return advices;
        }
        public static IEnumerable<destrict> getDestricts(this IEnumerable<city> cities)
        {
            List<destrict> destricts = new List<destrict>();
            foreach (var city in cities)
            {
                destricts.AddRange(city.destricts);
            }
            return destricts;
        }
        public static IEnumerable<doctor> groupingDoctros(this IEnumerable<speciality> specialities)
        {
            List<doctor> doctors = new List<doctor>();
            foreach (var sp in specialities)
            {
                doctors.AddRange(sp.allDoctors());
            }
            return doctors.Distinct();
        }       
        /*private functions**********************************private extension functions*******************************/
        public static IEnumerable<doctor> getDoctorsOfName(this IEnumerable<doctor> doctors,string name)
        {
            return doctors.Where(d => 
                d.doctorInfo.fnameAr.concatString(d.doctorInfo.lnameAr, " ") == name 
                || d.doctorInfo.fnameEng.concatString(d.doctorInfo.lnameEng, " ") == name
                ||d.doctorInfo.fnameAr==name||d.doctorInfo.lnameAr==name
                ||d.doctorInfo.fnameEng==name||d.doctorInfo.lnameEng==name);
        }
        private static string concatString(this string originString,string addedString,string sperator)
        {
            return originString + sperator + addedString;
        }
        private static IEnumerable<doctor> getDoctorsOfSpecialities(this IEnumerable<doctor> doctors,List<byte>specialityIDs)
        {
            if (specialityIDs.Count == 0) return doctors;
            if(specialityIDs.Count==1)
            {//this is main speciality ||all specialities
                if (specialityIDs[0] == 0)
                    return doctors;//this is all specialities
                else//return doctors of particular speciality
                    return globalController.db.specialities.FirstOrDefault(s => s.id == specialityIDs[0]).doctors;
            }
            //the passed array of ids of sub specialities ids
            return globalController.db.specialities.Where(s => s.id.existsIn(specialityIDs)).groupingDoctros();
        }        
        private static IEnumerable<doctor> getDoctorsOfDestrict(this IEnumerable<doctor> doctors, byte destrictID,byte cityID=0)
        {
            //all destricts in all cities
            if (destrictID == 0&&cityID==0) return doctors;
            //all destricts in particular city           
            if (destrictID == 0 && cityID != 0) return doctors.Where(d => d.destrict.cityID == cityID);
            return doctors.Where(d => d.destrictID == destrictID);
        }
        private static IEnumerable<doctor> getDoctorsOfGender(this IEnumerable<doctor> doctors, List<byte> genders)
        {
            if (genders.Count == 0 || genders.Count == 2) return doctors;
            else if (genders.Count == 1)
            {
                bool gender = (genders[0] == 0) ? false : true;
                return doctors.Where(d => d.doctorInfo.type == gender);
            }
            else
            {
                return doctors;
            }

        }
        private static IEnumerable<doctor> getDoctorsOfEducation(this IEnumerable<doctor> doctors, List<byte> educations)
        {
            if (educations.Count == 0) return doctors;
            return doctors.Where(d => d.doctorInfo.education.existsIn(educations));

        }
        private static IEnumerable<doctor> getDoctorsOfFeesRang(this IEnumerable<doctor> doctors,byte mark)
        {
            if (mark == 0)
                return doctors.Where(d => d.doctorInfo.examinFees < 50);
            else if (mark == 1)
                return doctors.Where(d => d.doctorInfo.examinFees >= 50 && d.doctorInfo.examinFees <= 100);
            else if (mark == 2)
                return doctors.Where(d => d.doctorInfo.examinFees > 100 && d.doctorInfo.examinFees <= 200);
            else if (mark == 3)
                return doctors.Where(d => d.doctorInfo.examinFees > 200);
            else return doctors;
        }
        private static IEnumerable<doctor> getDoctorsOfExaminFees(this IEnumerable<doctor> doctors, List<byte> marks)
        {
            if (marks.Count == 0) return doctors;
            List<doctor> matchedDoctors = new List<doctor>();
            foreach (var mark in marks)
            {
                matchedDoctors.AddRange(doctors.getDoctorsOfFeesRang(mark));
            }
            return matchedDoctors;
        }
        private static IEnumerable<speciality> findSpecialities(this IEnumerable<speciality> specialities, List<byte> ids)
        {
            if (ids.Count == 0||(ids.Count==1&&ids[0]==0))
            {
                //return all specialilities
                return specialities;
            }
            else if (ids.Count == 1)
                return specialities.Where(s => s.id == ids[0]);
            else
            {
                return specialities.Where(s => s.id.existsIn<byte>(ids));
            }
        }       
    }
    
}