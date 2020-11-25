using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class OAuthService : IOAuthService
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IClientCredentialsTokenGenerator _clientCredentialsTokenGenerator;
        private readonly IPasswordTokenGenerator _passwordTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public OAuthService(
            IApplicationsService applicationsService,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IClientCredentialsTokenGenerator clientCredentialsTokenGenerator,
            IPasswordTokenGenerator passwordTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator)
        {
            _applicationsService = applicationsService;
            _authorizationCodeRepository = authorizationCodeRepository;
            _clientCredentialsTokenGenerator = clientCredentialsTokenGenerator;
            _passwordTokenGenerator = passwordTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
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

            if (request.RedirectUri != null && application.RedirectUri != request.RedirectUri)
            {
                throw new InvalidClientException("The given redirect URI does not match the one on record.");
            }

            var code = new AuthorizationCode { ClientId = clientId };
            _authorizationCodeRepository.Add(code);
            await _authorizationCodeRepository.SaveAsync();

            return new AuthorizationResponse
            {
                Code = code.AuthorizationCodeId.ToString(),
                State = request.State,
                RedirectUri = request.RedirectUri ?? application.RedirectUri,
            };
        }
    }
}
