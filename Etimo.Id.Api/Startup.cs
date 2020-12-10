using Etimo.Id.Abstractions;
using Etimo.Id.Api.Bootstrapping;
using Etimo.Id.Api.Errors;
using Etimo.Id.Api.Middleware;
using Etimo.Id.Api.Security;
using Etimo.Id.Api.Settings;
using Etimo.Id.Data;
using Etimo.Id.Data.Repositories;
using Etimo.Id.Service;
using Etimo.Id.Service.Scopes;
using Etimo.Id.Service.Settings;
using Etimo.Id.Service.TokenGenerators;
using Etimo.Id.Service.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Etimo.Id.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "etimo-id", Version = "v1" });
            });

            services.UseEtimoIdData();

            var siteSettings = new SiteSettings();
            Configuration.GetSection("SiteSettings").Bind(siteSettings);
            services.AddSingleton(siteSettings);

            var cryptologySettings = new CryptologySettings();
            Configuration.GetSection("CryptologySettings").Bind(cryptologySettings);
            services.AddSingleton(cryptologySettings);
            services.AddSingleton(cryptologySettings.PasswordSettings);

            var oauth2Settings = new OAuth2Settings();
            Configuration.GetSection("OAuth2Settings").Bind(oauth2Settings);
            services.AddSingleton(oauth2Settings);

            var jwtSettings = new JwtSettings();
            Configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(config =>
            {
                AddScopePolicies(config, InbuiltScopes.All);
                AddCombinedScopePolicies(config, new Dictionary<string, string[]>
                {
                    { CombinedScopes.ReadApplicationRole, new string[] { ApplicationScopes.Read, RoleScopes.Read } }
                });
            });

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            services.AddSingleton(Log.Logger);

            var passwordHasher = new BCryptPasswordHasher(cryptologySettings);
            services.AddSingleton<IPasswordHasher>(passwordHasher);
            services.AddTransient<IPasswordGenerator, PasswordGeneratorAdapter>();

            // ApplicationServices
            services.AddTransient<IAddApplicationService, AddApplicationService>();
            services.AddTransient<IAuthenticateClientService, AuthenticateClientService>();
            services.AddTransient<IDeleteApplicationService, DeleteApplicationService>();
            services.AddTransient<IFindApplicationService, FindApplicationService>();
            services.AddTransient<IGenerateClientSecretService, GenerateClientSecretService>();
            services.AddTransient<IGetApplicationsService, GetApplicationsService>();
            services.AddTransient<IUpdateApplicationService, UpdateApplicationService>();

            // AuthorizationServices
            services.AddTransient<IValidateTokenService, ValidateTokenService>();
            services.AddTransient<IAuthorizeService, AuthorizeService>();
            services.AddTransient<IGenerateTokenService, GenerateTokenService>();

            // RoleServices
            services.AddTransient<IAddRoleScopeRelationService, AddRoleScopeRelationService>();
            services.AddTransient<IAddRoleService, AddRoleService>();
            services.AddTransient<IDeleteRoleScopeRelationService, DeleteRoleScopeRelationService>();
            services.AddTransient<IDeleteRoleService, DeleteRoleService>();
            services.AddTransient<IFindRoleService, FindRoleService>();
            services.AddTransient<IGetRolesService, GetRolesService>();
            services.AddTransient<IUpdateRoleService, UpdateRoleService>();

            // ScopeServices
            services.AddTransient<IAddScopeService, AddScopeService>();
            services.AddTransient<IDeleteScopeService, DeleteScopeService>();
            services.AddTransient<IFindScopeService, FindScopeService>();
            services.AddTransient<IGetScopesService, GetScopesService>();
            services.AddTransient<IUpdateScopeService, UpdateScopeService>();

            // UserServices
            services.AddTransient<IAddUserService, AddUserService>();
            services.AddTransient<IAuthenticateUserService, AuthenticateUserService>();
            services.AddTransient<IDeleteUserService, DeleteUserService>();
            services.AddTransient<IFindUserService, FindUserService>();
            services.AddTransient<IGetUsersService, GetUsersService>();
            services.AddTransient<IUpdateUserService, UpdateUserService>();

            // Token Generators
            services.AddTransient<IAuthorizationCodeTokenGenerator, AuthorizationCodeTokenGenerator>();
            services.AddTransient<IClientCredentialsTokenGenerator, ClientCredentialsTokenGenerator>();
            services.AddTransient<IResourceOwnerCredentialsTokenGenerator, ResourceOwnerCredentialsTokenGenerator>();
            services.AddTransient<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddTransient<IJwtTokenFactory, JwtTokenFactory>();

            // Repositories
            services.AddTransient<IApplicationRepository, ApplicationRepository>();
            services.AddTransient<IAccessTokenRepository, AccessTokenRepository>();
            services.AddTransient<IAuthorizationCodeRepository, AuthorizationCodeRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IScopeRepository, ScopeRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddDistributedMemoryCache();
            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseErrorMiddleware();

            if (Environment.IsDevelopment())
            {
                ApplyDevelopmentSettings(app);
            }
            else
            {
                ApplyProductionSettings(app);
            }

            ApplyCommonSettings(app);
        }

        private static void ApplyDevelopmentSettings(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "etimo_id v1"));
        }

        private static void ApplyProductionSettings(IApplicationBuilder app)
        {
            app.UseHsts();
        }

        private static void ApplyCommonSettings(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseBruteForceProtection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void AddScopePolicies(AuthorizationOptions config, List<string> policies)
        {
            foreach (var policy in policies)
            {
                config.AddPolicy(policy, Policies.ScopePolicy(policy));
            }
        }

        private static void AddCombinedScopePolicies(AuthorizationOptions config, Dictionary<string, string[]> policies)
        {
            foreach (var policy in policies)
            {
                config.AddPolicy(policy.Key, Policies.ScopePolicy(policy.Value));
            }
        }
    }
}
