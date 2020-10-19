using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using tabeeb.Areas.users.languages;
namespace tabeeb.Areas.users.Models
{
    public class externaluserProfile
    {
        [Required(ErrorMessageResourceType = (typeof(Resource1)), ErrorMessageResourceName = "phoneRequired")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{5})$", ErrorMessageResourceType = (typeof(Resource1)), ErrorMessageResourceName = "phoneNotValid")]    
        public string phone { get; set; }
        [Required(ErrorMessageResourceType = (typeof(Resource1)), ErrorMessageResourceName = "genderIsRequired")]
        public bool type { get; set; }
        [Required(ErrorMessageResourceType = (typeof(Resource1)), ErrorMessageResourceName = "birthDataIsRequired")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime birthDate { get; set; }
    }
}