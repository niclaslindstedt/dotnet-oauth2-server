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
    public class AuthorizationCodeTokenGenerator : IAuthorizationCodeTokenGenerator
    {
        private readonly IAuthenticateClientService _authenticateClientService;
        private readonly IFindApplicationService _findApplicationService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;
        private readonly IRequestContext _requestContext;

        private IAuthorizationCodeTokenRequest _request;
        private JwtToken _jwtToken;
        private RefreshToken _refreshToken;
        private AuthorizationCode _code;
        private Application _application;
        private string _redirectUri;

        public AuthorizationCodeTokenGenerator(
            IAuthenticateClientService authenticateClientService,
            IFindApplicationService applicationService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IAccessTokenRepository accessTokenRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtTokenFactory jwtTokenFactory,
            IRequestContext requestContext)
        {
            _authenticateClientService = authenticateClientService;
            _findApplicationService = applicationService;
            _refreshTokenGenerator = refreshTokenGenerator;
            _authorizationCodeRepository = authorizationCodeRepository;
            _accessTokenRepository = accessTokenRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenFactory = jwtTokenFactory;
            _requestContext = requestContext;
        }

        public async Task<JwtToken> GenerateTokenAsync(IAuthorizationCodeTokenRequest request)
        {
            _request = request;

            UpdateContext();
            await ValidateRequestAsync();
            await CreateJwtTokenAsync();
            GenerateRefreshToken();

            _code.Used = true;
            _code.AccessTokenId = _jwtToken.TokenId;
            _jwtToken.RefreshToken = _refreshToken.RefreshTokenId;

            var accessToken = _jwtToken.ToAccessToken();
            _accessTokenRepository.Add(accessToken);

            await SaveAsync();

            return _jwtToken;
        }

        private void UpdateContext()
        {
            _requestContext.ClientId = _request.ClientId;
        }

        private async Task ValidateRequestAsync()
        {

            if (_request.ClientId == Guid.Empty)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            if (_request.Code == null)
            {
                throw new InvalidGrantException("Invalid authorization code.");
            }

            _code = await _authorizationCodeRepository.FindAsync(_request.Code);
            if (_code?.UserId == null || _code.IsExpired)
            {
                throw new InvalidGrantException("Invalid authorization code.");
            }

            // If someone tries to use the same authorization code twice, disable the access token.
            if (_code.Used)
            {
                if (_code.AccessToken != null)
                {
                    _code.AccessToken.Disabled = true;
                    await _accessTokenRepository.SaveAsync();
                }

                throw new InvalidGrantException("Invalid authorization code.");
            }

            if (_code.ClientId != _request.ClientId)
            {
                throw new InvalidGrantException("Invalid client id.");
            }

            _application = await _findApplicationService.FindByClientIdAsync(_request.ClientId);
            if (_application.Type == ClientTypes.Confidential)
            {
                await _authenticateClientService.AuthenticateAsync(_request.ClientId, _request.ClientSecret);
            }

            _redirectUri = _request.RedirectUri ?? _application.RedirectUri;
            if (_redirectUri != _application.RedirectUri)
            {
                throw new InvalidGrantException("The provided redirect URI does not match the one on record.");
            }
        }

        private async Task CreateJwtTokenAsync()
        {
            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { _code.ClientId.ToString() },
                Subject = _code.UserId?.ToString(),
                Scope = _code.Scope
            };

            _jwtToken = await _jwtTokenFactory.CreateJwtTokenAsync(jwtRequest);
        }

        private void GenerateRefreshToken()
        {
            _refreshToken = _refreshTokenGenerator.GenerateRefreshToken(
                _application.ApplicationId,
                _redirectUri,
                _code.UserId.GetValueOrDefault(),
                _code.Scope);

            _refreshToken.AccessTokenId = _jwtToken.TokenId;
            _refreshToken.Code = _code.Code;
        }

        private async Task SaveAsync()
        {
            await _authorizationCodeRepository.SaveAsync();
            await _refreshTokenRepository.SaveAsync();
            await _accessTokenRepository.SaveAsync();
        }
    }
}
