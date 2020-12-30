using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Data
{
    public class Seeder
    {
        public static async Task SeedAsync(IEtimoIdDbContext context, IServiceProvider services)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any()) { return; }

            var passwordHasher = services.GetService(typeof(IPasswordHasher)) as IPasswordHasher;

            Console.WriteLine("Start seeding the database.");

            var adminUserId = new Guid("b7b229a3-c84d-495f-b4ac-da3a4fd0757f");
            var adminUser = new User
            {
                UserId   = adminUserId,
                Username = "admin",
                Password = passwordHasher.Hash("etimo"),
            };
            context.Users.Add(adminUser);

            var adminClientId = new Guid("11111111-1111-1111-1111-111111111111");
            context.Applications.Add(
                new Application
                {
                    ApplicationId                    = 1,
                    Name                             = "etimo-default",
                    Description                      = "Automatically generated.",
                    LogoBase64                       = "/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAMCAgICAgMCAgID",
                    HomepageUri                      = "https://localhost:5010",
                    RedirectUri                      = "https://localhost:5010/oauth2/callback",
                    AuthorizationCodeLifetimeSeconds = 15,
                    AccessTokenLifetimeMinutes       = 15,
                    RefreshTokenLifetimeDays         = 30,
                    ClientId                         = adminClientId,
                    ClientSecret                     = passwordHasher.Hash("etimo"),
                    UserId                           = adminUserId,
                    Type                             = "confidential",
                });

            var adminRole = new Role
            {
                Name          = "admin",
                Description   = "Automatically generated.",
                ApplicationId = 1,
                Users         = new List<User> { adminUser },
            };
            context.Roles.Add(adminRole);

            var scopes = GetBuiltInScopes("read").Concat(GetBuiltInScopes("write")).Concat(GetBuiltInScopes("admin")).ToList();
            context.Scopes.AddRange(scopes);

            adminRole.Scopes = scopes;

            await context.SaveChangesAsync();

            Console.WriteLine("Finished seeding the database.");
        }

        private static List<Scope> GetBuiltInScopes(string type)
        {
            var scopes = new List<Scope>();

            var resources = new List<string>
            {
                "application", "auditlog", "scope", "role", "user",
            };

            foreach (string resource in resources)
            {
                var scope = new Scope
                {
                    ApplicationId = 1,
                    Name          = $"{type}:{resource}",
                    Description   = "Built-in scope.",
                };
                scopes.Add(scope);
            }

            return scopes;
        }
    }
}
