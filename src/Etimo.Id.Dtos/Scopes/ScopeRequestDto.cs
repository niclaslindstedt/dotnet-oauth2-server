// ReSharper disable InconsistentNaming

using Etimo.Id.Attributes;
using Etimo.Id.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Dtos
{
    public class ScopeRequestDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(32)]
        [NqsChar]
        public string name { get; set; }

        [MaxLength(128)]
        [NqsChar]
        public string description { get; set; }

        [Required]
        public int? application_id { get; set; }

        public Scope ToScope(Guid? scopeId = null)
        {
            var scope = new Scope
            {
                Name          = name,
                Description   = description,
                ApplicationId = application_id ?? default,
            };

            if (scopeId != null) { scope.ScopeId = scopeId.Value; }

            return scope;
        }
    }
}
