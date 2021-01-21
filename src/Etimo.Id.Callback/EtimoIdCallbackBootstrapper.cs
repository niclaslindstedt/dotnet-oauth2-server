using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Etimo.Id.Callback
{
    [ExcludeFromCodeCoverage]
    public static class EtimoIdCallback
    {
        public static void UseEtimoIdCallback(this IServiceCollection services)
        {
            var assembly = Assembly.GetAssembly(typeof(OAuthController));
            services.AddControllers().AddApplicationPart(assembly).AddControllersAsServices();
        }
    }
}
