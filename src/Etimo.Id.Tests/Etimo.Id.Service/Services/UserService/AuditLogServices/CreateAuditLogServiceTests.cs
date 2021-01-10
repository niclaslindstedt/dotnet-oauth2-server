using Etimo.Id.Abstractions;
using Etimo.Id.Constants;
using Etimo.Id.Entities;
using Etimo.Id.Service;
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
                ApplicationId                   = 57,
                ClientId                        = Guid.NewGuid(),
                FailedLoginsBeforeLocked        = 7,
                FailedLoginsLockLifetimeMinutes = 55,
            };
            RequestContext        requestContext = new() { ClientId = application.ClientId };
            CreateAuditLogService service        = CreateAuditLogService(application, requestContext);

            // Act
            await service.CreateFailedLoginAuditLogAsync(user, application);

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
            AuthorizationCode code = new() { UserId = Guid.NewGuid() };
            Application application = new()
            {
                ApplicationId = 57,
                ClientId      = Guid.NewGuid(),
            };
            RequestContext        requestContext = new() { ClientId = application.ClientId };
            CreateAuditLogService service        = CreateAuditLogService(application, requestContext);

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
            RefreshToken refreshToken = new() { UserId = Guid.NewGuid() };
            Application application = new()
            {
                ApplicationId = 57,
                ClientId      = Guid.NewGuid(),
            };
            RequestContext        requestContext = new() { ClientId = application.ClientId };
            CreateAuditLogService service        = CreateAuditLogService(application, requestContext);

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

        [Fact]
        public async Task CreateForbiddenGrantTypeAuditLogAsync_ShouldAddAuditLogToRepository()
        {
            // Arrange
            string grantType = GrantTypes.AuthorizationCode;
            Application application = new()
            {
                ApplicationId = 57,
                ClientId      = Guid.NewGuid(),
            };
            RequestContext        requestContext = new() { ClientId = application.ClientId };
            CreateAuditLogService service        = CreateAuditLogService(application, requestContext);

            // Act
            await service.CreateForbiddenGrantTypeAuditLogAsync(grantType);

            // Assert
            _auditLogRepository.Verify(
                r => r.Add(
                    It.Is<AuditLog>(
                        a => a.Type == AuditLogTypes.ForbiddenGrantType
                         && a.UserId == null
                         && a.ApplicationId == application.ApplicationId)),
                Times.Once);
        }

        [Fact]
        public async Task CreateFailedLoginAuditLogAsync_ShouldStoreIpAddressFromContext()
        {
            // Arrange
            User        user        = new();
            Application application = new() { ClientId = Guid.NewGuid() };
            RequestContext requestContext = new()
            {
                ClientId  = application.ClientId,
                IpAddress = "127.1.1.12",
            };
            CreateAuditLogService service = CreateAuditLogService(application, requestContext);

            // Act
            await service.CreateFailedLoginAuditLogAsync(user, application);

            // Assert
            _auditLogRepository.Verify(r => r.Add(It.Is<AuditLog>(a => a.IpAddress == requestContext.IpAddress)), Times.Once);
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
