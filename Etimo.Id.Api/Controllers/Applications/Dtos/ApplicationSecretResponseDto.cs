// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;

namespace Etimo.Id.Api.Applications
{
    public class ApplicationSecretResponseDto : ApplicationResponseDto
    {
        public string client_secret { get; set; }

        public new static ApplicationSecretResponseDto FromApplication(Application application)
        {
            return new ApplicationSecretResponseDto
            {
                application_id = application.ApplicationId,
                name = application.Name,
                description = application.Description,
                type = application.Type,
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
