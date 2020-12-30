// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;

namespace Etimo.Id.Api.Applications
{
    public class ApplicationSecretResponseDto : ApplicationResponseDto
    {
        public string client_secret { get; set; }

        public new static ApplicationSecretResponseDto FromApplication(Application application)
            => new()
            {
                application_id                                  = application.ApplicationId,
                name                                            = application.Name,
                description                                     = application.Description,
                type                                            = application.Type,
                logo_base64                                     = application.LogoBase64,
                homepage_uri                                    = application.HomepageUri,
                redirect_uri                                    = application.RedirectUri,
                authorization_code_lifetime_seconds             = application.AuthorizationCodeLifetimeSeconds,
                access_token_lifetime_minutes                   = application.AccessTokenLifetimeMinutes,
                refresh_token_lifetime_days                     = application.RefreshTokenLifetimeDays,
                allow_credentials_in_body                       = application.AllowCredentialsInBody,
                allow_authorization_code_grant                  = application.AllowAuthorizationCodeGrant,
                allow_client_credentials_grant                  = application.AllowClientCredentialsGrant,
                allow_resource_owner_password_credentials_grant = application.AllowResourceOwnerPasswordCredentialsGrant,
                allow_implicit_grant                            = application.AllowImplicitGrant,
                client_id                                       = application.ClientId,
                client_secret                                   = application.ClientSecret,
                user_id                                         = application.UserId,
                created_date                                    = application.CreatedDateTime,
                modified_date                                   = application.ModifiedDateTime,
            };
    }
}
