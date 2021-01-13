// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etimo.Id.Dtos
{
    public class RoleResponseDto
    {
        public Guid                   role_id        { get; set; }
        public string                 name           { get; set; }
        public string                 description    { get; set; }
        public int                    application_id { get; set; }
        public DateTime               created_date   { get; set; }
        public DateTime               modified_date  { get; set; }
        public List<ScopeResponseDto> scopes         { get; set; }

        public static RoleResponseDto FromRole(Role role)
            => FromRole(role, true);

        public static RoleResponseDto FromRole(Role role, bool includeChildren)
        {
            var dto = new RoleResponseDto
            {
                role_id        = role.RoleId,
                name           = role.Name,
                description    = role.Description,
                application_id = role.ApplicationId,
                created_date   = role.CreatedDateTime,
                modified_date  = role.ModifiedDateTime,
            };

            if (includeChildren) { dto.scopes = role.Scopes?.Select(s => ScopeResponseDto.FromScope(s, false)).ToList(); }

            return dto;
        }
    }
}
