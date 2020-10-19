using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Areas.users.Models
{
    public class thankForBookingData
    {
        public string name { get; set; }
        public DateTime appointementDate { get; set; }
        public string clinicAddress { get; set; }
        public string interval { get; set; }
        public bool bookingType { get; set; }
    }
}