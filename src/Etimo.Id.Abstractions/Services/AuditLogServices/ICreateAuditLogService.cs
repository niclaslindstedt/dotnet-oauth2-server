using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface ICreateAuditLogService
    {
        Task CreateFailedLoginAuditLogAsync(User user, Application application);
        Task CreateAuthorizationCodeAbuseAuditLogAsync(AuthorizationCode code);
        Task CreateRefreshTokenAbuseAuditLogAsync(RefreshToken refreshToken);
        Task CreateForbiddenGrantTypeAuditLogAsync(string grantType);
        Task CreateUnlockedAuditLogAsync(User user);
    }
}
