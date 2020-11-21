// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;

namespace Etimo.Id.Api.Models
{
    public class TokenRequestForm
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }

        public TokenRequest ToTokenRequest()
        {
            return new TokenRequest
            {
                GrantType = grant_type,
                ClientId = client_id,
                ClientSecret = client_secret
            };
        }
    }
}
