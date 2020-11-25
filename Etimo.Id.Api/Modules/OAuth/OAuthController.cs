using Etimo.Id.Abstractions;
using Etimo.Id.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Etimo.Id.Service.Exceptions;

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

        [HttpPost]
        [Route("token")]
        public async Task<TokenResponseDto> TokenAsync([FromForm] TokenRequestForm req)
        {
            if (Request.IsBasicAuthentication())
            {
                (req.client_id, req.client_secret) = Request.GetBasicAuthenticationCredentials();
            }

            if (req.client_id == null || req.client_secret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            var token = await _oauthService.GenerateTokenAsync(req.ToTokenRequest());

            return TokenResponseDto.FromJwtToken(token);
        }
    }
}
