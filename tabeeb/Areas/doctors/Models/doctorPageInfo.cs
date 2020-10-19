using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace tabeeb.Areas.doctors.Models
{
    public class doctorPageInfo
    {
        public Guid id { set; get; }
       public string name {  set; get; }
       public string image { set; get; }
       public string cityname { set; get; }
       public string destrictName { set; get; }
       public string phone { set; get; }
       public string urls { set; get; }
       public short examinFee { set; get; }
       public string availableTime { set; get; }
       public double rate  {set; get;  }
       public string services { set; get; }
       public string aboutDoc { set; get; }
       public string spName { get; set; }
       public string professionName { set; get; }
    }
}