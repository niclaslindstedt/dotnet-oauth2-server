using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Constants;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Etimo.Id.Api.OAuth
{
    [ApiController]
    public class OAuthController : Controller
    {
        private readonly IOAuthService _oauthService;

        public OAuthController(IOAuthService oauthService)
        {
            _oauthService = oauthService;
        }

        [HttpGet]
        [Route("/oauth2/validate")]
        [Authorize]
        public IActionResult Validate()
        {
            return NoContent();
        }

        [HttpGet]
        [Route("/oauth2/authorize")]
        [ValidateModel]
        public IActionResult AuthenticateResourceOwner([FromQuery] AuthorizationRequestQuery query)
        {
            Response.PreventClickjacking();

            var queryParams = query.ToQueryParameters();

            return View("Authorize", $"/oauth2/authorize?{queryParams}");
        }

        [HttpPost]
        [Route("/oauth2/authorize")]
        [ValidateModel]
        public async Task<IActionResult> AuthorizeAsync([FromQuery] AuthorizationCodeRequestQuery query, [FromForm] AuthorizationCodeRequestForm form)
        {
            if (Request.IsBasicAuthentication())
            {
                (form.username, form.password) = Request.GetCredentialsFromAuthorizationHeader();
            }

            var request = query.ToAuthorizeRequest(form.username, form.password);
            var redirectUri = await _oauthService.AuthorizeAsync(request);

            return Redirect(redirectUri);
        }

        [HttpPost]
        [Route("/oauth2/token")]
        [ValidateModel]
        [NoCache]
        public async Task<IActionResult> TokenAsync([FromForm] AccessTokenRequestForm form)
        {
            var request = form.ToTokenRequest();

            // This is likely a public client trying to create a token through the authorization code flow
            if (request.GrantType == GrantTypes.AuthorizationCode && !Request.IsBasicAuthentication())
            {
                var clientIdString = form.client_id != null ? form.client_id.ToString() : string.Empty;
                request.ClientId = ParseClientId(clientIdString);
            }
            else
            {
                var (clientId, clientSecret) = Request.GetCredentialsFromAuthorizationHeader();
                if (!Regex.IsMatch(clientSecret, CharacterSetPatterns.UNICODECHARNOCRLF))
                {
                    throw new UnauthorizedException("Invalid client_secret.");
                }

                request.ClientId = ParseClientId(clientId);
                request.ClientSecret = clientSecret;
            }

            var token = await _oauthService.GenerateTokenAsync(request);

            return Ok(AccessTokenResponseDto.FromJwtToken(token));
        }

        private static Guid ParseClientId(string clientId)
        {
            if (!Guid.TryParse(clientId, out var clientGuid) || clientGuid == Guid.Empty)
            {
                throw new UnauthorizedException("Invalid client_id format; should be of type guid.");
            }

            return clientGuid;
        }
    }
}
