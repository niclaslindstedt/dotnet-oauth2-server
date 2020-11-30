using Etimo.Id.Api.Settings;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Security.Authentication;

namespace Etimo.Id.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var siteSettings = new SiteSettings();
            config.GetSection("SiteSettings").Bind(siteSettings);

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting up");

                var host = WebHost.CreateDefaultBuilder(args)
                    .UseSerilog()
                    .UseKestrel(opt =>

                    {
                        opt.AddServerHeader = false;
                        opt.ConfigureHttpsDefaults(s =>
                        {
                            s.SslProtocols = GetProtocols(siteSettings);
                        });
                    })
                    .UseStartup<Startup>()
                    .UseUrls(siteSettings.ListenUri)
                    .Build();

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static SslProtocols GetProtocols(SiteSettings settings)
        {
            var tlsVersions = SslProtocols.None;

            if (settings.TlsVersions.Contains("1.0"))
            {
                tlsVersions |= SslProtocols.Tls;
            }

            if (settings.TlsVersions.Contains("1.1"))
            {
                tlsVersions |= SslProtocols.Tls11;
            }

            if (settings.TlsVersions.Contains("1.2"))
            {
                tlsVersions |= SslProtocols.Tls12;
            }

            if (settings.TlsVersions.Contains("1.3"))
            {
                tlsVersions |= SslProtocols.Tls13;
            }

            return tlsVersions;
        }
    }
}
