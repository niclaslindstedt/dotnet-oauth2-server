using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Settings;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IUsersService _usersService;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IAccessTokensRepository _accessTokensRepository;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly OAuth2Settings _settings;

        public AuthorizationService(
            IApplicationsService applicationsService,
            IUsersService usersService,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IAccessTokensRepository accessTokensRepository,
            IPasswordGenerator passwordGenerator,
            OAuth2Settings settings)
        {
            _applicationsService = applicationsService;
            _usersService = usersService;
            _authorizationCodeRepository = authorizationCodeRepository;
            _accessTokensRepository = accessTokensRepository;
            _passwordGenerator = passwordGenerator;
            _settings = settings;
        }

        public async Task<string> AuthorizeAsync(AuthorizationRequest request)
        {
            if (request.ResponseType != "code")
            {
                throw new UnsupportedResponseTypeException("The only supported response type is 'code'.");
            }

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
                Code = _passwordGenerator.Generate(_settings.AuthorizationCodeLength),
                ExpirationDate = DateTime.UtcNow.AddMinutes(_settings.AuthorizationCodeLifetimeMinutes),
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
