using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using tabeeb.Controllers;
namespace tabeeb.Models
{
    public class emailNoeExists : ValidationAttribute
    {
        private string idProperty;
        private string validationMessage;
        public emailNoeExists(string idproperty)
        {
            this.idProperty = idproperty;
            validationMessage = "email is already exists";
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var db = globalController.db;
            var currentID = validationContext.ObjectType.GetProperty(idProperty);
            var id = (Guid)currentID.GetValue(validationContext.ObjectInstance, null);
            string email = "";
            patient currentPatient = db.patients.FirstOrDefault(d => d.id == id);
            if (currentPatient != null)
            {
                email = currentPatient.mail;
            }
            bool isPtientFound = db.patients.Any(p => p.mail == (string)value);
            if (!isPtientFound || (email == (string)value && currentPatient != null))
                return ValidationResult.Success;
            else
                return new ValidationResult(validationMessage);
        }
    }
}