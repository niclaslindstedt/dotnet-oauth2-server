using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class OAuthService : IOAuthService
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IUsersService _usersService;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IAccessTokensRepository _accessTokensRepository;
        private readonly IAuthorizationCodeTokenGenerator _authorizationCodeTokenGenerator;
        private readonly IClientCredentialsTokenGenerator _clientCredentialsTokenGenerator;
        private readonly IResourceOwnerCredentialsTokenGenerator _resourceOwnerCredentialsTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IPasswordGenerator _passwordGenerator;

        public OAuthService(
            IApplicationsService applicationsService,
            IUsersService usersService,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IAccessTokensRepository accessTokensRepository,
            IAuthorizationCodeTokenGenerator authorizationCodeTokenGenerator,
            IClientCredentialsTokenGenerator clientCredentialsTokenGenerator,
            IResourceOwnerCredentialsTokenGenerator resourceOwnerCredentialsTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            IPasswordGenerator passwordGenerator)
        {
            _applicationsService = applicationsService;
            _usersService = usersService;
            _authorizationCodeRepository = authorizationCodeRepository;
            _accessTokensRepository = accessTokensRepository;
            _authorizationCodeTokenGenerator = authorizationCodeTokenGenerator;
            _clientCredentialsTokenGenerator = clientCredentialsTokenGenerator;
            _resourceOwnerCredentialsTokenGenerator = resourceOwnerCredentialsTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _passwordGenerator = passwordGenerator;
        }

        public Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            return request.GrantType switch
            {
                GrantTypes.AuthorizationCode => _authorizationCodeTokenGenerator.GenerateTokenAsync(request),
                GrantTypes.ClientCredentials => _clientCredentialsTokenGenerator.GenerateTokenAsync(request),
                GrantTypes.Password => _resourceOwnerCredentialsTokenGenerator.GenerateTokenAsync(request),
                GrantTypes.RefreshToken => _refreshTokenGenerator.GenerateTokenAsync(request),
                _ => throw new UnsupportedGrantTypeException("Grant type not supported.")
            };
        }

        public async Task<string> AuthorizeAsync(AuthorizationRequest request)
        {
            var application = await _applicationsService.FindByClientIdAsync(request.ClientId);
            if (application == null)
            {
                throw new InvalidClientException("No application with that client ID could be found.");
            }

            // Make sure the provided redirect uri is identical to the registered redirect uri.
            var redirectUri = request.RedirectUri ?? application.RedirectUri;
            if (redirectUri != application.RedirectUri)
            {
                throw new InvalidGrantException("The provided redirect URI does not match the one on record.");
            }

            var user = await _usersService.AuthenticateAsync(request.Username, request.Password);
            var code = await GenerateAuthorizationCodeAsync(user.UserId, request.ClientId, request.RedirectUri);

            var delimiter = code.RedirectUri.Contains("?") ? "&" : "?";

            return $"{code.RedirectUri}{delimiter}code={code.Code}&state={request.State}";
        }

        public async Task ValidateAsync(Guid accessTokenId)
        {
            var accessToken = await _accessTokensRepository.FindAsync(accessTokenId);
            if (accessToken == null || accessToken.Disabled)
            {
                throw new UnauthorizedException("Access token has been disabled.");
            }
        }

        private async Task<AuthorizationCode> GenerateAuthorizationCodeAsync(Guid userId, Guid clientId, string redirectUri)
        {
            var code = new AuthorizationCode
            {
                Code = _passwordGenerator.Generate(32),
                ExpirationDate = DateTime.UtcNow.AddMinutes(10),
                ClientId = clientId,
                UserId = userId,
                RedirectUri = redirectUri
            };
            _authorizationCodeRepository.Add(code);
            await _authorizationCodeRepository.SaveAsync();

            return code;
        }
    }
}
