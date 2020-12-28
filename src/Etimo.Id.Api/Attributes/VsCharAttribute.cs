using Etimo.Id.Api.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Etimo.Id.Api.Attributes
{
    /// <summary>
    ///     Allows ASCII characters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class VsCharAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Null values shouldn't be validated by this attribute.
            if (value == null) { return ValidationResult.Success; }

            string stringValue = value.ToString() ?? string.Empty;
            string memberName  = validationContext.MemberName;

            var regex = new Regex(CharacterSetPatterns.VSCHAR);
            if (!regex.IsMatch(stringValue))
            {
                return new ValidationResult($"The {memberName} field can only contain VSCHAR characters (%x20-7E).");
            }

            return ValidationResult.Success;
        }
    }
}
