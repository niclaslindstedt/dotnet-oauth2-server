using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Etimo.Id.Api.Attributes
{
    /// <summary>
    ///     Allows a base64 encoded string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Base64Attribute : ValidationAttribute
    {
        private static readonly char[] Base64Chars =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y',
            'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
            'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/',
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Null values shouldn't be validated by this attribute.
            if (value == null) { return ValidationResult.Success; }

            string stringValue  = value.ToString() ?? string.Empty;
            string memberName   = validationContext.MemberName;
            var    errorMessage = $"The {memberName} field must contain a valid base64 encoded string.";

            if (stringValue == null
             || stringValue.Length == 0
             || stringValue.Length % 4 != 0
             || stringValue.Contains(' ')
             || stringValue.Contains('\t')
             || stringValue.Contains('\r')
             || stringValue.Contains('\n')) { return new ValidationResult(errorMessage); }

            int index = stringValue.Length - 1;

            if (stringValue[index] == '=') { index--; }

            if (stringValue[index] == '=') { index--; }

            for (var i = 0; i <= index; i++)
            {
                if (!Base64Chars.Contains(stringValue[i])) { return new ValidationResult(errorMessage); }
            }

            return ValidationResult.Success;
        }
    }
}
