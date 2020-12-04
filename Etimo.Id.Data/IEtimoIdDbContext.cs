using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Etimo.Id.Data
{
    public interface IEtimoIdDbContext
    {
        DbSet<AccessToken> AccessTokens { get; }
        DbSet<Application> Applications { get; }
        DbSet<AuthorizationCode> AuthorizationCodes { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<Scope> Scopes { get; }
        DbSet<User> Users { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
