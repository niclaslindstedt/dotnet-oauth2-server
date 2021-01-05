using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class Application
    {
        public int      ApplicationId                              { get; set; }
        public string   Name                                       { get; set; }
        public string   Description                                { get; set; }
        public string   Type                                       { get; set; }
        public Guid     ClientId                                   { get; set; } = Guid.NewGuid();
        public string   ClientSecret                               { get; set; }
        public string   LogoBase64                                 { get; set; }
        public string   HomepageUri                                { get; set; }
        public string   RedirectUri                                { get; set; }
        public int      FailedLoginsBeforeLocked                   { get; set; }
        public int      FailedLoginsLockLifetimeMinutes            { get; set; }
        public int      AuthorizationCodeLifetimeSeconds           { get; set; }
        public int      AccessTokenLifetimeMinutes                 { get; set; }
        public int      RefreshTokenLifetimeDays                   { get; set; }
        public bool     AllowCredentialsInBody                     { get; set; }
        public bool     AllowCustomQueryParametersInRedirectUri    { get; set; }
        public bool     AllowAuthorizationCodeGrant                { get; set; }
        public bool     AllowClientCredentialsGrant                { get; set; }
        public bool     AllowResourceOwnerPasswordCredentialsGrant { get; set; }
        public bool     AllowImplicitGrant                         { get; set; }
        public bool     GenerateRefreshTokenForAuthorizationCode   { get; set; }
        public bool     GenerateRefreshTokenForClientCredentials   { get; set; }
        public bool     GenerateRefreshTokenForPasswordCredentials { get; set; }
        public bool     GenerateRefreshTokenForImplicitFlow        { get; set; }
        public Guid     UserId                                     { get; set; }
        public DateTime CreatedDateTime                            { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDateTime                           { get; set; } = DateTime.UtcNow;

        public virtual User               User   { get; set; }
        public virtual ICollection<Scope> Scopes { get; set; }
        public virtual ICollection<Role>  Roles  { get; set; }

        public void Update(Application applicationWithUpdates)
        {
            Name                                       = applicationWithUpdates.Name;
            Description                                = applicationWithUpdates.Description;
            Type                                       = applicationWithUpdates.Type;
            LogoBase64                                 = applicationWithUpdates.LogoBase64;
            HomepageUri                                = applicationWithUpdates.HomepageUri;
            RedirectUri                                = applicationWithUpdates.RedirectUri;
            FailedLoginsBeforeLocked                   = applicationWithUpdates.FailedLoginsBeforeLocked;
            FailedLoginsLockLifetimeMinutes            = applicationWithUpdates.FailedLoginsLockLifetimeMinutes;
            AuthorizationCodeLifetimeSeconds           = applicationWithUpdates.AuthorizationCodeLifetimeSeconds;
            AccessTokenLifetimeMinutes                 = applicationWithUpdates.AccessTokenLifetimeMinutes;
            RefreshTokenLifetimeDays                   = applicationWithUpdates.RefreshTokenLifetimeDays;
            AllowCredentialsInBody                     = applicationWithUpdates.AllowCredentialsInBody;
            AllowCustomQueryParametersInRedirectUri    = applicationWithUpdates.AllowCustomQueryParametersInRedirectUri;
            AllowAuthorizationCodeGrant                = applicationWithUpdates.AllowAuthorizationCodeGrant;
            AllowClientCredentialsGrant                = applicationWithUpdates.AllowClientCredentialsGrant;
            AllowResourceOwnerPasswordCredentialsGrant = applicationWithUpdates.AllowResourceOwnerPasswordCredentialsGrant;
            AllowImplicitGrant                         = applicationWithUpdates.AllowImplicitGrant;
            GenerateRefreshTokenForAuthorizationCode   = applicationWithUpdates.GenerateRefreshTokenForAuthorizationCode;
            GenerateRefreshTokenForClientCredentials   = applicationWithUpdates.GenerateRefreshTokenForClientCredentials;
            GenerateRefreshTokenForPasswordCredentials = applicationWithUpdates.GenerateRefreshTokenForPasswordCredentials;
            GenerateRefreshTokenForImplicitFlow        = applicationWithUpdates.GenerateRefreshTokenForImplicitFlow;
            ModifiedDateTime                           = DateTime.UtcNow;
        }
    }
}
