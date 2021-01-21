using Etimo.Id.Constants;
using Etimo.Id.Dtos;
using Etimo.Id.Security;
using Etimo.Id.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Etimo.Id.Api.OAuth
{
    [ApiController]
    public class OpenIdController : Controller
    {
        private readonly EtimoIdSettings _settings;

        public OpenIdController(EtimoIdSettings settings)
        {
            _settings = settings;
        }

        [HttpGet]
        [Route("/.well-known/openid-configuration")]
        public IActionResult Validate()
        {
            string issuerUrl = _settings.Issuer.TrimEnd('/');

            // https://openid.net/specs/openid-connect-discovery-1_0.html#ProviderMetadata
            var dto = new OpenIdConfigurationDto
            {
                issuer                 = _settings.Issuer,
                authorization_endpoint = issuerUrl + "/oauth2/authorize",
                token_endpoint         = issuerUrl + "/oauth2/token",
                jwks_uri               = issuerUrl + "/openid/jwks",

                // registration_endpoint
                scopes_supported         = InbuiltScopes.All,
                response_types_supported = new List<string> { ResponseTypes.Code, ResponseTypes.Token },

                // response_modes_supported
                grant_types_supported = new List<string> { "authorization_code" },

                // acr_values_supported
                subject_types_supported = new List<string> { "public" },
                id_token_signing_alg_values_supported =
                    new List<string> { "RS256", "HS256" }, // deprecate HS256 after implementing asymmetric alg (RS256)
                // id_token_encryption_alg_values_supported
                // id_token_encryption_enc_values_supported
                // userinfo_signing_alg_values_supported
                // userinfo_encryption_alg_values_supported
                // userinfo_encryption_enc_values_supported
                // request_object_signing_alg_values_supported
                // request_object_encryption_alg_values_supported
                // request_object_encryption_enc_values_supported
                // token_endpoint_auth_methods_supported
                // token_endpoint_auth_signing_alg_values_supported
                // display_values_supported
                // claim_types_supported
                // claims_supported
                // service_documentation
                // claims_locales_supported
                // ui_locales_supported
                // claims_parameter_supported
                // request_parameter_supported
                // request_uri_parameter_supported
                // require_request_uri_registration
                // op_policy_uri
                // op_tos_uri
            };

            return Ok(dto);
        }
    }
}
