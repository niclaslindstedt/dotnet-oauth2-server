// ReSharper disable InconsistentNaming

using System;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Entities;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Api.Scopes.Dtos
{
    public class ScopeRequestDto
    {
        [Required]
        [MinLength(2), MaxLength(32)]
        [NqsChar]
        public string name { get; set; }

        [MaxLength(128)]
        [NqsChar]
        public string description { get; set; }

        [Required]
        public int? application_id { get; set; }

        public Scope ToScope(Guid? scopeId = null)
        {
            return new Scope
            {
                ScopeId = scopeId ?? Guid.NewGuid(),
                Name = name,
                Description = description,
                ApplicationId = application_id ?? default
            };
        }
    }
}
