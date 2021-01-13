// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;
using System.Linq;

namespace Etimo.Id.Dtos
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
                redirect_uris                                   = application.RedirectUri.Split(" ").ToList(),
                failed_logins_before_locked                     = application.FailedLoginsBeforeLocked,
                failed_logins_lock_lifetime_minutes             = application.FailedLoginsLockLifetimeMinutes,
                authorization_code_lifetime_seconds             = application.AuthorizationCodeLifetimeSeconds,
                access_token_lifetime_minutes                   = application.AccessTokenLifetimeMinutes,
                refresh_token_lifetime_days                     = application.RefreshTokenLifetimeDays,
                allow_credentials_in_body                       = application.AllowCredentialsInBody,
                allow_custom_query_parameters_in_redirect_uri   = application.AllowCustomQueryParametersInRedirectUri,
                allow_authorization_code_grant                  = application.AllowAuthorizationCodeGrant,
                allow_client_credentials_grant                  = application.AllowClientCredentialsGrant,
                allow_resource_owner_password_credentials_grant = application.AllowResourceOwnerPasswordCredentialsGrant,
                allow_implicit_grant                            = application.AllowImplicitGrant,
                generate_refresh_token_for_authorization_code   = application.GenerateRefreshTokenForAuthorizationCode,
                generate_refresh_token_for_client_credentials   = application.GenerateRefreshTokenForClientCredentials,
                generate_refresh_token_for_password_credentials = application.GenerateRefreshTokenForPasswordCredentials,
                generate_refresh_token_for_implicit_flow        = application.GenerateRefreshTokenForImplicitFlow,
                client_id                                       = application.ClientId,
                client_secret                                   = application.ClientSecret,
                user_id                                         = application.UserId,
                created_date                                    = application.CreatedDateTime,
                modified_date                                   = application.ModifiedDateTime,
            };
    }
}
