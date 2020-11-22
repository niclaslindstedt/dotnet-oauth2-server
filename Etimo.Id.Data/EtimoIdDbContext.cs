using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;

namespace Etimo.Id.Data
{
    public class EtimoIdDbContext : DbContext
    {
        public EtimoIdDbContext(DbContextOptions<EtimoIdDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(user => user.Username).IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<User> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
