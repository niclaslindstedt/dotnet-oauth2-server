using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Etimo.Id.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MinCountAttribute : ValidationAttribute
    {
        private readonly int _count;

        public MinCountAttribute(int count)
        {
            _count = count;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var values = value as List<string>;
            if (values == null) { return ValidationResult.Success; }

            string memberName = validationContext.MemberName;

            if (values.Count() < _count) { return new ValidationResult($"The {memberName} field should have at least {_count} values."); }

            return ValidationResult.Success;
        }
    }
}
