using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Areas.users.Models
{
    public class specialityDescription
    {
        public byte id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public int totalSpecializedDoctors { get; set; }
    }
}