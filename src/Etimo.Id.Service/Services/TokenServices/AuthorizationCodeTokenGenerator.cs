using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class AuthorizationCodeTokenGenerator : IAuthorizationCodeTokenGenerator
    {
        private readonly IAccessTokenRepository         _accessTokenRepository;
        private readonly IAuthenticateClientService     _authenticateClientService;
        private readonly IAuthorizationCodeRepository   _authorizationCodeRepository;
        private readonly ICreateAuditLogService         _createAuditLogService;
        private readonly IFindApplicationService        _findApplicationService;
        private readonly IJwtTokenFactory               _jwtTokenFactory;
        private readonly IRefreshTokenGenerator         _refreshTokenGenerator;
        private readonly IRefreshTokenRepository        _refreshTokenRepository;
        private readonly IRequestContext                _requestContext;
        private          Application                    _application;
        private          AuthorizationCode              _code;
        private          JwtToken                       _jwtToken;
        private          string                         _redirectUri;
        private          RefreshToken                   _refreshToken;
        private          IAuthorizationCodeTokenRequest _request;

        public AuthorizationCodeTokenGenerator(
            IAuthenticateClientService authenticateClientService,
            ICreateAuditLogService createAuditLogService,
            IFindApplicationService findApplicationService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IAccessTokenRepository accessTokenRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtTokenFactory jwtTokenFactory,
            IRequestContext requestContext)
        {
            _authenticateClientService   = authenticateClientService;
            _createAuditLogService       = createAuditLogService;
            _findApplicationService      = findApplicationService;
            _refreshTokenGenerator       = refreshTokenGenerator;
            _authorizationCodeRepository = authorizationCodeRepository;
            _accessTokenRepository       = accessTokenRepository;
            _refreshTokenRepository      = refreshTokenRepository;
            _jwtTokenFactory             = jwtTokenFactory;
            _requestContext              = requestContext;
        }

        public async Task<JwtToken> GenerateTokenAsync(IAuthorizationCodeTokenRequest request)
        {
            _request = request;

            UpdateContext();
            await ValidateRequestAsync();
            await CreateJwtTokenAsync();
            await GenerateRefreshTokenAsync();

            _code.Used          = true;
            _code.AccessTokenId = _jwtToken.TokenId;

            var accessToken = _jwtToken.ToAccessToken();
            _accessTokenRepository.Add(accessToken);

            await SaveAsync();

            return _jwtToken;
        }

        private void UpdateContext()
            => _requestContext.ClientId = _request.ClientId;

        private async Task ValidateRequestAsync()
        {
            if (_request.ClientId == Guid.Empty) { throw new InvalidClientException("Invalid client credentials."); }

            if (_request.Code == null) { throw new InvalidGrantException("Invalid authorization code."); }

            _code = await _authorizationCodeRepository.FindAsync(_request.Code);
            if (_code?.UserId == null || _code.IsExpired) { throw new InvalidGrantException("Invalid authorization code."); }

            // If someone tries to use the same authorization code twice, disable the access token.
            if (_code.Used)
            {
                if (_code.AccessToken != null)
                {
                    await _createAuditLogService.CreateAuthorizationCodeAbuseAuditLogAsync(_code);

                    _code.AccessToken.Disabled = true;
                    await _accessTokenRepository.SaveAsync();
                }

                throw new InvalidGrantException("Invalid authorization code.");
            }

            if (_code.ClientId != _request.ClientId) { throw new InvalidGrantException("Invalid client id."); }

            _application = await _findApplicationService.FindByClientIdAsync(_request.ClientId);
            if (_application.Type == ClientTypes.Confidential)
            {
                await _authenticateClientService.AuthenticateAsync(_request.ClientId, _request.ClientSecret);
            }

            if (!_application.AllowAuthorizationCodeGrant)
            {
                await _createAuditLogService.CreateForbiddenGrantTypeAuditLogAsync(GrantTypes.AuthorizationCode);

                throw new UnsupportedGrantTypeException("This authorization grant is not allowed for this application.");
            }

            if (!_application.AllowCredentialsInBody && _request.CredentialsInBody)
            {
                throw new InvalidGrantException("This application does not allow passing credentials in the request body.");
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
                Audience        = new List<string> { _code.ClientId.ToString() },
                ClientId        = _code.ClientId,
                Subject         = _code.UserId?.ToString(),
                Scope           = _code.Scope,
                LifetimeMinutes = _application.AccessTokenLifetimeMinutes,
            };

            _jwtToken = await _jwtTokenFactory.CreateJwtTokenAsync(jwtRequest);
        }

        private async Task GenerateRefreshTokenAsync()
        {
            if (!_application.GenerateRefreshTokenForAuthorizationCode) { return; }

            _refreshToken = await _refreshTokenGenerator.GenerateRefreshTokenAsync(
                _application.ApplicationId,
                _redirectUri,
                _code.UserId.GetValueOrDefault(),
                _code.Scope);

            _refreshToken.GrantType     = GrantTypes.AuthorizationCode;
            _refreshToken.AccessTokenId = _jwtToken.TokenId;
            _refreshToken.Code          = _code.Code;
            _jwtToken.RefreshToken      = _refreshToken.RefreshTokenId;
        }

        private async Task SaveAsync()
        {
            await _authorizationCodeRepository.SaveAsync();
            await _refreshTokenRepository.SaveAsync();
            await _accessTokenRepository.SaveAsync();
        }
    }
}
