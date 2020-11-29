using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;

namespace Etimo.Id.Data
{
    public class EtimoIdDbContext : DbContext
    {
        public EtimoIdDbContext(DbContextOptions<EtimoIdDbContext> options) : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<AuthorizationCode> AuthorizationCodes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseLazyLoadingProxies();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var application = modelBuilder.Entity<Application>();
            application.HasKey(a => a.ApplicationId);
            application.HasIndex(a => a.ClientId).IsUnique();
            application.HasOne(a => a.User).WithMany(u => u.Applications).HasForeignKey(a => a.UserId);

            var authorizationCode = modelBuilder.Entity<AuthorizationCode>();
            authorizationCode.HasKey(a => a.Code);

            var refreshToken = modelBuilder.Entity<RefreshToken>();
            refreshToken.HasKey(rt => rt.RefreshTokenId);
            refreshToken.HasOne(u => u.User).WithMany(u => u.RefreshTokens).HasForeignKey(rt => rt.UserId);
            refreshToken.HasOne(u => u.Application).WithMany(a => a.RefreshTokens).HasForeignKey(rt => rt.ApplicationId);

            var user = modelBuilder.Entity<User>();
            user.HasKey(u => u.UserId);
            user.HasIndex(u => u.Username).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
