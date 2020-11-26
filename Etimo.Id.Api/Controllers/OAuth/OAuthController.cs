using Etimo.Id.Abstractions;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Service.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Api.OAuth
{
    [ApiController]
    [Route("oauth2")]
    public class OAuthController : Controller
    {
        private readonly IOAuthService _oauthService;

        public OAuthController(IOAuthService oauthService)
        {
            _oauthService = oauthService;
        }

        [Authorize]
        [HttpGet]
        [Route("validate")]
        public void Validate()
        {
        }

        [HttpGet]
        [Route("authorize")]
        public async Task<IActionResult> AuthorizeAsync([FromQuery] AuthorizationCodeQuery query)
        {
            var request = query.ToAuthorizeRequest();
            var response = await _oauthService.StartAuthorizationCodeFlowAsync(request);

            return View("Authorize", $"/oauth2/authorize?{response.ToQueryParameters()}");
        }

        [HttpPost]
        [Route("authorize")]
        public async Task<IActionResult> AuthorizeAsync([FromQuery] AuthorizationCodeQuery query, [FromForm] AuthorizationCodeForm form)
        {
            if (Request.IsBasicAuthentication())
            {
                (form.username, form.password) = Request.GetBasicAuthenticationCredentials();
            }

            var request = query.ToAuthorizeRequest(form.username, form.password);
            var response = await _oauthService.FinishAuthorizationCodeAsync(request);

            return Redirect(response.ToString());
        }

        [HttpPost]
        [Route("token")]
        public async Task<AccessTokenResponseDto> TokenAsync([FromForm] AccessTokenRequestForm request)
        {
            if (Request.IsBasicAuthentication())
            {
                (request.client_id, request.client_secret) = Request.GetBasicAuthenticationCredentials();
            }

            if (request.client_id == null || request.client_secret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            var token = await _oauthService.GenerateTokenAsync(request.ToTokenRequest());

            return AccessTokenResponseDto.FromJwtToken(token);
        }
    }
}
