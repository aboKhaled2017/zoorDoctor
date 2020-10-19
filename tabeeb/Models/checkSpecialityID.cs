using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using tabeeb.Controllers;
namespace tabeeb.Models
{
    public class checkSpecialityID : ValidationAttribute
    {
        private string spIDProperty;
        private string validationMessage;
        public checkSpecialityID(string spIDProperty)
        {
            this.spIDProperty = spIDProperty;
            validationMessage = tabeeb.Areas.users.languages.Resource1.selectAtLeastOneSubSpeciality;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var db = globalController.db;
            var currentspIDProperty = validationContext.ObjectType.GetProperty(spIDProperty);
            byte selectedSpID = (byte)currentspIDProperty.GetValue(validationContext.ObjectInstance, null);
            IEnumerable<string> selectedSubSpID =(value!=null)?(IEnumerable<string>)value:null;
            if(db.specialities.Any(s=>s.id==selectedSpID&&s.subSpecialites.Count==0))
                return ValidationResult.Success; 
            if (selectedSubSpID!=null&&selectedSubSpID.Count()>0&&selectedSpID!=null)
                return ValidationResult.Success;
            else
                return new ValidationResult(validationMessage);
        }
    }
}