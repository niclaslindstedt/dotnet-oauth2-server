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
using Microsoft.EntityFrameworkCore;
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "etimo-id", Version = "v1" });
            });

            var connectionString = Configuration.GetConnectionString("EtimoId");
            services.AddDbContext<EtimoIdDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            var siteSettings = new SiteSettings();
            Configuration.GetSection("SiteSettings").Bind(siteSettings);
            services.AddSingleton(siteSettings);

            var oauthSettings = new OAuthSettings();
            Configuration.GetSection("OAuthSettings").Bind(oauthSettings);
            services.AddSingleton(oauthSettings);

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
                        ValidIssuer = oauthSettings.Issuer,
                        ValidAudience = oauthSettings.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(oauthSettings.Secret)),
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

            var passwordHasher = new BCryptPasswordHasher();
            services.AddSingleton<IPasswordHasher>(passwordHasher);
            services.AddTransient<IPasswordGenerator, PasswordGeneratorAdapter>();

            services.AddTransient<IAuthorizationCodeTokenGenerator, AuthorizationCodeTokenGenerator>();
            services.AddTransient<IClientCredentialsTokenGenerator, ClientCredentialsTokenGenerator>();
            services.AddTransient<IResourceOwnerCredentialsTokenGenerator, ResourceOwnerCredentialsTokenGenerator>();
            services.AddTransient<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddTransient<IJwtTokenFactory, JwtTokenFactory>();

            services.AddTransient<IApplicationsService, ApplicationsService>();
            services.AddTransient<IApplicationsRepository, ApplicationsRepository>();
            services.AddTransient<IAuthorizationCodeRepository, AuthorizationCodeRepository>();
            services.AddTransient<IOAuthService, OAuthService>();
            services.AddTransient<IRefreshTokensRepository, RefreshTokensRepository>();
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/error");

            if (env.IsDevelopment())
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
