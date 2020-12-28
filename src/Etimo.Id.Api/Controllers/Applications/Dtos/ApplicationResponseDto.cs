// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Roles;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etimo.Id.Api.Applications
{
    public class ApplicationResponseDto
    {
        public int                   application_id { get; set; }
        public string                name           { get; set; }
        public string                description    { get; set; }
        public string                type           { get; set; }
        public string                homepage_uri   { get; set; }
        public string                redirect_uri   { get; set; }
        public Guid                  client_id      { get; set; }
        public Guid                  user_id        { get; set; }
        public DateTime              created_date   { get; set; }
        public DateTime              modified_date  { get; set; }
        public List<RoleResponseDto> roles          { get; set; }

        public static ApplicationResponseDto FromApplication(Application application)
            => FromApplication(application, true);

        public static ApplicationResponseDto FromApplication(Application application, bool includeChildren)
        {
            var dto = new ApplicationResponseDto
            {
                application_id = application.ApplicationId,
                name           = application.Name,
                description    = application.Description,
                type           = application.Type,
                homepage_uri   = application.HomepageUri,
                redirect_uri   = application.RedirectUri,
                client_id      = application.ClientId,
                user_id        = application.UserId,
                created_date   = application.CreatedDateTime,
                modified_date  = application.ModifiedDateTime,
            };

            if (includeChildren) { dto.roles = application.Roles?.Select(r => RoleResponseDto.FromRole(r, true)).ToList(); }

            return dto;
        }
    }
}
