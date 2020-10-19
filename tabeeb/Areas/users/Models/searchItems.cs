using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Areas.users.Models
{
    public class searchItems
    {
        public string q { get; set; }
        private List<byte> specialities;
        private List<byte> prices;
        private List<byte> educations;
        private List<byte> types;
        private byte destrictid;
        private byte cityid;
        private byte pagenumber;
        public List<byte> specialityIDs { get{return (specialities==null)?new List<byte>():specialities;} set { specialities = value; } }
        public byte cityID { get { return cityid; } set { cityid = value; } }
        public byte destrictID { get { return destrictid; } set { destrictid = value; } }
        public List<byte> price { get { return (prices == null) ? new List<byte>() : prices; } set { prices = value; } }
        public List<byte> education { get { return (educations == null) ? new List<byte>() : educations; } set { educations = value; } }
        public List<byte> type { get { return (types == null) ? new List<byte>() : types; } set { types = value; } }
        public string docName { get; set; }
        public byte pageNumber { get { return (pagenumber == 0) ? (byte)1 : pagenumber; } set { pagenumber = value; } }       
    }
}