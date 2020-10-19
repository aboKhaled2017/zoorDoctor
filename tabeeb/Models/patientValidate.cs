using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using tabeeb.Areas.users.languages;
namespace tabeeb.Models
{
    [MetadataType(typeof(patientValidate))]
    public partial class patient
    {
       
    }
    public class patientValidate
    {
        public Guid id { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)),ErrorMessageResourceName ="usernameRequired")]
        [RegularExpression("^[a-zA-Z0-9_-]{3,16}$", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="userNameMinimumLength")]
        [userNameNotExists("id", "patient")]
        public string username { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="emailRequired")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="emailNotValid")]
        public string mail { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="phoneRequired")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{5})$", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="phoneNotValid")]
       // [noDuplicatedPhone("","patient")]       
        public string phone { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="passwordRequired")]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="passwordValidation")]
        public string password { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="genderIsRequired")]
        public bool type { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="birthDataIsRequired")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime birthDate { get; set; }
    }
}