using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class ResourceOwnerCredentialsTokenGenerator : IResourceOwnerCredentialsTokenGenerator
    {
        private readonly IAccessTokenRepository                        _accessTokenRepository;
        private readonly IApplicationRepository                        _applicationRepository;
        private readonly IAuthenticateClientService                    _authenticateClientService;
        private readonly IAuthenticateUserService                      _authenticateUserService;
        private readonly ICreateAuditLogService                        _createAuditLogService;
        private readonly IJwtTokenFactory                              _jwtTokenFactory;
        private readonly IRefreshTokenGenerator                        _refreshTokenGenerator;
        private readonly IRequestContext                               _requestContext;
        private          Application                                   _application;
        private          JwtToken                                      _jwtToken;
        private          RefreshToken                                  _refreshToken;
        private          IResourceOwnerPasswordCredentialsTokenRequest _request;
        private          User                                          _user;

        public ResourceOwnerCredentialsTokenGenerator(
            IAuthenticateUserService authenticateUserService,
            IAuthenticateClientService authenticateClientService,
            IAccessTokenRepository accessTokenRepository,
            IApplicationRepository applicationRepository,
            ICreateAuditLogService createAuditLogService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IJwtTokenFactory jwtTokenFactory,
            IRequestContext requestContext)
        {
            _authenticateUserService   = authenticateUserService;
            _authenticateClientService = authenticateClientService;
            _applicationRepository     = applicationRepository;
            _createAuditLogService     = createAuditLogService;
            _refreshTokenGenerator     = refreshTokenGenerator;
            _accessTokenRepository     = accessTokenRepository;
            _jwtTokenFactory           = jwtTokenFactory;
            _requestContext            = requestContext;
        }

        public async Task<JwtToken> GenerateTokenAsync(IResourceOwnerPasswordCredentialsTokenRequest request)
        {
            _request = request;

            UpdateContext();
            await ValidateRequestAsync();
            await CreateJwtTokenAsync();
            await GenerateRefreshTokenAsync();

            var accessToken = _jwtToken.ToAccessToken();
            _accessTokenRepository.Add(accessToken);
            await _accessTokenRepository.SaveAsync();

            return _jwtToken;
        }

        private void UpdateContext()
        {
            _requestContext.ClientId = _request.ClientId;
            _requestContext.Username = _request.Username;
        }

        private async Task ValidateRequestAsync()
        {
            if (_request.Username == null || _request.Password == null)
            {
                throw new InvalidGrantException("Invalid resource owner credentials.");
            }

            if (_request.ClientId == Guid.Empty
             || _request.ClientSecret == null
             || !await _applicationRepository.ExistsByClientIdAsync(_request.ClientId))
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            _user        = await _authenticateUserService.AuthenticateAsync(_request.Username, _request.Password);
            _application = await _authenticateClientService.AuthenticateAsync(_request.ClientId, _request.ClientSecret);

            if (!_application.AllowResourceOwnerPasswordCredentialsGrant)
            {
                await _createAuditLogService.CreateForbiddenGrantTypeAuditLogAsync(GrantTypes.AuthorizationCode);

                throw new UnsupportedGrantTypeException("This authorization grant is not allowed for this application.");
            }

            if (!_application.AllowCredentialsInBody && _request.CredentialsInBody)
            {
                throw new InvalidGrantException("This application does not allow passing credentials in the request body.");
            }
        }

        private async Task CreateJwtTokenAsync()
        {
            var jwtRequest = new JwtTokenRequest
            {
                Audience        = new List<string> { _application.ClientId.ToString() },
                ClientId        = _application.ClientId,
                Username        = _user.Username,
                Subject         = _user.UserId.ToString(),
                Scope           = _request.Scope,
                LifetimeMinutes = _application.AccessTokenLifetimeMinutes,
            };

            _jwtToken = await _jwtTokenFactory.CreateJwtTokenAsync(jwtRequest);
        }

        private async Task GenerateRefreshTokenAsync()
        {
            if (!_application.GenerateRefreshTokenForPasswordCredentials) { return; }

            _refreshToken = await _refreshTokenGenerator.GenerateRefreshTokenAsync(
                _application.ApplicationId,
                null,
                _application.UserId,
                _request.Scope);
            _refreshToken.GrantType     = GrantTypes.Password;
            _refreshToken.AccessTokenId = _jwtToken.TokenId;
            _jwtToken.RefreshToken      = _refreshToken.RefreshTokenId;
        }
    }
}
