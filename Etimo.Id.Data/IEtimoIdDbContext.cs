using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace Etimo.Id.Data
{
    public interface IEtimoIdDbContext
    {
        DbSet<AccessToken> AccessTokens { get; }
        DbSet<Application> Applications { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<AuthorizationCode> AuthorizationCodes { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<Role> Roles { get; }
        DbSet<Scope> Scopes { get; }
        DbSet<User> Users { get; }
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
