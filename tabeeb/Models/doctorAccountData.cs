using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace tabeeb.Models
{
    public partial class doctorAccountData
    {
        public string username { get; set; }
        public string mail { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public byte cityID { get; set; }
        public byte destrictID { get; set; }
        public bool bookingType { get; set; }
        public string proImage { get; set; }
        public byte spID { get; set; }
        public IEnumerable<string> subSpID { get; set; }
    }
}