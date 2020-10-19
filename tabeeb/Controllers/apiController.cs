using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tabeeb.Models;
using tabeeb.Controllers;
namespace tabeeb.Controllers
{
    public class patientController : ApiController
    {
        [HttpGet]
        public List<patient> myApi()
        {
            var db = globalController.db;
            db.Configuration.ProxyCreationEnabled = false;
            return db.patients.ToList();
        }
    }
}
