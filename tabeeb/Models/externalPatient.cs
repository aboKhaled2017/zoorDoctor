using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tabeeb.Models
{
    public class externalPatient
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$")]
        public string mail { get; set; }
        [Required]
        [RegularExpression("([0-9]{4}-[0-9]{2}-[0-9]{2})|([0-9]{2}/[0-9]{2}/[0-9]{4})")]
        public string birthDate { get; set; }
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{5})$")]
        public string phone { get; set; }
        [Required]
        public bool type { get; set; }
        [Required]
        public string username { get; set; }
        public string providerName { get; set; }
        [Required]
        public string providerID { get; set; }
    }
}