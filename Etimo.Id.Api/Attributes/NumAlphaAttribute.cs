using Etimo.Id.Api.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Etimo.Id.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NumAlphaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Null values shouldn't be validated by this attribute.
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var stringValue = value.ToString() ?? string.Empty;
            var memberName = validationContext.MemberName;

            var regex = new Regex(CharacterSetPatterns.NUMALPHA);
            if (!regex.IsMatch(stringValue))
            {
                return new ValidationResult($"The {memberName} field can only contain NUMALPHA characters (%x30-39 / %x41-5A / %x61-7A).");
            }

            return ValidationResult.Success;
        }
    }
}
