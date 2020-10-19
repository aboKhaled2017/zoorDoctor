using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using tabeeb.Models;
using tabeeb.Controllers;
using System.Threading;
using System.Globalization;
namespace tabeeb.Areas.doctors.Controllers
{
    //this is main class for all sub classes that serve all doctors operations
    //this area only avialable for authenticated doctors
    [areaAuthorize("doctors", Roles = "doctor")]
    public class mainController : globalController
    {   // variable that contains all database data            
        protected tabeebekEntities db = new tabeebekEntities();
        //return id current doctor is authenticated(logged in)
        public bool isDoctorAuthenticated
        {
            private set { }
            get { return (User != null && User.Identity.IsAuthenticated && Roles.IsUserInRole("doctor")); }
        }
        //get the record of current logged doctor from database
        public doctor getCurrentDoctor(string id=null)
        {
            try
            {
                if (isDoctorAuthenticated)
                {
                    return db.doctors.FirstOrDefault(d => d.username == User.Identity.Name);
                }
                else if (id != null)
                {
                    Guid docID = Guid.Parse(id);
                    return db.doctors.Find(docID);
                }
                else { return null; }
            }
            catch(Exception){return null;}
            
        }
        //get the record of current logged doctor from db based on his/her username
        public doctor getCurrentDoctorByUserName(string userName)
        {
           return db.doctors.FirstOrDefault(d => d.username ==userName);
        }       
	}
}