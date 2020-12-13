using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Scopes;
using Etimo.Id.Service.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IFindApplicationService _findApplicationService;
        private readonly IAuthenticateUserService _authenticateUserService;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly OAuth2Settings _settings;

        private IAuthorizationRequest _request;
        private AuthorizationCode _code;
        private User _user;

        public AuthorizeService(
            IFindApplicationService findApplicationService,
            IAuthenticateUserService authenticateUserService,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IPasswordGenerator passwordGenerator,
            OAuth2Settings settings)
        {
            _findApplicationService = findApplicationService;
            _authenticateUserService = authenticateUserService;
            _authorizationCodeRepository = authorizationCodeRepository;
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

        private async Task ValidateRequestAsync(IAuthorizationRequest request)
        {
            _request = request;

            if (_request.ResponseType != "code")
            {
                throw new UnsupportedResponseTypeException("The only supported response type is 'code'.");
            }

            var application = await _findApplicationService.FindByClientIdAsync(_request.ClientId);
            if (application == null)
            {
                throw new InvalidClientException("No application with that client ID could be found.", request.State);
            }

            // Make sure the provided scopes actually exists within this application.
            if (_request.Scope != null)
            {
                var scopes = _request.Scope.Split(" ");
                foreach (var scope in scopes)
                {
                    var allScopes = InbuiltScopes.All.Concat(application.Scopes.Select(s => s.Name));
                    if (!allScopes.Contains(scope))
                    {
                        throw new InvalidScopeException("The provided scope is invalid.", request.State);
                    }
                }
            }

            // Make sure the provided redirect uri is identical to the registered redirect uri.
            var redirectUri = _request.RedirectUri ?? application.RedirectUri;
            if (redirectUri != application.RedirectUri)
            {
                throw new InvalidGrantException("The provided redirect URI does not match the one on record.", request.State);
            }
        }

        private async Task AuthenticateUserAsync()
        {
            _user = await _authenticateUserService.AuthenticateAsync(_request);
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
