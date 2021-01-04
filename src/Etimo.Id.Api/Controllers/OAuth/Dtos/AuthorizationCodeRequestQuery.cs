// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Attributes;
using Etimo.Id.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Api.OAuth
{
    /// <summary>
    ///     This is the request object used when retrieving the authorization code in the authorization code flow.
    ///     https://tools.ietf.org/html/rfc6749#section-4.1.3
    /// </summary>
    public class AuthorizationCodeRequestQuery
    {
        [Required]
        [ValidValues("code")]
        public string response_type { get; set; }

        [Required]
        public Guid? client_id { get; set; }

        [Required]
        [ValidUri]
        public string redirect_uri { get; set; }

        [NqsChar] // Should be NQCHAR but since we're splitting these values with a space, we need to allow space as well.
        public string scope { get; set; }

        [VsChar]
        public string state { get; set; }

        public AuthorizationRequest ToAuthorizeRequest(string username, string password)
        {
            var request = new AuthorizationRequest(
                    response_type,
                    client_id ?? Guid.Empty,
                    state,
                    scope,
                    redirect_uri)
                {
                    Username = username,
                    Password = password,
                };

            return request;
        }
    }
}
