using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using tabeeb.Models;
using tabeeb.Controllers;
namespace tabeeb.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [areaAuthorize("admin",Roles = "admin")]
    public class mainController : globalController
    {
        protected tabeebekEntities db = new tabeebekEntities();
        public bool compareDate(DateTime date1, DateTime date2)
        {
            if (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day) return true;
            return false;
        }
        internal protected bool isAdminAuthenticated()
        {
            return (User.Identity.IsAuthenticated&&Roles.IsUserInRole("admin"));
        }
	}
}