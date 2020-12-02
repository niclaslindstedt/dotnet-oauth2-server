using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Constants;
using Etimo.Id.Api.Helpers;
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
            var sb = new StringBuilder();
            sb.Append($"response_type={query.response_type}&");
            sb.Append($"client_id={query.client_id}&");
            if (query.redirect_uri != null) { sb.Append($"redirect_uri={query.redirect_uri}&"); }
            if (query.scope != null) { sb.Append($"scope={query.scope}&"); }
            if (query.state != null) { sb.Append($"state={query.state}&"); }

            Response.PreventClickjacking();

            return View("Authorize", $"/oauth2/authorize?{sb.ToString().Trim('&')}");
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
            var (clientId, clientSecret) = Request.GetCredentialsFromAuthorizationHeader();
            if (!Regex.IsMatch(clientSecret, CharacterSetPatterns.UNICODECHARNOCRLF))
            {
                throw new UnauthorizedException("Invalid client_secret.");
            }

            if (!Guid.TryParse(clientId, out var clientGuid))
            {
                throw new UnauthorizedException("Invalid client_id format; should be of type guid.");
            }

            request.ClientId = clientGuid;
            request.ClientSecret = clientSecret;

            var token = await _oauthService.GenerateTokenAsync(request);

            return Ok(AccessTokenResponseDto.FromJwtToken(token));
        }
    }
}
