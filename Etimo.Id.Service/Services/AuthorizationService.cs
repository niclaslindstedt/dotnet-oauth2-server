using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IApplicationService _applicationService;
        private readonly IUserService _userService;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IAccessTokensRepository _accessTokensRepository;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly OAuth2Settings _settings;

        private IAuthorizationRequest _request;
        private AuthorizationCode _code;
        private User _user;

        public AuthorizationService(
            IApplicationService applicationService,
            IUserService userService,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IAccessTokensRepository accessTokensRepository,
            IPasswordGenerator passwordGenerator,
            OAuth2Settings settings)
        {
            _applicationService = applicationService;
            _userService = userService;
            _authorizationCodeRepository = authorizationCodeRepository;
            _accessTokensRepository = accessTokensRepository;
            _passwordGenerator = passwordGenerator;
            _settings = settings;
        }

        public async Task<string> AuthorizeAsync(IAuthorizationRequest request)
        {
            await ValidateRequestAsync(request);
            await AuthenticateUserAsync();
            await GenerateAuthorizationCodeAsync();

            return GenerateAuthorizationUrl();
        }

        public async Task ValidateAsync(Guid accessTokenId)
        {
            var accessToken = await _accessTokensRepository.FindAsync(accessTokenId);
            if (accessToken == null || accessToken.Disabled)
            {
                throw new UnauthorizedException("Access token has been disabled.");
            }
        }

        private async Task ValidateRequestAsync(IAuthorizationRequest request)
        {
            _request = request;

            if (_request.ResponseType != "code")
            {
                throw new UnsupportedResponseTypeException("The only supported response type is 'code'.");
            }

            var application = await _applicationService.FindByClientIdAsync(_request.ClientId);
            if (application == null)
            {
                throw new InvalidClientException("No application with that client ID could be found.");
            }

            // Make sure the provided scopes actually exists within this application.
            if (_request.Scope != null)
            {
                var scopes = _request.Scope.Split(" ");
                foreach (var scope in scopes)
                {
                    if (application.Scopes.All(s => s.Name != scope))
                    {
                        throw new InvalidScopeException("The provided scope is invalid.");
                    }
                }
            }

            // Make sure the provided redirect uri is identical to the registered redirect uri.
            var redirectUri = _request.RedirectUri ?? application.RedirectUri;
            if (redirectUri != application.RedirectUri)
            {
                throw new InvalidGrantException("The provided redirect URI does not match the one on record.");
            }
        }

        private async Task AuthenticateUserAsync()
        {
            _user = await _userService.AuthenticateAsync(_request.Username, _request.Password);
        }

        private async Task GenerateAuthorizationCodeAsync()
        {
            _code = new AuthorizationCode
            {
                Code = _passwordGenerator.Generate(_settings.AuthorizationCodeLength),
                ExpirationDate = DateTime.UtcNow.AddMinutes(_settings.AuthorizationCodeLifetimeMinutes),
                ClientId = _request.ClientId,
                UserId = _user.UserId,
                RedirectUri = _request.RedirectUri,
                Scope = _request.Scope
            };
            _authorizationCodeRepository.Add(_code);
            await _authorizationCodeRepository.SaveAsync();
        }

        private string GenerateAuthorizationUrl()
        {
            var delimiter = _code.RedirectUri.Contains("?") ? "&" : "?";

            return $"{_code.RedirectUri}{delimiter}code={_code.Code}&state={_request.State}";
        }
    }
}
