using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Areas.doctors.Models
{
    public class cityData
    {
        public byte id { get; set; }
        public string name { get; set; }
        public List<destrictData> destricts { get; set; }
    }
}