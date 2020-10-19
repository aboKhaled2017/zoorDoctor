using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Models
{
    public class doctorCard
    {
        public System.Guid id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string professionTitle { get; set; }
        public string spName { get; set; }
        public Nullable<byte> spSuperID { get; set; }
        public string image { get; set; }
        public string clinicAddress { get; set; }
        public short price { get; set; }
        public string timing { get; set; }
        public short waitingTime { get; set; }
        public int viewers { get; set; }
        public double rate { get; set; }
        public string cardLanguage { get; set; }
    }
}