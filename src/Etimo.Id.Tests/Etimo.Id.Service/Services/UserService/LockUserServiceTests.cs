using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Etimo.Id.Tests
{
    public class LockUserServiceTests
    {
        [Fact]
        public async Task LockAsync_NewUser_ShouldNotLock()
        {
            // Arrange
            LockUserService service = CreateLockUserService(3, 30);
            User            user    = new();

            // Act
            await service.LockAsync(user);

            // Assert
            Assert.False(user.IsLocked);
        }

        [Fact]
        public async Task LockAsync_TwoAttempts_ShouldNotLock()
        {
            // Arrange
            LockUserService service = CreateLockUserService(3, 30);
            User            user    = new();

            // Act
            await service.LockAsync(user);
            await service.LockAsync(user);

            // Assert
            Assert.False(user.IsLocked);
        }

        [Fact]
        public async Task LockAsync_ThreeAttempts_ShouldLock()
        {
            // Arrange
            LockUserService service = CreateLockUserService(3, 30);
            User            user    = new();

            // Act
            await service.LockAsync(user);
            await service.LockAsync(user);
            await service.LockAsync(user);

            // Assert
            Assert.True(user.IsLocked);
        }

        [Fact]
        public async Task LockAsync_ThreeAttempts_ShouldLockForForThirtyMinutes()
        {
            // Arrange
            LockUserService service = CreateLockUserService(3, 30);
            User            user    = new();

            // Act
            await service.LockAsync(user);
            await service.LockAsync(user);
            await service.LockAsync(user);

            // Assert
            var timeUntilUnlocked = (int)Math.Ceiling((user.LockedUntilDateTime.GetValueOrDefault() - DateTime.UtcNow).TotalMinutes);
            Assert.Equal(30, timeUntilUnlocked);
        }

        private LockUserService CreateLockUserService(int failedLoginsBeforeLocked, int failedLoginsLockLifetimeMinutes)
        {
            Application application = new()
            {
                FailedLoginsBeforeLocked        = failedLoginsBeforeLocked,
                FailedLoginsLockLifetimeMinutes = failedLoginsLockLifetimeMinutes,
            };

            return new LockUserService(
                new Mock<IUserRepository>().Object,
                CreateApplicationRepository(application),
                new Mock<ICreateAuditLogService>().Object,
                new RequestContext { ClientId = application.ClientId });
        }

        private IApplicationRepository CreateApplicationRepository(Application applicationToReturn)
        {
            var mock = new Mock<IApplicationRepository>();
            mock.Setup(m => m.FindByClientIdAsync(applicationToReturn.ClientId)).ReturnsAsync(applicationToReturn);
            return mock.Object;
        }
    }
}
