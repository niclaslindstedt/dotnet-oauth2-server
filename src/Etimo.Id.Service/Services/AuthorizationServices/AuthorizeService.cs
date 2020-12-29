using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Scopes;
using Etimo.Id.Service.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IAuthenticateUserService     _authenticateUserService;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IFindApplicationService      _findApplicationService;
        private readonly IPasswordGenerator           _passwordGenerator;
        private readonly OAuth2Settings               _settings;
        private          string                       _allScopes;
        private          AuthorizationCode            _code;

        private IAuthorizationRequest _request;
        private User                  _user;

        public AuthorizeService(
            IFindApplicationService findApplicationService,
            IAuthenticateUserService authenticateUserService,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IPasswordGenerator passwordGenerator,
            OAuth2Settings settings)
        {
            _findApplicationService      = findApplicationService;
            _authenticateUserService     = authenticateUserService;
            _authorizationCodeRepository = authorizationCodeRepository;
            _passwordGenerator           = passwordGenerator;
            _settings                    = settings;
        }

        public async Task<string> AuthorizeAsync(IAuthorizationRequest request)
        {
            _request = request;

            await ValidateRequestAsync();
            await AuthenticateUserAsync();
            VerifyScopesAreValid();
            await GenerateAuthorizationCodeAsync();

            return GenerateAuthorizationUrl();
        }

        private async Task ValidateRequestAsync()
        {
            if (_request.ResponseType != "code")
            {
                throw new UnsupportedResponseTypeException("The only supported response type is 'code'.");
            }

            Application application = await _findApplicationService.FindByClientIdAsync(_request.ClientId);
            if (application == null)
            {
                throw new InvalidClientException("No application with that client ID could be found.", _request.State);
            }

            // Make sure the provided scopes actually exists within this application.
            IEnumerable<string> allScopes = InbuiltScopes.All.Concat(application.Scopes.Select(s => s.Name));
            if (_request.Scope != null)
            {
                string[] scopes = _request.Scope.Split(" ");
                foreach (string scope in scopes)
                {
                    if (!allScopes.Contains(scope)) { throw new InvalidScopeException("The provided scope is invalid.", _request.State); }
                }
            }

            _allScopes = string.Join(" ", allScopes);

            // Make sure the provided redirect uri is identical to the registered redirect uri.
            string redirectUri = _request.RedirectUri ?? application.RedirectUri;
            if (redirectUri != application.RedirectUri)
            {
                throw new InvalidGrantException("The provided redirect URI does not match the one on record.", _request.State);
            }
        }

        private async Task AuthenticateUserAsync()
            => _user = await _authenticateUserService.AuthenticateAsync(_request);

        private void VerifyScopesAreValid()
        {
            if (_request.Scope == null) { return; }

            string[] requestedScopes = _request.Scope.Split(" ");
            var      availableScopes = _user.Roles.SelectMany(r => r.Scopes).Select(s => s.Name).ToList();

            if (!requestedScopes.All(s => availableScopes.Contains(s)))
            {
                throw new ForbiddenException("You do not have access to that scope.");
            }
        }

        private async Task GenerateAuthorizationCodeAsync()
        {
            _code = new AuthorizationCode
            {
                Code           = _passwordGenerator.Generate(_settings.AuthorizationCodeLength),
                ExpirationDate = DateTime.UtcNow.AddMinutes(_settings.AuthorizationCodeLifetimeMinutes),
                ClientId       = _request.ClientId,
                UserId         = _user.UserId,
                RedirectUri    = _request.RedirectUri,
                Scope          = _request.Scope ?? _allScopes,
            };
            _authorizationCodeRepository.Add(_code);
            await _authorizationCodeRepository.SaveAsync();
        }

        private string GenerateAuthorizationUrl()
        {
            string delimiter = _code.RedirectUri.Contains("?") ? "&" : "?";
            string code      = Uri.EscapeDataString(_code.Code);
            var    sb        = new StringBuilder($"{_code.RedirectUri}{delimiter}code={code}");
            if (_request.State != null)
            {
                string state = Uri.EscapeDataString(_request.State);
                sb.Append($"&state={state}");
            }

            return sb.ToString();
        }
    }
}
