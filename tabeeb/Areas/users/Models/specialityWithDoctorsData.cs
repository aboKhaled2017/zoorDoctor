using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Areas.users.Models
{
    public class specialityWithDoctorsData
    {
        public byte id { set; get; }
        public string name { set; get; }
        public List<tabeeb.Areas.doctors.Models.subSpecialityData> subSpecialities { get; set; }
        public List<Tuple<Guid,string>> doctors { get; set; }
    }
}