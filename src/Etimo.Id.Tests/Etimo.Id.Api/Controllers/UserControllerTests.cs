using Etimo.Id.Abstractions;
using Etimo.Id.Api.Users;
using Etimo.Id.Constants;
using Etimo.Id.Dtos;
using Etimo.Id.Entities;
using Etimo.Id.Exceptions;
using Etimo.Id.Settings;
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
        private readonly ClaimsPrincipal   _adminIdentity;
        private readonly List<Application> _applicationList;
        private readonly ClaimsPrincipal   _callerIdentity;
        private readonly User              _callerUser;
        private readonly Guid              _callerUserId;
        private readonly User              _otherUser;
        private readonly Guid              _otherUserId;
        private readonly List<Role>        _rolesList;
        private readonly List<User>        _userList;

        public UserControllerTests()
        {
            _callerUserId    = Guid.NewGuid();
            _otherUserId     = Guid.NewGuid();
            _callerIdentity  = CreateUser(_callerUserId);
            _adminIdentity   = CreateUser(_callerUserId, "etimoid:admin:user");
            _callerUser      = new User { UserId = _callerUserId };
            _otherUser       = new User { UserId = _otherUserId };
            _userList        = new List<User> { new(), new() };
            _applicationList = new List<Application> { new(), new() };
            _rolesList       = new List<Role> { new(), new() };
        }

        [Fact]
        public async Task GetAsync_When_Non_Admin_Calls_Then_Return_Callers_User()
        {
            // Arrange
            var            mock       = new Mock<IFindUserService>();
            UserController controller = CreateUserController(user: _callerIdentity, findUserService: mock);
            mock.Setup(m => m.FindAsync(It.Is<Guid>(g => g == _callerUserId))).ReturnsAsync(_callerUser);

            // Act
            await controller.GetAsync();

            // Assert
            mock.Verify(s => s.FindAsync(It.Is<Guid>(g => g == _callerUserId)), Times.Once);
        }

        [Fact]
        public async Task GetAsync_When_Admin_Calls_Then_Return_All_Users()
        {
            // Arrange
            var            mock       = new Mock<IGetUsersService>();
            UserController controller = CreateUserController(user: _adminIdentity, getUsersService: mock);
            mock.Setup(m => m.GetAllAsync()).ReturnsAsync(_userList);

            // Act
            await controller.GetAsync();

            // Assert
            mock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task FindAsync_When_Caller_Requests_Own_UserId_Then_Return_That_User()
        {
            // Arrange
            var            mock       = new Mock<IFindUserService>();
            UserController controller = CreateUserController(user: _callerIdentity, findUserService: mock);
            mock.Setup(m => m.FindAsync(It.Is<Guid>(g => g == _callerUserId))).ReturnsAsync(_callerUser);

            // Act
            await controller.FindAsync(_callerUserId);

            // Assert
            mock.Verify(s => s.FindAsync(It.Is<Guid>(g => g == _callerUserId)), Times.Once);
        }

        [Fact]
        public async Task FindAsync_When_Caller_Requests_Other_UserId_Then_Throw_ForbiddenException()
        {
            // Arrange
            UserController controller = CreateUserController(user: _callerIdentity);

            // Act + Assert
            await Assert.ThrowsAsync<ForbiddenException>(async () => await controller.FindAsync(_otherUserId));
        }

        [Fact]
        public async Task FindAsync_When_Admin_Requests_Other_UserId_Then_Return_That_User()
        {
            // Arrange
            var            mock       = new Mock<IFindUserService>();
            UserController controller = CreateUserController(user: _adminIdentity, findUserService: mock);
            mock.Setup(m => m.FindAsync(It.Is<Guid>(g => g == _otherUserId))).ReturnsAsync(_otherUser);

            // Act
            await controller.FindAsync(_otherUserId);

            // Assert
            mock.Verify(s => s.FindAsync(It.Is<Guid>(g => g == _otherUserId)), Times.Once);
        }

        [Fact]
        public async Task GetApplicationsAsync_When_Caller_Requests_Own_UserId_Then_Return_Owned_Applications()
        {
            // Arrange
            var            mock       = new Mock<IGetApplicationsService>();
            UserController controller = CreateUserController(user: _callerIdentity, getApplicationsService: mock);
            mock.Setup(m => m.GetByUserIdAsync(It.Is<Guid>(g => g == _callerUserId))).ReturnsAsync(_applicationList);

            // Act
            await controller.GetApplicationsAsync(_callerUserId);

            // Assert
            mock.Verify(s => s.GetByUserIdAsync(It.Is<Guid>(g => g == _callerUserId)), Times.Once);
        }

        [Fact]
        public async Task GetApplicationsAsync_When_Caller_Requests_Other_UserId_Then_Throw_ForbiddenException()
        {
            // Arrange
            UserController controller = CreateUserController(user: _callerIdentity);

            // Act + Assert
            await Assert.ThrowsAsync<ForbiddenException>(async () => await controller.GetApplicationsAsync(_otherUserId));
        }

        [Fact]
        public async Task GetApplicationsAsync_When_Admin_Requests_Other_UserId_Then_Return_That_Users_Owned_Applications()
        {
            // Arrange
            var            mock       = new Mock<IGetApplicationsService>();
            UserController controller = CreateUserController(user: _adminIdentity, getApplicationsService: mock);
            mock.Setup(m => m.GetByUserIdAsync(It.Is<Guid>(g => g == _otherUserId))).ReturnsAsync(_applicationList);

            // Act
            await controller.GetApplicationsAsync(_otherUserId);

            // Assert
            mock.Verify(s => s.GetByUserIdAsync(It.Is<Guid>(g => g == _otherUserId)), Times.Once);
        }

        [Fact]
        public async Task GetRolesAsync_When_Caller_Requests_Own_UserId_Then_Return_Owned_Roles()
        {
            // Arrange
            var            mock       = new Mock<IGetRolesService>();
            UserController controller = CreateUserController(user: _callerIdentity, getRolesService: mock);
            mock.Setup(m => m.GetByUserIdAsync(It.Is<Guid>(g => g == _callerUserId))).ReturnsAsync(_rolesList);

            // Act
            await controller.GetRolesAsync(_callerUserId);

            // Assert
            mock.Verify(s => s.GetByUserIdAsync(It.Is<Guid>(g => g == _callerUserId)), Times.Once);
        }

        [Fact]
        public async Task GetRolesAsync_When_Caller_Requests_Other_UserId_Then_Throw_ForbiddenException()
        {
            // Arrange
            UserController controller = CreateUserController(user: _callerIdentity);

            // Act + Assert
            await Assert.ThrowsAsync<ForbiddenException>(async () => await controller.GetRolesAsync(_otherUserId));
        }

        [Fact]
        public async Task GetRolesAsync_When_Admin_Requests_Other_UserId_Then_Return_That_Users_Owned_Roles()
        {
            // Arrange
            var            mock       = new Mock<IGetRolesService>();
            UserController controller = CreateUserController(user: _adminIdentity, getRolesService: mock);
            mock.Setup(m => m.GetByUserIdAsync(It.Is<Guid>(g => g == _otherUserId))).ReturnsAsync(_rolesList);

            // Act
            await controller.GetRolesAsync(_otherUserId);

            // Assert
            mock.Verify(s => s.GetByUserIdAsync(It.Is<Guid>(g => g == _otherUserId)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Use_RequestDto_To_Add_User()
        {
            // Arrange
            var            dto        = new UserRequestDto { username = "test123" };
            var            mock       = new Mock<IAddUserService>();
            UserController controller = CreateUserController(user: _callerIdentity, addUserService: mock);
            mock.Setup(m => m.AddAsync(It.Is<User>(u => u.Username == dto.username))).ReturnsAsync(dto.ToUser());

            // Act
            await controller.CreateAsync(dto);

            // Assert
            mock.Verify(s => s.AddAsync(It.Is<User>(u => u.Username == dto.username)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_201_Created()
        {
            // Arrange
            var            settings   = new SiteSettings { ListenUri  = "http://localhost" };
            var            dto        = new UserRequestDto { username = "test123" };
            var            mock       = new Mock<IAddUserService>();
            UserController controller = CreateUserController(user: _callerIdentity, addUserService: mock, siteSettings: settings);
            mock.Setup(m => m.AddAsync(It.Is<User>(u => u.Username == dto.username))).ReturnsAsync(dto.ToUser());

            // Act
            var result = await controller.CreateAsync(dto) as CreatedResult;

            // Assert
            Assert.Equal(201, result.StatusCode);
            Assert.StartsWith($"{settings.ListenUri}/users/", result.Location);
            Assert.Equal(dto.username, (result.Value as UserResponseDto).username);
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
