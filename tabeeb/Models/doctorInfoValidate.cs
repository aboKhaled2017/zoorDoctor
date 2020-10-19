using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using tabeeb.Areas.users.languages;
namespace tabeeb.Models
{
    [MetadataType(typeof(doctorInfoValidate))]
    public partial class doctorInfo
    {
    }
    public class doctorInfoValidate
    {
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="firstNamerequired")]
        [StringLength(25, MinimumLength = 3, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="firstNameValidation")]
        [RegularExpression("\\s?[a-zA-Z]{3,15}(\\s[a-zA-Z]{3,15})?\\s?", ErrorMessageResourceType = (typeof(Resource1)), ErrorMessageResourceName = "firstNameNotValid")]
        public string fnameEng { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="lastNameRequired")]
        [StringLength(25, MinimumLength = 3, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="lastnameValidation")]
        [RegularExpression("\\s?[a-zA-Z]{3,15}(\\s[a-zA-Z]{3,15})?\\s?", ErrorMessageResourceType = (typeof(Resource1)), ErrorMessageResourceName = "lastnameNotvalid")]
        public string lnameEng { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="firstNamerequired")]
        [StringLength(25, MinimumLength = 3, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="firstNameValidation")]
        [RegularExpression("\\s?[\u0600-\u06FF]{3,15}(\\s[\u0600-\u06FF]{3,15})?\\s?", ErrorMessageResourceType = (typeof(Resource1)), ErrorMessageResourceName = "firstNameNotValid")]
        public string fnameAr { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="lastNameRequired")]
        [StringLength(25, MinimumLength = 3, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="lastnameValidation")]
        [RegularExpression("\\s?[\u0600-\u06FF]{3,15}(\\s[\u0600-\u06FF]{3,15})?\\s?", ErrorMessageResourceType = (typeof(Resource1)), ErrorMessageResourceName = "lastnameNotvalid")]
        public string lnameAr { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="genderIsRequired")]
        public bool type { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="professiontitleisrequired")]
        [RegularExpression("[^أ-ى]{10,100}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="professiontitleValidationEng")]
        [StringLength(100, MinimumLength = 10, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="professiontitleLength")]
        public string profTitleEng { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="professiontitleisrequired")]
        [RegularExpression("[^a-zA-Z]{10,100}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="professiontitleValidationAr")]
        [StringLength(100, MinimumLength = 10, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="professiontitleLength")]
        public string profTitleAr { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="aboutDoctorrequired")]
        [RegularExpression("[^أ-ى]{20,200}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="aboutDoctorValidationEng")]
        [StringLength(200, MinimumLength = 20, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="aboutDoctorLength")]  
        public string aboutDocEng { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="aboutDoctorrequired")]
        [RegularExpression("[^a-zA-Z]{20,200}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="aboutDoctorValidationAr")]
        [StringLength(200, MinimumLength = 20, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="aboutDoctorLength")]  
        public string aboutDocAr { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="reservationtyperequired")]
        public bool reservingType { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="clinicAddressrequired")]
        [RegularExpression("[^أ-ى]{10,150}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="clinicAddressValidationEng")]
        [StringLength(150, MinimumLength = 10, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="clinicAddressLength")]
        public string clinicAddressEng { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="clinicAddressrequired")]
        [RegularExpression("[^a-zA-Z]{10,150}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="clinicAddressValidationAr")]
        [StringLength(150, MinimumLength = 10, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="clinicAddressLength")]
        public string clinicAddressAr { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="availableTimeRequired")]
        [RegularExpression("[^أ-ى]{8,100}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="availableTimeValidationEng")]
        [StringLength(50, MinimumLength = 8, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="avialableTimeLength")]
        public string availableTimeEng { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="availableTimeRequired")]
        [RegularExpression("[^a-zA-Z]{8,100}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="availableTimeValidationAr")]
        [StringLength(50, MinimumLength = 8, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="avialableTimeLength")]
        public string availableTimeAr { get; set; }
        [RegularExpression("[^أ-ى]{0,300}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="aboutClinicValidationEng")]
        public string aboutClnicEng { get; set; }
        [RegularExpression("[^a-zA-Z]{0,300}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="aboutClinicValidationAr")]
        public string aboutClnicAr { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="educationrequired")]
        [RegularExpression("[0-6]{1}", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="educationNotValid")]
        public byte education { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="examinFeeRequired")]
        [Range(10, 20000, ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="examinfeeRang")]
        public short examinFees { get; set; }
        [Required(ErrorMessageResourceType = (typeof(Resource1)), ErrorMessageResourceName = "waitingTimerequired")]
        [Range(0,5*60, ErrorMessageResourceType = (typeof(Resource1)), ErrorMessageResourceName = "waitingTimerequired")]
        public short waitingTime { get; set; }
        [Required(ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="personalImgrequired")]
        [RegularExpression("[^\\s]+\\.{1}(jpg|png|gif|jpeg|JPG|PNG|GIF|JPEG)$", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="personalImgNotValid")]
        public string image { get; set; }
        [RegularExpression("^\\[(\\{\"s\":\"(y|f|l|i|t|g)\",\"u\":\"(.)+\"\\}(,)?)*\\]$", ErrorMessageResourceType=(typeof(Resource1)), ErrorMessageResourceName="urlNotValid")]
        public string urls { get; set; }
    }
}