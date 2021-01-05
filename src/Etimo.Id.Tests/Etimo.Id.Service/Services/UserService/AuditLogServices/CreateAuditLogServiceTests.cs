using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service;
using Etimo.Id.Service.Constants;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Etimo.Id.Tests
{
    public class CreateAuditLogServiceTests
    {
        private readonly Mock<IAuditLogRepository> _auditLogRepository = new();

        [Fact]
        public async Task CreateFailedLoginAuditLogAsync_ShouldAddAuditLogToRepository()
        {
            // Arrange
            User user = new();
            Application application = new()
            {
                ClientId                        = Guid.NewGuid(),
                FailedLoginsBeforeLocked        = 5,
                FailedLoginsLockLifetimeMinutes = 50,
            };
            RequestContext requestContext = new()
            {
                ClientId  = application.ClientId,
                IpAddress = "127.1.1.2",
            };
            CreateAuditLogService service = CreateAuditLogService(application, requestContext);

            // Act
            await service.CreateFailedLoginAuditLogAsync(user);

            // Assert
            _auditLogRepository.Verify(
                r => r.Add(
                    It.Is<AuditLog>(
                        a => a.Type == AuditLogTypes.FailedLogin
                         && a.UserId == user.UserId
                         && a.ApplicationId == application.ApplicationId)),
                Times.Once);
        }

        [Fact]
        public async Task CreateAuthorizationCodeAbuseAuditLogAsync_ShouldAddAuditLogToRepository()
        {
            // Arrange
            AuthorizationCode code = new()
            {
                UserId = Guid.NewGuid(),
            };
            Application application = new()
            {
                ClientId                        = Guid.NewGuid(),
                FailedLoginsBeforeLocked        = 5,
                FailedLoginsLockLifetimeMinutes = 50,
            };
            RequestContext requestContext = new()
            {
                ClientId  = application.ClientId,
                IpAddress = "127.1.1.2",
            };
            CreateAuditLogService service = CreateAuditLogService(application, requestContext);

            // Act
            await service.CreateAuthorizationCodeAbuseAuditLogAsync(code);

            // Assert
            _auditLogRepository.Verify(
                r => r.Add(
                    It.Is<AuditLog>(
                        a => a.Type == AuditLogTypes.CodeReuse && a.UserId == code.UserId && a.ApplicationId == application.ApplicationId)),
                Times.Once);
        }

        [Fact]
        public async Task CreateRefreshTokenAbuseAuditLogAsync_ShouldAddAuditLogToRepository()
        {
            // Arrange
            RefreshToken refreshToken = new()
            {
                UserId = Guid.NewGuid(),
            };
            Application application = new()
            {
                ClientId                        = Guid.NewGuid(),
                FailedLoginsBeforeLocked        = 5,
                FailedLoginsLockLifetimeMinutes = 50,
            };
            RequestContext requestContext = new()
            {
                ClientId  = application.ClientId,
                IpAddress = "127.1.1.2",
            };
            CreateAuditLogService service = CreateAuditLogService(application, requestContext);

            // Act
            await service.CreateRefreshTokenAbuseAuditLogAsync(refreshToken);

            // Assert
            _auditLogRepository.Verify(
                r => r.Add(
                    It.Is<AuditLog>(
                        a => a.Type == AuditLogTypes.RefreshTokenAbuse
                         && a.UserId == refreshToken.UserId
                         && a.ApplicationId == application.ApplicationId)),
                Times.Once);
        }

        private CreateAuditLogService CreateAuditLogService(Application application, RequestContext requestContext)
            => new(_auditLogRepository.Object, CreateApplicationRepository(application), requestContext);

        private IApplicationRepository CreateApplicationRepository(Application applicationToReturn)
        {
            var mock = new Mock<IApplicationRepository>();
            mock.Setup(m => m.FindByClientIdAsync(applicationToReturn.ClientId)).ReturnsAsync(applicationToReturn);
            return mock.Object;
        }
    }
}
