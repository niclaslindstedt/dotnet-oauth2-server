using Etimo.Id.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Etimo.Id.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddTransient<IEtimoIdApplicationClient, EtimoIdApplicationClient>();
            builder.Services.AddTransient<IEtimoIdAuditLogClient, EtimoIdAuditLogClient>();
            builder.Services.AddTransient<IEtimoIdOAuthClient, EtimoIdOAuthClient>();
            builder.Services.AddTransient<IEtimoIdRoleClient, EtimoIdRoleClient>();
            builder.Services.AddTransient<IEtimoIdScopeClient, EtimoIdScopeClient>();
            builder.Services.AddTransient<IEtimoIdUserClient, EtimoIdUserClient>();

            builder.Services.AddOidcAuthentication(
                options =>
                {
                    builder.Configuration.Bind("OidcProviderOptions", options.ProviderOptions);
                    options.ProviderOptions.ResponseType = "code";
                });

            await builder.Build().RunAsync();
        }
    }
}
