using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Errors;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Constants;
using Etimo.Id.Dtos;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Etimo.Id.Api.OAuth
{
    [ApiController]
    public class OAuthController : Controller
    {
        private readonly IAuthorizeService     _authorizeService;
        private readonly IGenerateTokenService _generateTokenService;
        private readonly IRequestContext       _requestContext;
        private readonly IValidateTokenService _validateTokenService;

        public OAuthController(
            IAuthorizeService authorizeService,
            IValidateTokenService validateTokenService,
            IGenerateTokenService generateTokenService,
            IRequestContext requestContext)
        {
            _authorizeService     = authorizeService;
            _validateTokenService = validateTokenService;
            _generateTokenService = generateTokenService;
            _requestContext       = requestContext;
        }

        [HttpGet]
        [Route("/oauth2/validate")]
        [Authorize]
        public async Task<IActionResult> Validate()
        {
            await _validateTokenService.ValidateTokenAsync(this.GetAccessTokenId());

            return NoContent();
        }

        [HttpGet]
        [Route("/oauth2/authorize")]
        [ValidateModel]
        public IActionResult AuthenticateResourceOwner([FromQuery] AuthorizationRequestQuery query)
        {
            Response.PreventClickjacking();

            string queryParams = query.ToQueryParameters();

            return View("Authorize", $"/oauth2/authorize?{queryParams}");
        }

        [HttpPost]
        [Route("/oauth2/authorize")]
        public async Task<IActionResult> AuthorizeAsync(
            [FromQuery] AuthorizationCodeRequestQuery query,
            [FromForm] AuthorizationCodeRequestForm form)
        {
            string redirectUri;
            try
            {
                if (!ModelState.IsValid)
                {
                    IEnumerable<string> errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    throw new InvalidRequestException(string.Join(" ", errorMessages));
                }

                if (Request.IsBasicAuthentication()) { (form.username, form.password) = Request.GetCredentialsFromAuthorizationHeader(); }

                _requestContext.ClientId = query.client_id;
                _requestContext.Username = form.username;

                AuthorizationRequest request = query.ToAuthorizeRequest(form.username, form.password);
                redirectUri = await _authorizeService.AuthorizeAsync(request);
            }
            catch (ErrorCodeException ex)
            {
                if (query.redirect_uri == null) { throw new InvalidRequestException("The redirect_uri field is required."); }

                redirectUri =  query.redirect_uri;
                redirectUri += query.redirect_uri.Contains("?") ? "&" : "?";
                redirectUri += $"error={Uri.EscapeDataString(ex.ErrorCode)}";

                if (!string.IsNullOrEmpty(ex.Message)) { redirectUri += $"&error_description={Uri.EscapeDataString(ex.Message)}"; }

                var errorUri = ex.GetStatusCode().GetStatusCodeUri().ToString();
                redirectUri += $"&error_uri={Uri.EscapeDataString(errorUri)}";

                if (query.state != null) { redirectUri += $"&state={Uri.EscapeDataString(query.state)}"; }
            }

            return Redirect(redirectUri);
        }

        [HttpPost]
        [Route("/oauth2/token")]
        [ValidateModel]
        [NoCache]
        public async Task<IActionResult> TokenAsync([FromForm] AccessTokenRequestForm form)
        {
            var request = form.ToTokenRequest();

            // https://tools.ietf.org/html/rfc6749#section-5.2
            if (Request.IsBasicAuthentication() && request.ClientSecret != null)
            {
                throw new InvalidRequestException("You cannot include multiple credentials (i.e. basic auth and credentials in body).");
            }

            if (!Request.IsBasicAuthentication())
            {
                string clientIdString = form.client_id != null ? form.client_id.ToString() : string.Empty;
                request.ClientId          = ParseClientId(clientIdString);
                request.ClientSecret      = form.client_secret;
                request.CredentialsInBody = form.client_secret != null;
            }
            else
            {
                (string clientId, string clientSecret) = Request.GetCredentialsFromAuthorizationHeader();
                if (!Regex.IsMatch(clientSecret, CharacterSetPatterns.UNICODECHARNOCRLF))
                {
                    throw new InvalidRequestException("client_secret contains illegal characters.");
                }

                request.ClientId     = ParseClientId(clientId);
                request.ClientSecret = clientSecret;
            }

            _requestContext.ClientId = form.client_id;

            JwtToken token = await _generateTokenService.GenerateTokenAsync(request);

            return Ok(AccessTokenResponseDto.FromJwtToken(token));
        }

        private static Guid ParseClientId(string clientId)
        {
            if (!Guid.TryParse(clientId, out Guid clientGuid) || clientGuid == Guid.Empty)
            {
                throw new InvalidClientException("client_id should be of type guid.");
            }

            return clientGuid;
        }
    }
}
