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
        private readonly IClientCredentialsTokenGenerator _clientCredentialsTokenGenerator;
        private readonly IPasswordTokenGenerator _passwordTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IPasswordGenerator _passwordGenerator;

        public OAuthService(
            IApplicationsService applicationsService,
            IUsersService usersService,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IClientCredentialsTokenGenerator clientCredentialsTokenGenerator,
            IPasswordTokenGenerator passwordTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            IPasswordGenerator passwordGenerator)
        {
            _applicationsService = applicationsService;
            _usersService = usersService;
            _authorizationCodeRepository = authorizationCodeRepository;
            _clientCredentialsTokenGenerator = clientCredentialsTokenGenerator;
            _passwordTokenGenerator = passwordTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _passwordGenerator = passwordGenerator;
        }

        public Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            ITokenGenerator generator = request.GrantType switch
            {
                GrantTypes.ClientCredentials => _clientCredentialsTokenGenerator,
                GrantTypes.Password => _passwordTokenGenerator,
                GrantTypes.RefreshToken => _refreshTokenGenerator,
                _ => throw new InvalidRequestException("Grant type not supported.")
            };

            return generator.GenerateTokenAsync(request);
        }

        public async Task<AuthorizationResponse> StartAuthorizationCodeFlowAsync(AuthorizationRequest request)
        {
            if (request.ClientId == null)
            {
                throw new InvalidClientException("Client ID is missing from request.");
            }

            var clientId = request.ClientId.Value;
            var application = await _applicationsService.FindByClientIdAsync(clientId);

            var redirectUri = request.RedirectUri ?? application.RedirectUri;
            if (redirectUri != application.RedirectUri)
            {
                throw new InvalidClientException("The given redirect URI does not match the one on record.");
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

        public async Task<Uri> FinishAuthorizationCodeAsync(AuthorizationRequest request)
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

            await _usersService.AuthenticateAsync(request.Username, request.Password);

            var code = await _authorizationCodeRepository.FindAsync(request.AuthorizationCodeId.Value);
            if (code.IsExpired)
            {
                throw new InvalidGrantException("Invalid authorization code.");
            }

            code.Authorized = true;
            await _authorizationCodeRepository.SaveAsync();

            return new Uri($"{code.RedirectUri}?code={code.Code}&state={request.State}");
        }
    }
}
