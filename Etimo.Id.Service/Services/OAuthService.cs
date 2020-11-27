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
        private readonly IAuthorizationCodeTokenGenerator _authorizationCodeTokenGenerator;
        private readonly IClientCredentialsTokenGenerator _clientCredentialsTokenGenerator;
        private readonly IResourceOwnerCredentialsTokenGenerator _resourceOwnerCredentialsTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IPasswordGenerator _passwordGenerator;

        public OAuthService(
            IApplicationsService applicationsService,
            IUsersService usersService,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IAuthorizationCodeTokenGenerator authorizationCodeTokenGenerator,
            IClientCredentialsTokenGenerator clientCredentialsTokenGenerator,
            IResourceOwnerCredentialsTokenGenerator resourceOwnerCredentialsTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            IPasswordGenerator passwordGenerator)
        {
            _applicationsService = applicationsService;
            _usersService = usersService;
            _authorizationCodeRepository = authorizationCodeRepository;
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

        public async Task<AuthorizationResponse> StartAuthorizationCodeFlowAsync(AuthorizationRequest request)
        {
            if (request.ResponseType != "code")
            {
                if (request.ResponseType == "token")
                {
                    throw new UnsupportedResponseTypeException("The implicit grant type should no longer be used. Read more: https://tools.ietf.org/html/draft-ietf-oauth-security-topics-16#section-2.1.2");
                }

                throw new UnsupportedResponseTypeException("Invalid response type");
            }

            if (request.ClientId == null)
            {
                throw new InvalidClientException("Client ID is missing from request.");
            }

            var clientId = request.ClientId.Value;
            var application = await _applicationsService.FindByClientIdAsync(clientId);

            var redirectUri = request.RedirectUri ?? application.RedirectUri;
            if (redirectUri != application.RedirectUri)
            {
                throw new InvalidClientException("The provided redirect URI does not match the one on record.");
            }

            var code = await GenerateAuthorizationCodeAsync(clientId, redirectUri);

            return new AuthorizationResponse
            {
                ResponseType = "code",
                ClientId = clientId,
                State = request.State,
                RedirectUri = code.RedirectUri,
                AuthorizationCodeId = code.AuthorizationCodeId
            };
        }

        public async Task<string> FinishAuthorizationCodeAsync(AuthorizationRequest request)
        {
            if (request.ClientId == null)
            {
                throw new InvalidClientException("Client ID is missing from request.");
            }

            if (request.AuthorizationCodeId == null)
            {
                throw new InvalidGrantException("Invalid authorization code ID.");
            }

            if (request.Username == null || request.Password == null)
            {
                throw new InvalidGrantException("Invalid user credentials.");
            }

            var code = await _authorizationCodeRepository.FindAsync(request.AuthorizationCodeId.Value);
            if (code == null || code.IsExpired)
            {
                throw new InvalidGrantException("Invalid authorization code.");
            }

            var user = await _usersService.AuthenticateAsync(request.Username, request.Password);

            code.Authorized = true;
            code.UserId = user.UserId;
            await _authorizationCodeRepository.SaveAsync();

            return $"{code.RedirectUri}?code={code.Code}&state={request.State}";
        }

        private async Task<AuthorizationCode> GenerateAuthorizationCodeAsync(Guid clientId, string redirectUri)
        {
            var code = new AuthorizationCode
            {
                Code = _passwordGenerator.Generate(32),
                ExpirationDate = DateTime.UtcNow.AddMinutes(10),
                ClientId = clientId,
                RedirectUri = redirectUri
            };
            _authorizationCodeRepository.Add(code);
            await _authorizationCodeRepository.SaveAsync();

            return code;
        }
    }
}
