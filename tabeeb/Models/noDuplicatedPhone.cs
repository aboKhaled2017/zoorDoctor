using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;
using tabeeb.Controllers;
namespace tabeeb.Models
{
    public class noDuplicatedPhone : ValidationAttribute
    {
        private string idProperty;
        private string type;
        private  string validationMessage;
        public noDuplicatedPhone(string idproperty,string Type)
        {
            this.idProperty = idproperty;
            this.type = Type;
            validationMessage = tabeeb.Areas.users.languages.Resource1.phoneISAlreadyExists;
                 
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var db = globalController.db;
            var id = default(Guid);
            if (idProperty == "id")
            {
                var currentID = validationContext.ObjectType.GetProperty(idProperty);
                var idValue = currentID.GetValue(validationContext.ObjectInstance, null);
                id = (Guid)idValue;
            }   
            string phone = (string)value;
            if(type=="doctor")
            {
                if (!db.doctors.Any(p => p.id != id && p.phone == phone))
                {
                    return ValidationResult.Success;
                }
                else
                    return new ValidationResult(validationMessage);
            }
            else
            {
                if (!db.patients.Any(p => p.id != id && p.phone == phone))
                {
                    return ValidationResult.Success;
                }
                else
                    return new ValidationResult(validationMessage);
            }
        }
    }
    
}