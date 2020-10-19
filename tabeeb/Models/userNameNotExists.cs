using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tabeeb.Models;
using System.Web.Security;
using tabeeb.Controllers;
using tabeeb.Areas.users.languages;
namespace tabeeb.Models
{
    public class userNameNotExists : ValidationAttribute
    {
        string idProperty;
        string typeProperty;
        private  string validationMessage;
        private  string nameIsRequired;
        private  string validationName;
        public userNameNotExists(string idproperty,string typeProperty)
        {
            this.idProperty = idproperty;
            this.typeProperty = typeProperty;
            validationMessage = Resource1.userNameIsAlreadyExists;
            nameIsRequired = Resource1.usernameRequired;
            validationName = Resource1.userNameMinimumLength;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(nameIsRequired);
            var db = globalController.db;
            var id = default(Guid);
            if(idProperty=="id")
            {
                var currentID = validationContext.ObjectType.GetProperty(idProperty);
                var idValue = currentID.GetValue(validationContext.ObjectInstance, null);
                id =(Guid)idValue;
            }            
            string currentName = (string)value;
            if (typeProperty == "doctor")
            {               
                if(!db.doctors.Any(d=>d.id!=id&&d.username==currentName))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(validationMessage);
                }
            }
            else
            {
                if(!db.patients.Any(p => p.id != id && p.username == currentName &&p.providerName==null))
                    return ValidationResult.Success;
                else
                {
                    return new ValidationResult(validationMessage);
                }
            }
        } 
    }
    
}