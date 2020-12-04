using Etimo.Id.Api.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Etimo.Id.Api.Attributes
{
    /// <summary>
    /// Allows ASCII characters except \ " and space.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NqCharAttribute : ValidationAttribute
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

            var regex = new Regex(CharacterSetPatterns.NQCHAR);
            if (!regex.IsMatch(stringValue))
            {
                return new ValidationResult($"The {memberName} field can only contain NQCHAR characters (%x21 / %x23-5B / %x5D-7E).");
            }

            return ValidationResult.Success;
        }
    }
}
