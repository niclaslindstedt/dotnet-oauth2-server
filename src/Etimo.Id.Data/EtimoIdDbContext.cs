using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;

namespace Etimo.Id.Data
{
    public class EtimoIdDbContext : DbContext, IEtimoIdDbContext
    {
        public EtimoIdDbContext(DbContextOptions<EtimoIdDbContext> options) : base(options)
        {
        }

        public DbSet<AccessToken> AccessTokens { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AuthorizationCode> AuthorizationCodes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Scope> Scopes { get; set; }
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
            application.HasMany(a => a.Scopes);

            var accessToken = modelBuilder.Entity<AccessToken>();
            accessToken.HasKey(at => at.AccessTokenId);

            var auditLog = modelBuilder.Entity<AuditLog>();
            auditLog.HasKey(al => al.AuditLogId);
            auditLog.HasOne(al => al.User);
            auditLog.HasOne(al => al.Application);

            var authorizationCode = modelBuilder.Entity<AuthorizationCode>();
            authorizationCode.HasKey(ac => ac.Code);
            authorizationCode.HasOne(ac => ac.AccessToken);

            var refreshToken = modelBuilder.Entity<RefreshToken>();
            refreshToken.HasKey(rt => rt.RefreshTokenId);
            refreshToken.HasOne(rt => rt.AuthorizationCode).WithMany().HasForeignKey(rt => rt.Code);
            refreshToken.HasOne(rt => rt.AccessToken);
            refreshToken.HasOne(rt => rt.Application);

            var role = modelBuilder.Entity<Role>();
            role.HasKey(r => r.RoleId);
            role.HasIndex(a => a.Name);
            role.HasOne(r => r.Application).WithMany(a => a.Roles).HasForeignKey(r => r.ApplicationId);
            role.HasMany(r => r.Scopes).WithMany(s => s.Roles);

            var scope = modelBuilder.Entity<Scope>();
            scope.HasKey(s => s.ScopeId);
            scope.HasIndex(a => a.Name);
            scope.HasOne(s => s.Application).WithMany(u => u.Scopes).HasForeignKey(s => s.ApplicationId);

            var user = modelBuilder.Entity<User>();
            user.HasKey(u => u.UserId);
            user.HasIndex(u => u.Username).IsUnique();
            user.HasMany(u => u.Applications).WithOne(a => a.User);
            user.HasMany(u => u.Roles).WithMany(r => r.Users);

            base.OnModelCreating(modelBuilder);
        }
    }
}
