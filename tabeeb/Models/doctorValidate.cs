using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using tabeeb.Areas.users.languages;
namespace tabeeb.Models
{
    [MetadataType(typeof(doctorValidate))]
    public partial class doctorAccountData
    {
 
    }
    public class doctorValidate
    {
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="usernameRequired")]
        [RegularExpression("^[a-z0-9_-]{3,16}$",ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="userNameMinimumLength")]
        [Remote("checkDoctorUserNameIfUnique", "doctor", "doctors", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="userNameIsAlreadyExists", HttpMethod = "POST")]
        public string username { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="emailRequired")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="emailNotValid")]
        public string mail { get; set; }
         [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="phoneRequired")]
         [RegularExpression("[0-9]{11}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="phoneNotValid")]
         [Remote("checkDoctorPhoneNumberIfUnique", "doctor", "doctors", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="phoneISAlreadyExists", HttpMethod = "POST")]       
        public string phone { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="passwordRequired")]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="passwordValidation")]
        public string password { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="cityRequired")]
        public byte cityID { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="destrictRequired")]
        [checkValueNoeEqual("0", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="destrictnotValid")]
        public byte destrictID { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="bookingTypeRequired")]
        public bool bookingType { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="professionImagerequired")]
        public string proImage { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="specialityrequired")]
       // [checkValueNoeEqual("0", ErrorMessage = "Please select a destrict from a city"")]
        [Remote("isValidSpecialityID", "doctor", "doctors", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="specialityNotValid", HttpMethod = "POST", AdditionalFields = "subSpID")]
        public byte spID { get; set; }
        [Remote("checkSubSpeciality", "doctor", "doctors", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="selectAtLeastOneSubSpeciality", HttpMethod = "POST", AdditionalFields = "spID")]
        public string[] subSpID { get; set; }
    }
}