using System.Linq;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Etimo.Id.Api;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Threading.Tasks;
using System.Collections.Generic;
using Etimo.Id.Security;
using Moq;
using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.TokenGenerators;
using Etimo.Id.Settings;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using System.Net;

namespace Etimo.Id.Tests.TestHelpers
{
    public class TestHelper
    {
        private static readonly RSA Rsa = RSA.Create();
        private static readonly string PublicKey = Convert.ToBase64String(Rsa.ExportRSAPublicKey());
        private static readonly string PrivateKey = Convert.ToBase64String(Rsa.ExportRSAPrivateKey());
        private const string FakeIssuer = "FakeIssuer";
        private static  readonly string FakeSecret = Guid.NewGuid().ToString();
        public static TestServer CreateTestServer(Action<IServiceCollection> overrideServices = null)
        {
            SetupFakedEnvironmentVariables();
            SetupDummyLogger();

            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

            return new TestServer(
               new WebHostBuilder()
                    .UseSerilog()
                    .UseConfiguration(configuration)
                    .UseEnvironment("Testing")
                    .ConfigureTestServices(services =>
                    {
                        if (overrideServices != null) overrideServices(services);

                    }).UseStartup<TestStartup>());
        }
        public static Task<string> GetAdminJwt => CreateFakeJwt(InbuiltScopes.Admin, InbuiltScopes.Admin);
        public static Task<string> GetReadJwt => CreateFakeJwt(InbuiltScopes.Read, InbuiltScopes.Read);
        public static Task<string> GetWriteJwt => CreateFakeJwt(InbuiltScopes.Write, InbuiltScopes.Write);
        public static async Task<string> CreateFakeJwt(IList<string> roles, IList<string> scopes)
        {
            var jwtTokenFactory = new JwtTokenFactory(CreateMockedRoleService(roles, scopes), new JwtSettings
            {
                Issuer = FakeIssuer,
                Secret = FakeSecret,
                PublicKey = PublicKey,
                PrivateKey = PrivateKey,
            });

            var token = await jwtTokenFactory.CreateJwtTokenAsync(new JwtTokenRequest
            {
                Username = "DummyUser",
                Subject = Guid.NewGuid().ToString(),
                LifetimeMinutes = 1337,
                ClientId = Guid.NewGuid(),
                Audience = new List<string>(),
                Scope = string.Join(" ", scopes)
            });

            return token.AccessToken;
        }

        private static void SetupFakedEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("EtimoIdSettings:Secret", FakeSecret);
            Environment.SetEnvironmentVariable("EtimoIdSettings:Issuer", FakeIssuer);
            Environment.SetEnvironmentVariable("EtimoIdSettings:PublicKey", PublicKey);
            Environment.SetEnvironmentVariable("EtimoIdSettings:PrivateKey", PrivateKey);
            Environment.SetEnvironmentVariable("ConnectionStrings:EtimoId", "DummyConnectionString");
        }

        private static void SetupDummyLogger()
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateLogger();
        }
        private static IGetRolesService CreateMockedRoleService(IList<string> roles, IList<string> scopes)
        {
            var rolesMock = new Mock<IGetRolesService>();

            var dbRoles = roles.Select(role => new Entities.Role
            {
                Name = role,
                Scopes = new List<Scope>()
            });

            dbRoles.First().Scopes = scopes.Select(scope => new Scope
            {
                Name = scope,
            }).ToList();

            rolesMock
                .Setup(m => m
                .GetByUserIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(dbRoles.ToList()));

            return rolesMock.Object;
        }

        private class TestStartup : Startup
        {
            public TestStartup(IConfiguration configuration, IWebHostEnvironment environment) : base(configuration, environment)
            {
            }
            public override void Configure(IApplicationBuilder app)
            {
                app.Use(async (context, next) =>
                {
                    // Middleware for setting up a dummy IP for the test requests.
                    context.Connection.RemoteIpAddress = IPAddress.Parse("66.137.93.66");
                    await next.Invoke();
                });
                base.Configure(app);

            }
        }
    }
}
