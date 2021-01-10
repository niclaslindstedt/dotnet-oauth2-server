using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Etimo.Id.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ValidValuesAttribute : ValidationAttribute
    {
        private readonly object[] _validValues;

        public ValidValuesAttribute(params object[] validValues)
        {
            _validValues = validValues;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Null values shouldn't be validated by this attribute.
            if (value == null) { return ValidationResult.Success; }

            foreach (object validValue in _validValues)
            {
                if (value.Equals(validValue)) { return ValidationResult.Success; }
            }

            var validValues  = string.Join(", ", _validValues.Select(v => v.ToString()));
            var errorMessage = $"The {validationContext.MemberName} field can only have the following values: {validValues}";

            return new ValidationResult(errorMessage);
        }
    }
}
