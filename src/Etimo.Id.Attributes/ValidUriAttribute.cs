using System;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidUriAttribute : ValidationAttribute
    {
        private readonly bool _allowFragment;
        private readonly bool _allowHttp;

        public ValidUriAttribute(bool allowFragment = false, bool allowHttp = false)
        {
            _allowFragment = allowFragment;
            _allowHttp     = allowHttp;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Null values shouldn't be validated by this attribute.
            if (value == null) { return ValidationResult.Success; }

            var    uri        = value.ToString();
            string memberName = validationContext.MemberName;

            if (!IsWellFormedUriString(uri)) { return new ValidationResult($"The {memberName} field needs to be a valid URI."); }

            if (!_allowHttp && !UsesHttpsProtocol(uri))
            {
                return new ValidationResult($"The {memberName} field needs to use the HTTPS protocol.");
            }

            if (!_allowFragment && ContainsFragment(uri))
            {
                return new ValidationResult($"The {memberName} field cannot use fragments (#).");
            }

            return ValidationResult.Success;
        }

        protected bool IsWellFormedUriString(string uri)
            => Uri.IsWellFormedUriString(uri, UriKind.Absolute);

        protected bool UsesHttpsProtocol(string uri)
            => uri.StartsWith("https://");

        protected bool ContainsFragment(string uri)
            => uri.Contains("#");
    }
}
