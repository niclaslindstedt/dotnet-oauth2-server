using Etimo.Id.Abstractions;
using Etimo.Id.Constants;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Exceptions;
using Etimo.Id.Security;
using Etimo.Id.Service.Utilities;
using Etimo.Id.Settings;
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
        private readonly IImplicitTokenGenerator      _implicitTokenGenerator;
        private readonly IPasswordGenerator           _passwordGenerator;
        private readonly OAuth2Settings               _settings;
        private readonly IVerifyScopeService          _verifyScopeService;
        private          string                       _allScopes;
        private          Application                  _application;
        private          AuthorizationCode            _code;

        private IAuthorizationRequest _request;
        private User                  _user;

        public AuthorizeService(
            IFindApplicationService findApplicationService,
            IAuthenticateUserService authenticateUserService,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IImplicitTokenGenerator implicitTokenGenerator,
            IPasswordGenerator passwordGenerator,
            IVerifyScopeService verifyScopeService,
            OAuth2Settings settings)
        {
            _findApplicationService      = findApplicationService;
            _authenticateUserService     = authenticateUserService;
            _authorizationCodeRepository = authorizationCodeRepository;
            _implicitTokenGenerator      = implicitTokenGenerator;
            _passwordGenerator           = passwordGenerator;
            _verifyScopeService          = verifyScopeService;
            _settings                    = settings;
        }

        public async Task<string> AuthorizeAsync(IAuthorizationRequest request)
        {
            _request = request;

            await ValidateRequestAsync();
            await AuthenticateUserAsync();
            _verifyScopeService.Verify(request.Scope, _user);

            switch (request.ResponseType)
            {
                case ResponseTypes.Code:
                    await GenerateAuthorizationCodeAsync();
                    return GenerateAuthorizationCodeUrl();

                case ResponseTypes.Token: return await GenerateImplicitUrl();

                default: throw new UnsupportedResponseTypeException("Unsupported response type.");
            }
        }

        private async Task ValidateRequestAsync()
        {
            _application = await _findApplicationService.FindByClientIdAsync(_request.ClientId);
            if (_application == null)
            {
                throw new InvalidClientException("No application with that client ID could be found.", _request.State);
            }

            // Make sure the provided scopes actually exists within this application.
            IEnumerable<string> allScopes = InbuiltScopes.All.Concat(_application.Scopes.Select(s => s.Name)).Distinct().OrderBy(s => s);
            if (_request.Scope != null)
            {
                string[] scopes = _request.Scope.Split(" ");
                foreach (string scope in scopes)
                {
                    if (!allScopes.Contains(scope)) { throw new InvalidScopeException("The provided scope is invalid.", _request.State); }
                }
            }

            _allScopes = string.Join(" ", allScopes);

            var redirectUris = _application.RedirectUri.Split(" ").ToList();
            if (_request.RedirectUri == null && redirectUris.Count() > 1)
            {
                throw new InvalidGrantException("The provided redirect URI does not match the one on record.");
            }

            if (!RedirectUriHelper.UriMatches(_request.RedirectUri, redirectUris, _application.AllowCustomQueryParametersInRedirectUri))
            {
                throw new InvalidGrantException("The provided redirect URI does not match the one on record.");
            }
        }

        private async Task AuthenticateUserAsync()
            => _user = await _authenticateUserService.AuthenticateAsync(_request);

        private async Task GenerateAuthorizationCodeAsync()
        {
            _code = new AuthorizationCode
            {
                Code           = _passwordGenerator.Generate(_settings.AuthorizationCodeLength),
                ExpirationDate = DateTime.UtcNow.AddSeconds(_application.AuthorizationCodeLifetimeSeconds),
                ClientId       = _request.ClientId,
                UserId         = _user.UserId,
                RedirectUri    = _request.RedirectUri,
                Scope          = _request.Scope ?? _allScopes,
            };
            _authorizationCodeRepository.Add(_code);
            await _authorizationCodeRepository.SaveAsync();
        }

        private string GenerateAuthorizationCodeUrl()
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

        private async Task<string> GenerateImplicitUrl()
        {
            JwtToken jwtToken = await _implicitTokenGenerator.GenerateTokenAsync(_request);
            var      sb       = new StringBuilder($"{_request.RedirectUri}#");
            sb.Append($"access_token={jwtToken.AccessToken}");
            if (_request.State != null)
            {
                string state = Uri.EscapeDataString(_request.State);
                sb.Append($"&state={state}");
            }

            sb.Append($"&token_type={jwtToken.TokenType}");
            sb.Append($"&expires_in={jwtToken.ExpiresIn}");

            return sb.ToString();
        }
    }
}
