using System.Web.Mvc;
using tabeeb.Controllers;
namespace tabeeb.Areas.doctors
{
    public class doctorsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "doctors";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "doctors_default",
                globalController.defaultPathForDoctorsArea+"/{action}/{id}",
                new { controller = "doctor", action = "doctorPage", id = UrlParameter.Optional }
            );         
        }
    }
}