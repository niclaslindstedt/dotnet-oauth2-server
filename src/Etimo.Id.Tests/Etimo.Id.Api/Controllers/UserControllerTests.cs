using Etimo.Id.Abstractions;
using Etimo.Id.Api.Settings;
using Etimo.Id.Api.Users;
using Etimo.Id.Constants;
using Etimo.Id.Entities;
using Etimo.Id.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Etimo.Id.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public async Task GetAsync_NotAdmin_ShouldFindOwnUser()
        {
            // Arrange
            var             userId         = Guid.NewGuid();
            ClaimsPrincipal claimsIdentity = CreateUser(userId);
            var             user           = new User { UserId = userId };
            var             mock           = new Mock<IFindUserService>();
            UserController  controller     = CreateUserController(user: claimsIdentity, findUserService: mock);
            mock.Setup(m => m.FindAsync(It.Is<Guid>(g => g == userId))).ReturnsAsync(user);

            // Act
            await controller.GetAsync();

            // Assert
            mock.Verify(s => s.FindAsync(It.Is<Guid>(g => g == userId)), Times.Once);
        }

        [Fact]
        public async Task GetAsync_WithAdminScope_ShouldGetAllUsers()
        {
            // Arrange
            var             userId         = Guid.NewGuid();
            ClaimsPrincipal claimsIdentity = CreateUser(userId, "admin:user");
            var             users          = new List<User> { new(), new() };
            var             mock           = new Mock<IGetUsersService>();
            UserController  controller     = CreateUserController(user: claimsIdentity, getUsersService: mock);
            mock.Setup(m => m.GetAllAsync()).ReturnsAsync(users);

            // Act
            await controller.GetAsync();

            // Assert
            mock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task FindAsync_OwnUserId_ShouldFindUser()
        {
            // Arrange
            var             userId         = Guid.NewGuid();
            ClaimsPrincipal claimsIdentity = CreateUser(userId);
            var             user           = new User { UserId = userId };
            var             mock           = new Mock<IFindUserService>();
            UserController  controller     = CreateUserController(user: claimsIdentity, findUserService: mock);
            mock.Setup(m => m.FindAsync(It.Is<Guid>(g => g == userId))).ReturnsAsync(user);

            // Act
            await controller.FindAsync(userId);

            // Assert
            mock.Verify(s => s.FindAsync(It.Is<Guid>(g => g == userId)), Times.Once);
        }

        [Fact]
        public async Task FindAsync_OtherUserId_ShouldThrowForbiddenException()
        {
            // Arrange
            var             userId         = Guid.NewGuid();
            ClaimsPrincipal claimsIdentity = CreateUser(userId);
            var             user           = new User { UserId = userId };
            UserController  controller     = CreateUserController(user: claimsIdentity);

            // Act + Assert
            await Assert.ThrowsAsync<ForbiddenException>(async () => await controller.FindAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task FindAsync_OtherUserIdAsAdmin_ShouldFindUser()
        {
            // Arrange
            var             userId         = Guid.NewGuid();
            var             otherUserId    = Guid.NewGuid();
            ClaimsPrincipal claimsIdentity = CreateUser(userId, "admin:user");
            var             otherUser      = new User { UserId = userId };
            var             mock           = new Mock<IFindUserService>();
            UserController  controller     = CreateUserController(user: claimsIdentity, findUserService: mock);
            mock.Setup(m => m.FindAsync(It.Is<Guid>(g => g == otherUserId))).ReturnsAsync(otherUser);

            // Act
            await controller.FindAsync(otherUserId);

            // Assert
            mock.Verify(s => s.FindAsync(It.Is<Guid>(g => g == otherUserId)), Times.Once);
        }

        [Fact]
        public async Task GetApplicationsAsync_OwnUserId_ShouldGetApplications()
        {
            // Arrange
            var               userId         = Guid.NewGuid();
            ClaimsPrincipal   claimsIdentity = CreateUser(userId);
            List<Application> applications   = new() { new Application(), new Application() };
            var               mock           = new Mock<IGetApplicationsService>();
            UserController    controller     = CreateUserController(user: claimsIdentity, getApplicationsService: mock);
            mock.Setup(m => m.GetByUserIdAsync(It.Is<Guid>(g => g == userId))).ReturnsAsync(applications);

            // Act
            await controller.GetApplicationsAsync(userId);

            // Assert
            mock.Verify(s => s.GetByUserIdAsync(It.Is<Guid>(g => g == userId)), Times.Once);
        }

        [Fact]
        public async Task GetApplicationsAsync_OtherUserId_ShouldThrowForbiddenException()
        {
            // Arrange
            var               userId         = Guid.NewGuid();
            var               otherUserId    = Guid.NewGuid();
            ClaimsPrincipal   claimsIdentity = CreateUser(userId);
            List<Application> applications   = new() { new Application(), new Application() };
            UserController    controller     = CreateUserController(user: claimsIdentity);

            // Act + Assert
            await Assert.ThrowsAsync<ForbiddenException>(async () => await controller.GetApplicationsAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetApplicationsAsync_OtherUserId_ShouldGetApplications()
        {
            // Arrange
            var               userId         = Guid.NewGuid();
            var               otherUserId    = Guid.NewGuid();
            ClaimsPrincipal   claimsIdentity = CreateUser(userId, "admin:user");
            List<Application> applications   = new() { new Application(), new Application() };
            var               mock           = new Mock<IGetApplicationsService>();
            UserController    controller     = CreateUserController(user: claimsIdentity, getApplicationsService: mock);
            mock.Setup(m => m.GetByUserIdAsync(It.Is<Guid>(g => g == otherUserId))).ReturnsAsync(applications);

            // Act
            await controller.GetApplicationsAsync(otherUserId);

            // Assert
            mock.Verify(s => s.GetByUserIdAsync(It.Is<Guid>(g => g == otherUserId)), Times.Once);
        }

        private UserController CreateUserController(
            SiteSettings siteSettings = null,
            Mock<IAddUserRoleRelationService> addUserRoleRelationService = null,
            Mock<IAddUserService> addUserService = null,
            Mock<IDeleteUserRoleRelationService> deleteUserRoleRelationService = null,
            Mock<IDeleteUserService> deleteUserService = null,
            Mock<IFindUserService> findUserService = null,
            Mock<IGetApplicationsService> getApplicationsService = null,
            Mock<IGetRolesService> getRolesService = null,
            Mock<IGetUsersService> getUsersService = null,
            Mock<IUnlockUserService> unlockUserService = null,
            Mock<IUpdateUserService> updateUserService = null,
            ClaimsPrincipal user = null)
        {
            siteSettings                  ??= new SiteSettings();
            addUserRoleRelationService    ??= new Mock<IAddUserRoleRelationService>();
            addUserService                ??= new Mock<IAddUserService>();
            deleteUserRoleRelationService ??= new Mock<IDeleteUserRoleRelationService>();
            deleteUserService             ??= new Mock<IDeleteUserService>();
            findUserService               ??= new Mock<IFindUserService>();
            getApplicationsService        ??= new Mock<IGetApplicationsService>();
            getRolesService               ??= new Mock<IGetRolesService>();
            getUsersService               ??= new Mock<IGetUsersService>();
            unlockUserService             ??= new Mock<IUnlockUserService>();
            updateUserService             ??= new Mock<IUpdateUserService>();

            var controller = new UserController(
                siteSettings,
                addUserRoleRelationService.Object,
                addUserService.Object,
                deleteUserRoleRelationService.Object,
                deleteUserService.Object,
                findUserService.Object,
                getApplicationsService.Object,
                getRolesService.Object,
                getUsersService.Object,
                unlockUserService.Object,
                updateUserService.Object);

            if (user != null)
            {
                controller.ControllerContext             = new ControllerContext();
                controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            }

            return controller;
        }

        private ClaimsPrincipal CreateUser(Guid userId, params string[] scopes)
        {
            Claim[] claims =
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(CustomClaimTypes.Scope, string.Join(" ", scopes)),
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthentication"));

            return user;
        }
    }
}
