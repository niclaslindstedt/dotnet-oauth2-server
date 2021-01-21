using Etimo.Id.Constants;
using Etimo.Id.Security;
using Etimo.Id.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Etimo.Id.Authentication
{
    [ExcludeFromCodeCoverage]
    public static class EtimoIdAuthentication
    {
        public static void UseEtimoId(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            var etimoIdSettings = new EtimoIdSettings();
            configuration.GetSection("EtimoIdSettings").Bind(etimoIdSettings);
            services.AddSingleton(etimoIdSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken            = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateActor = true,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = etimoIdSettings.Issuer,
                            ValidAudience = etimoIdSettings.Issuer,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(etimoIdSettings.Secret)),
                            ClockSkew = TimeSpan.Zero,
                            NameClaimType = CustomClaimTypes.Name,
                            RoleClaimType = CustomClaimTypes.Role,
                            RequireAudience = true,
                            RequireExpirationTime = true,
                            RequireSignedTokens = true,
                            IgnoreTrailingSlashWhenValidatingAudience = true,
                        };
                    });

            services.AddAuthorization(
                config =>
                {
                    AddScopePolicies(config, InbuiltScopes.All);
                    AddCombinedScopePolicies(
                        config,
                        new Dictionary<string, string[]>
                        {
                            { CombinedScopes.ReadApplicationRole, new[] { ApplicationScopes.Read, RoleScopes.Read } },
                            { CombinedScopes.ReadRoleScope, new[] { RoleScopes.Read, ScopeScopes.Read } },
                            { CombinedScopes.ReadUserApplication, new[] { UserScopes.Read, ApplicationScopes.Read } },
                            { CombinedScopes.ReadUserRole, new[] { UserScopes.Read, RoleScopes.Read } },
                        });
                });
        }

        private static void AddScopePolicies(AuthorizationOptions config, List<string> policies)
        {
            foreach (string policy in policies) { config.AddPolicy(policy, Policies.ScopePolicy(policy)); }
        }

        private static void AddCombinedScopePolicies(AuthorizationOptions config, Dictionary<string, string[]> policies)
        {
            foreach (KeyValuePair<string, string[]> policy in policies)
            {
                config.AddPolicy(policy.Key, Policies.ScopePolicy(policy.Value));
            }
        }
    }
}
