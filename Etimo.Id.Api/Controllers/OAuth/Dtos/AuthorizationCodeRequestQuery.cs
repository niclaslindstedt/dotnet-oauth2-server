// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Constants;
using Etimo.Id.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Api.OAuth
{
    /// <summary>
    /// This is the request object used when retrieving the authorization code in the authorization code flow.
    /// https://tools.ietf.org/html/rfc6749#section-4.1.3
    /// </summary>
    public class AuthorizationCodeRequestQuery
    {
        [Required]
        [ValidValues("code")]
        public string response_type { get; set; }

        [Required]
        public Guid client_id { get; set; }

        [Regex("^https://", "The redirect_uri must use TLS encryption (https). Read more: https://tools.ietf.org/html/rfc6749#section-3.1.2.1")]
        [Regex("^[^#]+$", "The redirect_uri cannot use fragments. Read more: https://tools.ietf.org/html/rfc6749#section-3.1.2")]
        public string redirect_uri { get; set; }

        [Regex(CharacterSetPatterns.NQCHAR, "The scope field can only contain NQCHAR characters (%x21 / %x23-5B / %x5D-7E).")]
        public string scope { get; set; }

        public string state { get; set; }

        public AuthorizationRequest ToAuthorizeRequest(string username, string password)
        {
            var request = new AuthorizationRequest(response_type, client_id, state, scope, redirect_uri)
            {
                Username = username,
                Password = password
            };

            return request;
        }
    }
}
