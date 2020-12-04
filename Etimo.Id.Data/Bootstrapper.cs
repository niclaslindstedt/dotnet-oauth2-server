using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Etimo.Id.Data
{
    public static class Bootstrapper
    {
        public static void UseEtimoIdData(this IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var config = sp.GetService<IConfiguration>();
            var env = sp.GetService<IHostEnvironment>();

            var connectionString = config.GetConnectionString("EtimoId");
            services.AddDbContext<IEtimoIdDbContext, EtimoIdDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.EnableSensitiveDataLogging(env.IsDevelopment());
            });
        }
    }
}
