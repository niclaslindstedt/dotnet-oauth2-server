using Etimo.Id.Api.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Etimo.Id.Api.Attributes
{
    /// <summary>
    ///     Allows ASCII characters except \ and ".
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NqsCharAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Null values shouldn't be validated by this attribute.
            if (value == null) { return ValidationResult.Success; }

            string stringValue = value.ToString() ?? string.Empty;
            string memberName  = validationContext.MemberName;

            if (!Regex.IsMatch(stringValue, CharacterSetPatterns.NQSCHAR))
            {
                return new ValidationResult($"The {memberName} field can only contain NQSCHAR characters (%x20-21 / %x23-5B / %x5D-7E).");
            }

            return ValidationResult.Success;
        }
    }
}
