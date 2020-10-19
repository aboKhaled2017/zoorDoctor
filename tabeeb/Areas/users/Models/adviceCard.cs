using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Areas.users.Models
{
    public class adviceCard
    {
        public Guid id { get; set; }
        public int comments { get; set; }
        public int likes { get; set; }
        public int shares { get; set; }
        public int seens { get; set; }
        public DateTime date { get; set; }
        public string content { get; set; }
        public string name { get; set; }
        public string speciality { get; set; }
        public string image { get; set; }
    }
}