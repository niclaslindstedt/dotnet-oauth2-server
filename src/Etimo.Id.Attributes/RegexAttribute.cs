using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Etimo.Id.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RegexAttribute : ValidationAttribute
    {
        private readonly string _errorMessage;
        private readonly string _pattern;

        public RegexAttribute(string pattern, string errorMessage)
        {
            _pattern      = pattern;
            _errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Null values shouldn't be validated by this attribute.
            if (value == null) { return ValidationResult.Success; }

            var stringValue = value.ToString();
            var regex       = new Regex(_pattern);
            if (regex.IsMatch(stringValue)) { return ValidationResult.Success; }

            return new ValidationResult(_errorMessage);
        }
    }
}
