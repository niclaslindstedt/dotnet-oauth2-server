using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;

namespace Etimo.Id.Data
{
    public class EtimoIdDbContext : DbContext
    {
        public EtimoIdDbContext(DbContextOptions<EtimoIdDbContext> options) : base(options)
        {
        }

        public DbSet<User> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
