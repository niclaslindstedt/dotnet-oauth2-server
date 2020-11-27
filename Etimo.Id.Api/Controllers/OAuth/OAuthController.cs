using Etimo.Id.Abstractions;
using Etimo.Id.Api.Helpers;
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
            var redirectUri = await _oauthService.FinishAuthorizationCodeAsync(request);

            return Redirect(redirectUri);
        }

        [HttpPost]
        [Route("token")]
        public async Task<AccessTokenResponseDto> TokenAsync([FromForm] AccessTokenRequestForm form)
        {
            if (Request.IsBasicAuthentication())
            {
                (form.client_id, form.client_secret) = Request.GetBasicAuthenticationCredentials();
            }

            var request = form.ToTokenRequest();
            var token = await _oauthService.GenerateTokenAsync(request);

            return AccessTokenResponseDto.FromJwtToken(token);
        }
    }
}
