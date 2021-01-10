using Etimo.Id.Abstractions;
using Etimo.Id.Constants;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Exceptions;
using Etimo.Id.Service.TokenGenerators;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Etimo.Id.Tests.Etimo.Id.Service.Services.TokenServices
{
    public class ClientCredentialsTokenGeneratorTests
    {
        private const int    AccessTokenLifetimeMinutes = 1337;
        private const int    ApplicationId              = 666;
        private const string DummySecret                = "DummySecret";
        private const string DummyScope                 = "DummyScope";
        private const string DummyUsername              = "DummyUsername";

        private readonly Mock<IAccessTokenRepository>     _accessTokenRepositoryMock;
        private readonly Mock<IAuthenticateClientService> _authenticateClientServiceMock;
        private readonly Mock<ICreateAuditLogService>     _createAuditLogServiceMock;
        private readonly Mock<IJwtTokenFactory>           _jwtTokenFactoryMock;
        private readonly Mock<IRefreshTokenGenerator>     _refreshTokenGeneratorMock;
        private readonly Mock<IRequestContext>            _requestContextMock;
        private readonly ClientCredentialsTokenGenerator  _tokenGenerator;
        private readonly Guid                             DummyClientId = Guid.NewGuid();
        private readonly Guid                             DummyUserId   = Guid.NewGuid();

        public ClientCredentialsTokenGeneratorTests()
        {
            _authenticateClientServiceMock = new Mock<IAuthenticateClientService>();
            _createAuditLogServiceMock     = new Mock<ICreateAuditLogService>();
            _accessTokenRepositoryMock     = new Mock<IAccessTokenRepository>();
            _jwtTokenFactoryMock           = new Mock<IJwtTokenFactory>();
            _refreshTokenGeneratorMock     = new Mock<IRefreshTokenGenerator>();
            _requestContextMock            = new Mock<IRequestContext>();

            _tokenGenerator = new ClientCredentialsTokenGenerator(
                _authenticateClientServiceMock.Object,
                _accessTokenRepositoryMock.Object,
                _createAuditLogServiceMock.Object,
                _jwtTokenFactoryMock.Object,
                _refreshTokenGeneratorMock.Object,
                _requestContextMock.Object);
        }

        [Fact]
        public void GenerateTokenAsync_When_ClientId_Is_Empty_Validation_Throws_InvalidClientException()
        {
            // Arrange
            ClientCredentialsTokenRequest request = GetRequest();
            request.ClientId = Guid.Empty;

            // Assert
            Should.Throw<InvalidClientException>(async () => await _tokenGenerator.GenerateTokenAsync(request))
                .Message.ShouldBe("Invalid client credentials.");
        }

        [Fact]
        public void GenerateTokenAsync_When_ClientSecret_Is_Null_Validation_Throws_InvalidClientException()
        {
            // Arrange
            ClientCredentialsTokenRequest request = GetRequest();
            request.ClientSecret = null;

            // Assert
            Should.Throw<InvalidClientException>(async () => await _tokenGenerator.GenerateTokenAsync(request))
                .Message.ShouldBe("Invalid client credentials.");
        }

        [Fact]
        public void GenerateTokenAsync_When_Application_Is_Public_Validation_Throws_UnauthorizedClientException()
        {
            // Arrange
            Application application = GetApplication();
            application.Type = ClientTypes.Public;
            AuthenticateAsyncReturns(application);

            // Assert
            Should.Throw<UnauthorizedClientException>(async () => await _tokenGenerator.GenerateTokenAsync(GetRequest()))
                .Message.ShouldBe("Public clients cannot use the client credentials grant.");
        }

        [Fact]
        public void
            GenerateTokenAsync_When_Application_Is_Not_Allowing_ClientCredentialsGrant_Validation_Throws_UnsupportedGrantTypeException()
        {
            // Arrange
            Application application = GetApplication();
            application.AllowClientCredentialsGrant = false;
            AuthenticateAsyncReturns(application);

            // Assert
            Should.Throw<UnsupportedGrantTypeException>(async () => await _tokenGenerator.GenerateTokenAsync(GetRequest()))
                .Message.ShouldBe("This authorization grant is not allowed for this application.");
        }

        [Fact]
        public void
            GenerateTokenAsync_When_Credentials_Is_In_Body_And_Application_Is_Not_Supporting_That_Validation_Throws_InvalidGrantException()
        {
            // Arrange
            ClientCredentialsTokenRequest request = GetRequest();
            request.CredentialsInBody = true;

            Application application = GetApplication();
            application.AllowCredentialsInBody = false;
            AuthenticateAsyncReturns(application);

            // Assert
            Should.Throw<InvalidGrantException>(async () => await _tokenGenerator.GenerateTokenAsync(request))
                .Message.ShouldBe("This application does not allow passing credentials in the request body.");
        }

        [Fact]
        public async Task GenerateTokenAsync_Creates_JwtToken_Based_On_Request_And_App()
        {
            // Arrange
            AuthenticateAsyncReturns(GetApplication());
            CreateJwtTokenReturns(new JwtToken());

            // Act
            await _tokenGenerator.GenerateTokenAsync(GetRequest());

            // Assert
            var jwtRequest = new JwtTokenRequest
            {
                Audience        = new List<string> { DummyClientId.ToString() },
                ClientId        = DummyClientId,
                Subject         = DummyUserId.ToString(),
                Username        = DummyUsername,
                Scope           = DummyScope,
                LifetimeMinutes = AccessTokenLifetimeMinutes,
            };

            _jwtTokenFactoryMock.Verify(m => m.CreateJwtTokenAsync(Is(jwtRequest)));
        }

        [Fact]
        public async Task GenerateTokenAsync_When_Refreshtokens_Is_Disabled_For_The_Application_Refreshtoken_Is_Not_Generated()
        {
            // Arrange
            Application application = GetApplication();
            application.GenerateRefreshTokenForClientCredentials = false;
            AuthenticateAsyncReturns(application);

            CreateJwtTokenReturns(new JwtToken());

            // Act
            await _tokenGenerator.GenerateTokenAsync(GetRequest());

            // Assert
            _refreshTokenGeneratorMock.Verify(
                m => m.GenerateRefreshTokenAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task GenerateTokenAsync_When_Refreshtokens_Is_Disabled_For_The_Application_RefreshToken_Is_Generated()
        {
            // Arrange
            Application application = GetApplication();
            application.GenerateRefreshTokenForClientCredentials = true;
            AuthenticateAsyncReturns(application);

            CreateJwtTokenReturns(new JwtToken());
            CreateRefreshTokenReturns(new RefreshToken());

            // Act
            await _tokenGenerator.GenerateTokenAsync(GetRequest());

            // Assert
            _refreshTokenGeneratorMock.Verify(
                m => m.GenerateRefreshTokenAsync(
                    Is(ApplicationId),
                    Is<string>(null),
                    Is(DummyUserId),
                    Is(DummyScope)),
                Times.Once);
        }

        [Fact]
        public async Task GenerateTokenAsync_Saves_AccessToken_In_Db()
        {
            // Arrange
            AuthenticateAsyncReturns(GetApplication());
            CreateJwtTokenReturns(new JwtToken());
            CreateRefreshTokenReturns(new RefreshToken());

            // Act
            await _tokenGenerator.GenerateTokenAsync(GetRequest());

            // Assert
            _accessTokenRepositoryMock.Verify(m => m.Add(It.IsAny<AccessToken>()), Times.Once);
            _accessTokenRepositoryMock.Verify(m => m.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task GenerateTokenAsync_Sets_RefreshToken_Properties()
        {
            // Arrange
            Application application = GetApplication();
            application.GenerateRefreshTokenForClientCredentials = true;
            AuthenticateAsyncReturns(application);

            CreateJwtTokenReturns(
                new JwtToken
                {
                    TokenId = Guid.NewGuid(),
                });

            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid().ToString(),
            };
            CreateRefreshTokenReturns(refreshToken);

            // Act
            JwtToken token = await _tokenGenerator.GenerateTokenAsync(GetRequest());

            // Assert
            refreshToken.GrantType.ShouldBe(GrantTypes.ClientCredentials);
            refreshToken.AccessTokenId.ShouldBe(token.TokenId);
        }

        [Fact]
        public async Task GenerateTokenAsync_Sets_RefreshTokenId_ToJwt()
        {
            // Arrange
            Application application = GetApplication();
            application.GenerateRefreshTokenForClientCredentials = true;
            AuthenticateAsyncReturns(application);

            CreateJwtTokenReturns(
                new JwtToken
                {
                    TokenId = Guid.NewGuid(),
                });

            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid().ToString(),
            };

            CreateRefreshTokenReturns(refreshToken);

            // Act
            JwtToken token = await _tokenGenerator.GenerateTokenAsync(GetRequest());

            // Assert
            token.RefreshToken.ShouldBe(refreshToken.RefreshTokenId);
        }

        private ClientCredentialsTokenRequest GetRequest()
            => new(DummyScope)
            {
                ClientId     = DummyClientId,
                ClientSecret = DummySecret,
            };

        private Application GetApplication()
            => new()
            {
                ApplicationId               = ApplicationId,
                Type                        = ClientTypes.Confidential,
                AllowClientCredentialsGrant = true,
                AccessTokenLifetimeMinutes  = AccessTokenLifetimeMinutes,
                UserId                      = DummyUserId,
                User = new User
                {
                    Username = DummyUsername,
                },
            };

        private void AuthenticateAsyncReturns(Application application)
            => _authenticateClientServiceMock.Setup(service => service.AuthenticateAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.FromResult(application));

        private void CreateJwtTokenReturns(JwtToken token)
            => _jwtTokenFactoryMock.Setup(service => service.CreateJwtTokenAsync(It.IsAny<IJwtTokenRequest>()))
                .Returns(Task.FromResult(token));

        private void CreateRefreshTokenReturns(RefreshToken token)
            => _refreshTokenGeneratorMock
                .Setup(
                    service => service.GenerateRefreshTokenAsync(
                        It.IsAny<int>(),
                        It.IsAny<string>(),
                        It.IsAny<Guid>(),
                        It.IsAny<string>()))
                .Returns(Task.FromResult(token));

        private T Is<T>(T expected)
        {
            Func<T, bool> isEqual = x =>
            {
                x.ShouldBeEquivalentTo(expected);
                return true;
            };
            return It.Is<T>(x => isEqual(x));
        }
    }
}
