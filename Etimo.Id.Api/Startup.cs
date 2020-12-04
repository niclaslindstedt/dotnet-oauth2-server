using Etimo.Id.Abstractions;
using Etimo.Id.Api.Bootstrapping;
using Etimo.Id.Api.Security;
using Etimo.Id.Api.Settings;
using Etimo.Id.Data;
using Etimo.Id.Data.Repositories;
using Etimo.Id.Service;
using Etimo.Id.Service.Settings;
using Etimo.Id.Service.TokenGenerators;
using Etimo.Id.Service.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
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
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.User, Policies.UserPolicy());
            });

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            services.AddSingleton(Log.Logger);

            var passwordHasher = new BCryptPasswordHasher(cryptologySettings);
            services.AddSingleton<IPasswordHasher>(passwordHasher);
            services.AddTransient<IPasswordGenerator, PasswordGeneratorAdapter>();

            services.AddTransient<IAuthorizationCodeTokenGenerator, AuthorizationCodeTokenGenerator>();
            services.AddTransient<IClientCredentialsTokenGenerator, ClientCredentialsTokenGenerator>();
            services.AddTransient<IResourceOwnerCredentialsTokenGenerator, ResourceOwnerCredentialsTokenGenerator>();
            services.AddTransient<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddTransient<IJwtTokenFactory, JwtTokenFactory>();

            services.AddTransient<IApplicationsService, ApplicationsService>();
            services.AddTransient<IApplicationsRepository, ApplicationsRepository>();
            services.AddTransient<IAccessTokensRepository, AccessTokensRepository>();
            services.AddTransient<IAuthorizationCodeRepository, AuthorizationCodeRepository>();
            services.AddTransient<IOAuthService, OAuthService>();
            services.AddTransient<IRefreshTokensRepository, RefreshTokensRepository>();
            services.AddTransient<IScopesService, ScopesService>();
            services.AddTransient<IScopesRepository, ScopesRepository>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IUsersRepository, UsersRepository>();

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
            app.UseExceptionHandler("/error");

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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
