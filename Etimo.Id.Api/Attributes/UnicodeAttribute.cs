using Etimo.Id.Api.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Etimo.Id.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class UnicodeAttribute : ValidationAttribute
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

            var regex = new Regex(CharacterSetPatterns.UNICODECHARNOCRLF);
            if (!regex.IsMatch(stringValue))
            {
                return new ValidationResult($"The {memberName} field can only contain UNICODECHARNOCRLF characters (%x09 / %x20-7E / %x80-D7FF / %xE000-FFFD / %x10000-10FFFF).");
            }

            return ValidationResult.Success;
        }
    }
}
