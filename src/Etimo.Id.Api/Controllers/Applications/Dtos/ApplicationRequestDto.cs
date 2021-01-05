// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Attributes;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Etimo.Id.Api.Applications
{
    public class ApplicationRequestDto
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
        [ValidValues("public", "confidential")]
        public string type { get; set; }

        [Base64]
        public string logo_base64 { get; set; }

        [Required]
        [ValidUri(allowFragment: true)]
        public string homepage_uri { get; set; }

        [Required]
        [MinCount(1)]
        [ValidUris]
        public List<string> redirect_uris { get; set; }

        [Range(1, 1440)]
        public int failed_logins_before_locked { get; set; } = 3;

        [Range(1, 1440)]
        public int failed_logins_lock_lifetime_minutes { get; set; } = 30;

        [Range(5, 600)]
        public int authorization_code_lifetime_seconds { get; set; } = 180;

        [Range(1, 1440)]
        public int access_token_lifetime_minutes { get; set; } = 15;

        [Range(1, 90)]
        public int refresh_token_lifetime_days { get; set; } = 30;

        public bool  allow_credentials_in_body                       { get; set; } = false;
        public bool  allow_custom_query_parameters_in_redirect_uri   { get; set; } = false;
        public bool  allow_authorization_code_grant                  { get; set; } = true;
        public bool  allow_client_credentials_grant                  { get; set; } = true;
        public bool  allow_resource_owner_password_credentials_grant { get; set; } = false;
        public bool  allow_implicit_grant                            { get; set; } = false;
        public bool  generate_refresh_token_for_authorization_code   { get; set; } = true;
        public bool  generate_refresh_token_for_client_credentials   { get; set; } = false;
        public bool  generate_refresh_token_for_password_credentials { get; set; } = false;
        public bool  generate_refresh_token_for_implicit_flow        { get; set; } = true;
        public Guid? user_id                                         { get; set; }

        public Application ToApplication(int? applicationId = null)
            => new()
            {
                ApplicationId                              = applicationId ?? default,
                Name                                       = name,
                Description                                = description,
                Type                                       = type,
                LogoBase64                                 = logo_base64,
                HomepageUri                                = homepage_uri,
                RedirectUri                                = string.Join(" ", redirect_uris.Select(u => u.Trim())),
                FailedLoginsBeforeLocked                   = failed_logins_before_locked,
                FailedLoginsLockLifetimeMinutes            = failed_logins_lock_lifetime_minutes,
                AuthorizationCodeLifetimeSeconds           = authorization_code_lifetime_seconds,
                AccessTokenLifetimeMinutes                 = access_token_lifetime_minutes,
                RefreshTokenLifetimeDays                   = refresh_token_lifetime_days,
                AllowCredentialsInBody                     = allow_credentials_in_body,
                AllowCustomQueryParametersInRedirectUri    = allow_custom_query_parameters_in_redirect_uri,
                AllowAuthorizationCodeGrant                = allow_authorization_code_grant,
                AllowClientCredentialsGrant                = allow_client_credentials_grant,
                AllowResourceOwnerPasswordCredentialsGrant = allow_resource_owner_password_credentials_grant,
                AllowImplicitGrant                         = allow_implicit_grant,
                GenerateRefreshTokenForAuthorizationCode   = generate_refresh_token_for_authorization_code,
                GenerateRefreshTokenForClientCredentials   = generate_refresh_token_for_client_credentials,
                GenerateRefreshTokenForPasswordCredentials = generate_refresh_token_for_password_credentials,
                GenerateRefreshTokenForImplicitFlow        = generate_refresh_token_for_implicit_flow,
                UserId                                     = user_id.GetValueOrDefault(),
            };
    }
}
