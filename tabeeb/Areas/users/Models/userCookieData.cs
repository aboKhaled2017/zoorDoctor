using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Areas.users.Models
{
    public class userCookieData
    {
        public string providerName { get; set; }
        public string providerID { get; set; }
        public string username { get; set; }
        public string lang { get; set; }
    }
}