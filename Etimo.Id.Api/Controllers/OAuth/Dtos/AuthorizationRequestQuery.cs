// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Constants;
using Etimo.Id.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Api.OAuth
{
    /// <summary>
    /// These are the query parameters used when starting an authorization code flow (by authenticating the user).
    /// https://tools.ietf.org/html/rfc6749#section-4.1.1
    /// </summary>
    public class AuthorizationRequestQuery
    {
        [Required]
        [ValidValues("code")]
        public string response_type { get; set; }

        [Required]
        public Guid? client_id { get; set; }

        [ValidUri]
        public string redirect_uri { get; set; }

        [NqChar]
        public string scope { get; set; }
        public string state { get; set; }
    }
}
