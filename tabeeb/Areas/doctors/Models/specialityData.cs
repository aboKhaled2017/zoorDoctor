using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Areas.doctors.Models
{
    public class specialityData
    {
        public byte id { set; get; }
        public string name { set; get; }
        public List<subSpecialityData> subSpecialities { get; set; }
    }
}