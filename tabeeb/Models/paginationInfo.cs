using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tabeeb.Models
{
    public class paginationInfo
    {
        public int totalItems { get; set; }
        public int itemsPerPage { get; set; }
        public int currentPage { get; set; }
        public int totalPages { set { } get { return (int)Math.Ceiling((decimal)totalItems / itemsPerPage); } }
    }
}