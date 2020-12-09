// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Roles;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etimo.Id.Api.Scopes
{
    public class ScopeResponseDto
    {
        public Guid scope_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int application_id { get; set; }
        public DateTime created_date { get; set; }
        public DateTime modified_date { get; set; }
        public List<RoleResponseDto> roles { get; set; }


        public static ScopeResponseDto FromScope(Scope scope) => FromScope(scope, true);
        public static ScopeResponseDto FromScope(Scope scope, bool includeChildren)
        {
            var dto = new ScopeResponseDto
            {
                scope_id = scope.ScopeId,
                name = scope.Name,
                description = scope.Description,
                application_id = scope.ApplicationId,
                created_date = scope.CreatedDateTime,
                modified_date = scope.ModifiedDateTime
            };

            if (includeChildren)
            {
                dto.roles = scope.Roles.Select(r =>
                    RoleResponseDto.FromRole(r, false)).ToList();
            }

            return dto;
        }
    }
}
