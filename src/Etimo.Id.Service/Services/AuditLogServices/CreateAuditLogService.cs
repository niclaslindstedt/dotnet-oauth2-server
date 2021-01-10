using Etimo.Id.Abstractions;
using Etimo.Id.Constants;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    [ExcludeFromCodeCoverage]
    public class CreateAuditLogService : ICreateAuditLogService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IAuditLogRepository    _auditLogRepository;
        private readonly IRequestContext        _requestContext;

        public CreateAuditLogService(
            IAuditLogRepository auditLogRepository,
            IApplicationRepository applicationRepository,
            IRequestContext requestContext)
        {
            _auditLogRepository    = auditLogRepository;
            _applicationRepository = applicationRepository;
            _requestContext        = requestContext;
        }

        public Task CreateFailedLoginAuditLogAsync(User user, Application application)
            => CreateAuditLogAsync(
                user.UserId,
                AuditLogTypes.FailedLogin,
                $"Too many login attempts ({application.FailedLoginsBeforeLocked})."
              + $"The account has been locked for {application.FailedLoginsLockLifetimeMinutes} minutes.");

        public Task CreateAuthorizationCodeAbuseAuditLogAsync(AuthorizationCode code)
            => CreateAuditLogAsync(code.UserId, AuditLogTypes.CodeReuse, "Authorization code reuse detected.");

        public Task CreateRefreshTokenAbuseAuditLogAsync(RefreshToken refreshToken)
            => CreateAuditLogAsync(refreshToken.UserId, AuditLogTypes.RefreshTokenAbuse, "Refresh token abuse detected.");

        public Task CreateForbiddenGrantTypeAuditLogAsync(string grantType)
            => CreateAuditLogAsync(null, AuditLogTypes.ForbiddenGrantType, $"Attempted use of forbidden grant type ({grantType}).");

        public Task CreateUnlockedAuditLogAsync(User user)
            => CreateAuditLogAsync(user.UserId, AuditLogTypes.UnlockedUser, $"User has been unlocked by {_requestContext.Username}.");

        private async Task CreateAuditLogAsync(
            Guid? userId,
            string auditLogType,
            string message,
            string body = null)
        {
            Application application = await _applicationRepository.FindByClientIdAsync(_requestContext.ClientId.Value);

            AuditLog auditLog = new()
            {
                Type          = auditLogType,
                Message       = message,
                Body          = body,
                UserId        = userId,
                ApplicationId = application.ApplicationId,
                IpAddress     = _requestContext.IpAddress,
            };

            _auditLogRepository.Add(auditLog);

            await _auditLogRepository.SaveAsync();
        }
    }
}
