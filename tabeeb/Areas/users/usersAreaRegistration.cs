using System.Web.Mvc;
using tabeeb.Controllers;
namespace tabeeb.Areas.users
{
    public class usersAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "users";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            /*context.MapRoute(
                "language_default",
                "{lang}/user-site/{action}/{id}",
                new { area = "users", Controller = "patient", action = "register", id = UrlParameter.Optional },
                new {lang="en-US|ar-EG"}
            ); */
            //main site
            context.MapRoute(
                "mainSite",
                "",
                new { area = "users", Controller = "index", action = "Index", id = UrlParameter.Optional}
            ); 
            //doctor page at users site
            context.MapRoute(
                "docotr-page",
                globalController.defaultPathDoctorPage+"/{id}",
                new { area = "users", Controller = "index", action = "doctorPage", id = UrlParameter.Optional }
            ); 
            //user site
            context.MapRoute(
                "userPage",
                globalController.defaultPathForUserSite+"/{action}/{id}",
                new { area = "users", Controller = "patient", action = "register", id = UrlParameter.Optional}
            );            
            /*search*/
            context.MapRoute(
                "users_search",
                globalController .defaultPathSearchPart,
                new { area = "users", Controller = "search", action = "search"}
            );
            context.MapRoute(
                null,
                "Advanced-search/{action}",
                new { area = "users", Controller = "search", action = "search"}
            );
            /*advices*/
            context.MapRoute(
                "users_advices",
                globalController.defaultPathAdvicesPart + "/{action}/{id}",
                new { area = "users", Controller = "advices", action = "advices", id = UrlParameter.Optional }
            );
            /*reserving*/
                context.MapRoute(
                "doctorReserving-page",
                globalController.defaultPathDoctorReservingPage + "/{name}/{id}",
                new { area = "users", Controller = "reservation", action = "doctorReservingsPage", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "reserving",
                globalController.defaultPathReservation + "/{action}/{id}",
                new { area = "users", Controller = "reservation", action = "reservation", id = UrlParameter.Optional }
            );
            /*specialities*/
            context.MapRoute(
                null,
                globalController.defaultPathSpecialityPart + "/{name}",
                new { area = "users", Controller = "specialities", action = "getSpecialityDoctors" }
            );
            context.MapRoute(
                null,
                globalController.defaultPathSpecialityPart,
                new { area = "users", Controller = "specialities", action = "specialities" }
            );
            context.MapRoute(
                "users_specialities",
                globalController.defaultPathSpecialityPart + "/{Controller}/{action}/{id}",
                new { area = "users", Controller = "specialities", action = "specialities", id = UrlParameter.Optional }
            ); 
            /*default*/
            context.MapRoute(
               "users_default",
               globalController .defaultPathLanguagePart+ "/{lang}/{id}",
               new { area = "users", Controller = "index", action = "changeLanguage", id = UrlParameter.Optional, lang = "ar" }
           );
            context.MapRoute(
                "generalPage",
                "{action}/{id}",
                new { area = "users", Controller = "index", action = "Index", id = UrlParameter.Optional }
            );              
        }
    }
}