using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Constants;
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

        public async Task CreateFailedLoginAuditLogAsync(User user)
        {
            Application application = await _applicationRepository.FindByClientIdAsync(_requestContext.ClientId.Value);

            AuditLog auditLog = new()
            {
                Type = AuditLogTypes.FailedLogin,
                Message = $"Too many login attempts ({application.FailedLoginsBeforeLocked})."
                  + $"The account has been locked for {application.FailedLoginsLockLifetimeMinutes} minutes.",
                UserId        = user.UserId,
                ApplicationId = application.ApplicationId,
            };

            _auditLogRepository.Add(auditLog);
            await _auditLogRepository.SaveAsync();
        }

        public async Task CreateAuthorizationCodeAbuseAuditLogAsync(AuthorizationCode code)
        {
            Application application = await _applicationRepository.FindByClientIdAsync(_requestContext.ClientId.Value);

            AuditLog auditLog = new()
            {
                Type          = AuditLogTypes.CodeReuse,
                Message       = $"Authorization code reuse detected from IP address {_requestContext.IpAddress}.",
                UserId        = code.UserId.Value,
                ApplicationId = application.ApplicationId,
            };

            _auditLogRepository.Add(auditLog);

            await _auditLogRepository.SaveAsync();
        }

        public async Task CreateRefreshTokenAbuseAuditLogAsync(RefreshToken refreshToken)
        {
            Application application = await _applicationRepository.FindByClientIdAsync(_requestContext.ClientId.Value);

            AuditLog auditLog = new()
            {
                Type          = AuditLogTypes.RefreshTokenAbuse,
                Message       = $"Refresh token abuse detected from IP address {_requestContext.IpAddress}.",
                UserId        = refreshToken.UserId,
                ApplicationId = application.ApplicationId,
            };

            _auditLogRepository.Add(auditLog);

            await _auditLogRepository.SaveAsync();
        }
    }
}
