using Etimo.Id.Abstractions;
using Etimo.Id.Api.Bootstrapping;
using Etimo.Id.Api.Errors;
using Etimo.Id.Api.Middleware;
using Etimo.Id.Authentication;
using Etimo.Id.Data;
using Etimo.Id.Data.Repositories;
using Etimo.Id.Service;
using Etimo.Id.Service.TokenGenerators;
using Etimo.Id.Service.Utilities;
using Etimo.Id.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Etimo.Id.Api
{
    public class Startup
    {
        private const string CorsPolicyAllowAll = "cors-allow-all";

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment   = environment;
        }

        public IConfiguration      Configuration { get; }
        public IWebHostEnvironment Environment   { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "etimo-id", Version = "v1" }); });
            services.AddCors(options => { options.AddPolicy(CorsPolicyAllowAll, builder => { builder.WithOrigins("*"); }); });

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
            Configuration.GetSection("EtimoIdSettings").Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            services.UseEtimoId();

            services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });

            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateLogger();
            services.AddSingleton(Log.Logger);

            var passwordHasher = new BCryptPasswordHasher(cryptologySettings);
            services.AddSingleton<IPasswordHasher>(passwordHasher);
            services.AddTransient<IPasswordGenerator, PasswordGeneratorAdapter>();
            services.AddRequestContext();

            // ApplicationServices
            services.AddTransient<IAddApplicationService, AddApplicationService>();
            services.AddTransient<IAuthenticateClientService, AuthenticateClientService>();
            services.AddTransient<IDeleteApplicationService, DeleteApplicationService>();
            services.AddTransient<IFindApplicationService, FindApplicationService>();
            services.AddTransient<IGenerateClientSecretService, GenerateClientSecretService>();
            services.AddTransient<IGetApplicationsService, GetApplicationsService>();
            services.AddTransient<IUpdateApplicationService, UpdateApplicationService>();

            // AuditLogServices
            services.AddTransient<ICreateAuditLogService, CreateAuditLogService>();
            services.AddTransient<IFindAuditLogService, FindAuditLogService>();
            services.AddTransient<IGetAuditLogsService, GetAuditLogsService>();

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
            services.AddTransient<IVerifyScopeService, VerifyScopeService>();

            // UserServices
            services.AddTransient<IAddUserRoleRelationService, AddUserRoleRelationService>();
            services.AddTransient<IAddUserService, AddUserService>();
            services.AddTransient<IAuthenticateUserService, AuthenticateUserService>();
            services.AddTransient<IDeleteUserRoleRelationService, DeleteUserRoleRelationService>();
            services.AddTransient<IDeleteUserService, DeleteUserService>();
            services.AddTransient<IFindUserService, FindUserService>();
            services.AddTransient<IGetUsersService, GetUsersService>();
            services.AddTransient<ILockUserService, LockUserService>();
            services.AddTransient<IUnlockUserService, UnlockUserService>();
            services.AddTransient<IUpdateUserService, UpdateUserService>();

            // Token Generators
            services.AddTransient<IAuthorizationCodeTokenGenerator, AuthorizationCodeTokenGenerator>();
            services.AddTransient<IClientCredentialsTokenGenerator, ClientCredentialsTokenGenerator>();
            services.AddTransient<IImplicitTokenGenerator, ImplicitTokenGenerator>();
            services.AddTransient<IResourceOwnerCredentialsTokenGenerator, ResourceOwnerCredentialsTokenGenerator>();
            services.AddTransient<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddTransient<IJwtTokenFactory, JwtTokenFactory>();

            // Repositories
            services.AddTransient<IApplicationRepository, ApplicationRepository>();
            services.AddTransient<IAccessTokenRepository, AccessTokenRepository>();
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            services.AddTransient<IAuthorizationCodeRepository, AuthorizationCodeRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IScopeRepository, ScopeRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddDistributedMemoryCache();
            services.AddControllersWithViews()
                .AddJsonOptions(
                    options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance;
                        options.JsonSerializerOptions.IgnoreNullValues     = true;
                    })
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseCors(CorsPolicyAllowAll);
            app.UseErrorMiddleware();
            app.UseRateLimiter();

            if (Environment.IsDevelopment()) { ApplyDevelopmentSettings(app); }
            else { ApplyProductionSettings(app); }

            ApplyCommonSettings(app);
        }

        private static void ApplyDevelopmentSettings(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "etimo_id v1"));
        }

        private static void ApplyProductionSettings(IApplicationBuilder app)
            => app.UseHsts();

        private static void ApplyCommonSettings(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRequestContext();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
