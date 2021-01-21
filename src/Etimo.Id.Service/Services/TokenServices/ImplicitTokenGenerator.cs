using Etimo.Id.Abstractions;
using Etimo.Id.Constants;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Exceptions;
using Etimo.Id.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class ImplicitTokenGenerator : IImplicitTokenGenerator
    {
        private readonly IAccessTokenRepository     _accessTokenRepository;
        private readonly IApplicationRepository     _applicationRepository;
        private readonly IAuthenticateClientService _authenticateClientService;
        private readonly IAuthenticateUserService   _authenticateUserService;
        private readonly ICreateAuditLogService     _createAuditLogService;
        private readonly IJwtTokenFactory           _jwtTokenFactory;
        private readonly IRefreshTokenGenerator     _refreshTokenGenerator;
        private readonly IRefreshTokenRepository    _refreshTokenRepository;
        private readonly IRequestContext            _requestContext;
        private          Application                _application;
        private          JwtToken                   _jwtToken;
        private          string                     _redirectUri;
        private          RefreshToken               _refreshToken;
        private          IImplicitTokenRequest      _request;
        private          User                       _user;

        public ImplicitTokenGenerator(
            IAuthenticateUserService authenticateUserService,
            IAuthenticateClientService authenticateClientService,
            ICreateAuditLogService createAuditLogService,
            IApplicationRepository applicationRepository,
            IRefreshTokenGenerator refreshTokenGenerator,
            IAccessTokenRepository accessTokenRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtTokenFactory jwtTokenFactory,
            IRequestContext requestContext)
        {
            _authenticateUserService   = authenticateUserService;
            _authenticateClientService = authenticateClientService;
            _createAuditLogService     = createAuditLogService;
            _applicationRepository     = applicationRepository;
            _refreshTokenGenerator     = refreshTokenGenerator;
            _accessTokenRepository     = accessTokenRepository;
            _refreshTokenRepository    = refreshTokenRepository;
            _jwtTokenFactory           = jwtTokenFactory;
            _requestContext            = requestContext;
        }

        public async Task<JwtToken> GenerateTokenAsync(IImplicitTokenRequest request)
        {
            _request = request;

            UpdateContext();
            await ValidateRequestAsync();
            await CreateJwtTokenAsync();
            await GenerateRefreshTokenAsync();

            var accessToken = _jwtToken.ToAccessToken();
            _accessTokenRepository.Add(accessToken);

            await SaveAsync();

            return _jwtToken;
        }

        private void UpdateContext()
            => _requestContext.ClientId = _request.ClientId;

        private async Task ValidateRequestAsync()
        {
            if (_request.Username == null || _request.Password == null)
            {
                throw new InvalidGrantException("Invalid resource owner credentials.");
            }

            if (_request.ClientId == Guid.Empty || !await _applicationRepository.ExistsByClientIdAsync(_request.ClientId))
            {
                throw new InvalidClientException("Invalid client.");
            }

            _user        = await _authenticateUserService.AuthenticateAsync(_request.Username, _request.Password);
            _application = await _applicationRepository.FindByClientIdAsync(_request.ClientId);

            if (!_application.AllowImplicitGrant)
            {
                await _createAuditLogService.CreateForbiddenGrantTypeAuditLogAsync(GrantTypes.Implicit);

                throw new UnsupportedGrantTypeException("This authorization grant is not allowed for this application.");
            }

            var redirectUris = _application.RedirectUri.Split(" ").ToList();
            if (_request.RedirectUri == null)
            {
                if (redirectUris.Count() > 1)
                {
                    throw new InvalidGrantException("The provided redirect URI does not match the one on record.");
                }

                _redirectUri = redirectUris.First();
            }
            else
            {
                if (!RedirectUriHelper.UriMatches(_request.RedirectUri, redirectUris, _application.AllowCustomQueryParametersInRedirectUri))
                {
                    throw new InvalidGrantException("The provided redirect URI does not match the one on record.");
                }

                _redirectUri = _request.RedirectUri;
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
            if (!_application.GenerateRefreshTokenForImplicitFlow) { return; }

            _refreshToken = await _refreshTokenGenerator.GenerateRefreshTokenAsync(
                _application.ApplicationId,
                _redirectUri,
                _user.UserId,
                _request.Scope);

            _refreshToken.GrantType     = GrantTypes.AuthorizationCode;
            _refreshToken.AccessTokenId = _jwtToken.TokenId;
            _jwtToken.RefreshToken      = _refreshToken.RefreshTokenId;
        }

        private async Task SaveAsync()
        {
            await _refreshTokenRepository.SaveAsync();
            await _accessTokenRepository.SaveAsync();
        }
    }
}
