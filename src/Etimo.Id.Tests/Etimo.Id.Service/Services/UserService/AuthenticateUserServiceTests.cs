using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service;
using Etimo.Id.Service.Exceptions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Etimo.Id.Tests
{
    public class AuthenticateUserServiceTests
    {
        [Fact]
        public async Task AuthenticateAsync_NullUser_ShouldThrowInvalidGrantException()
        {
            // Arrange
            AuthenticateUserService service = CreateAuthenticateUserService(null, true);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidGrantException>(() => service.AuthenticateAsync(new AuthorizationRequest()));
        }

        [Fact]
        public async Task AuthenticateAsync_VerifyFailed_ShouldThrowInvalidGrantException()
        {
            // Arrange
            AuthenticateUserService service = CreateAuthenticateUserService(new User(), false);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidGrantException>(() => service.AuthenticateAsync(new AuthorizationRequest()));
        }

        [Fact]
        public async Task AuthenticateAsync_LockedUser_ShouldThrowInvalidGrantException()
        {
            // Arrange
            User                    user    = new() { LockedUntilDateTime = DateTime.UtcNow.AddMinutes(30) };
            AuthenticateUserService service = CreateAuthenticateUserService(user, true);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidGrantException>(() => service.AuthenticateAsync(new AuthorizationRequest()));
        }

        [Fact]
        public async Task AuthenticateAsync_LockedUserSucceedsWithLogin_ShouldResetFailedLogins()
        {
            // Arrange
            User                    user    = new() { FailedLogins = 1 };
            AuthenticateUserService service = CreateAuthenticateUserService(user, true);

            // Act
            await service.AuthenticateAsync(new AuthorizationRequest());

            // Assert
            Assert.Equal(0, user.FailedLogins);
        }

        private AuthenticateUserService CreateAuthenticateUserService(User user, bool authenticationSuccess)
            => new(CreateUserRepository(user), new Mock<ILockUserService>().Object, CreatePasswordHasher(authenticationSuccess));

        private IUserRepository CreateUserRepository(User user)
        {
            var mock = new Mock<IUserRepository>();
            mock.Setup(m => m.FindByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
            return mock.Object;
        }

        private IPasswordHasher CreatePasswordHasher(bool verifyResult)
        {
            var mock = new Mock<IPasswordHasher>();
            mock.Setup(m => m.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(verifyResult);
            return mock.Object;
        }


        private class AuthorizationRequest : IAuthenticationRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string State    { get; set; }
        }
    }
}
