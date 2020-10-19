using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace tabeeb.Models
{
    public class checkValueNoeEqual : ValidationAttribute
    {
        private string validationMessage;
        private string exceptedValue;
        public checkValueNoeEqual(string exceptedValue)
        {
            this.exceptedValue = exceptedValue;
            validationMessage = tabeeb.Areas.users.languages.Resource1.destrictRequired;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var selectedCity = value.ToString().Trim();
            if (selectedCity != exceptedValue)
                return ValidationResult.Success;
            else
                return new ValidationResult(validationMessage);
        }
    }
}