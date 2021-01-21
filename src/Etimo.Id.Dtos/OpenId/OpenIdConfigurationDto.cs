using System.Collections.Generic;

namespace Etimo.Id.Dtos
{
    public class OpenIdConfigurationDto
    {
        public string       issuer                                           { get; set; }
        public string       authorization_endpoint                           { get; set; }
        public string       token_endpoint                                   { get; set; }
        public List<string> token_endpoint_auth_methods_supported            { get; set; }
        public string       token_endpoint_auth_signing_alg_values_supported { get; set; }
        public string       userinfo_endpoint                                { get; set; }
        public string       check_session_iframe                             { get; set; }
        public string       end_session_endpoint                             { get; set; }
        public string       jwks_uri                                         { get; set; }
        public string       registration_endpoint                            { get; set; }
        public List<string> grant_types_supported                            { get; set; }
        public List<string> scopes_supported                                 { get; set; }
        public List<string> response_types_supported                         { get; set; }
        public List<string> acr_values_supported                             { get; set; }
        public List<string> subject_types_supported                          { get; set; }
        public List<string> userinfo_signing_alg_values_supported            { get; set; }
        public List<string> userinfo_encryption_alg_values_supported         { get; set; }
        public List<string> userinfo_encryption_enc_values_supported         { get; set; }
        public List<string> id_token_signing_alg_values_supported            { get; set; }
        public List<string> id_token_encryption_alg_values_supported         { get; set; }
        public List<string> id_token_encryption_enc_values_supported         { get; set; }
        public List<string> request_object_signing_alg_values_supported      { get; set; }
        public List<string> display_values_supported                         { get; set; }
        public List<string> claim_types_supported                            { get; set; }
        public List<string> claims_supported                                 { get; set; }
        public bool         claims_parameter_supported                       { get; set; }
        public string       service_documentation                            { get; set; }
        public List<string> ui_locales_supported                             { get; set; }
    }
}
