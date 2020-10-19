using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Areas.Admin.Models
{
    public  class advPages
    {
        public byte id { get; set; }
        public List<byte> pagesID{set;get;}
        public string description { get; set; }
        public string content { get; set; }
        public string image { get; set; }
        public System.DateTime startDate { get; set; }
        public int amount { get; set; }
        public int waitingTime { get; set; }
        public int waitingAfterClosed { get; set; }
        public bool isViewAgain { get; set; }
        public bool stat { get; set; }
    }
}