using Etimo.Id.Data;
using Etimo.Id.Settings;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Etimo.Id.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();

            var siteSettings = new SiteSettings();
            config.GetSection("SiteSettings").Bind(siteSettings);

            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateLogger();

            try
            {
                Log.Information("Starting up");

                IWebHost host = WebHost.CreateDefaultBuilder(args)
                    .UseSerilog()
                    .UseKestrel(
                        opt =>

                        {
                            opt.AddServerHeader = false;
                            opt.ConfigureHttpsDefaults(o => { o.SslProtocols = GetProtocols(siteSettings); });
                        })
                    .UseStartup<Startup>()
                    .UseUrls(GetUrls(siteSettings))
                    .Build();

                SeedDatabaseAsync(host).Wait();

                host.Run();
            }
            catch (Exception ex) { Log.Fatal(ex, "Application start-up failed"); }
            finally { Log.CloseAndFlush(); }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        private static SslProtocols GetProtocols(SiteSettings settings)
        {
            SslProtocols tlsVersions = SslProtocols.None;

            if (settings.TlsVersions.Contains("1.0")) { tlsVersions |= SslProtocols.Tls; }

            if (settings.TlsVersions.Contains("1.1")) { tlsVersions |= SslProtocols.Tls11; }

            if (settings.TlsVersions.Contains("1.2")) { tlsVersions |= SslProtocols.Tls12; }

            if (settings.TlsVersions.Contains("1.3")) { tlsVersions |= SslProtocols.Tls13; }

            return tlsVersions;
        }

        private static string GetUrls(SiteSettings siteSettings)
        {
            string urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            if (!string.IsNullOrEmpty(urls)) { return urls; }

            return siteSettings.ListenUri;
        }

        private static async Task SeedDatabaseAsync(IWebHost host)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                try
                {
                    IEtimoIdDbContext context = services.GetRequiredService<IEtimoIdDbContext>();

                    await Seeder.SeedAsync(context, services);
                }
                catch (Exception) { Log.Error("An error occurred while seeding the database"); }
            }
        }
    }
}
