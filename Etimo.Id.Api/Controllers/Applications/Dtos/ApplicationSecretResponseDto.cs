// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;
using System;

namespace Etimo.Id.Api.Applications
{
    public class ApplicationSecretResponseDto
    {
        public int application_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string homepage_uri { get; set; }
        public string redirect_uri { get; set; }
        public Guid client_id { get; set; }
        public string client_secret { get; set; }
        public Guid user_id { get; set; }
        public DateTime created_date { get; set; }
        public DateTime modified_date { get; set; }

        public static ApplicationSecretResponseDto FromApplication(Application application)
        {
            return new ApplicationSecretResponseDto
            {
                application_id = application.ApplicationId,
                name = application.Name,
                description = application.Description,
                homepage_uri = application.HomepageUri,
                redirect_uri = application.RedirectUri,
                client_id = application.ClientId,
                client_secret = application.ClientSecret,
                user_id = application.UserId,
                created_date = application.CreatedDateTime,
                modified_date = application.ModifiedDateTime
            };
        }
    }
}
