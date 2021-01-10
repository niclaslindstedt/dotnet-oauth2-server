using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class UnlockUserService : IUnlockUserService
    {
        private readonly ICreateAuditLogService _createAuditLogService;
        private readonly IRequestContext        _requestContext;
        private readonly IUserRepository        _userRepository;

        public UnlockUserService(
            IUserRepository userRepository,
            ICreateAuditLogService createAuditLogService,
            IRequestContext requestContext)
        {
            _userRepository        = userRepository;
            _createAuditLogService = createAuditLogService;
            _requestContext        = requestContext;
        }

        public async Task UnlockAsync(Guid userId)
        {
            User user = await _userRepository.FindAsync(userId);
            if (user == null) { throw new NotFoundException(); }

            user.LockedUntilDateTime = null;
            user.FailedLogins        = 0;

            await _createAuditLogService.CreateUnlockedAuditLogAsync(user);

            await _userRepository.SaveAsync();
        }
    }
}
