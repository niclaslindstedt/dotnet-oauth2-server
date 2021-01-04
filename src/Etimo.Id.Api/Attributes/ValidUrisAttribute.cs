using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Etimo.Id.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidUrisAttribute : ValidUriAttribute
    {
        private readonly bool _allowFragment;
        private readonly bool _allowHttp;

        public ValidUrisAttribute(bool allowFragment = false, bool allowHttp = false)
        {
            _allowFragment = allowFragment;
            _allowHttp     = allowHttp;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var uris = value as List<string>;
            if (uris == null || !uris.Any()) { return ValidationResult.Success; }

            string memberName = validationContext.MemberName;

            foreach (string uri in uris)
            {
                if (!IsWellFormedUriString(uri)) { return new ValidationResult($"The {memberName} field needs to contain valid URIs."); }

                if (!_allowHttp && !UsesHttpsProtocol(uri))
                {
                    return new ValidationResult($"The {memberName} field needs to contain URIs using the HTTPS protocol.");
                }

                if (!_allowFragment && ContainsFragment(uri))
                {
                    return new ValidationResult($"The {memberName} field can only contain URIs without fragments (#).");
                }
            }

            return ValidationResult.Success;
        }
    }
}
