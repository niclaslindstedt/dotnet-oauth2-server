// ReSharper disable InconsistentNaming

using Etimo.Id.Attributes;
using Etimo.Id.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Etimo.Id.Dtos
{
    /// <summary>
    ///     These are the query parameters used when starting an authorization code flow (by authenticating the user).
    ///     https://tools.ietf.org/html/rfc6749#section-4.1.1
    /// </summary>
    public class AuthorizationRequestQuery
    {
        [Required]
        [ValidValues(ResponseTypes.Code, ResponseTypes.Token)]
        public string response_type { get; set; }

        [Required]
        public Guid? client_id { get; set; }

        [ValidUri]
        public string redirect_uri { get; set; }

        [NqsChar] // Should be NQCHAR but since we're splitting these values with a space, we need to allow space as well.
        public string scope { get; set; }

        [VsChar]
        public string state { get; set; }

        public string ToQueryParameters()
        {
            var sb = new StringBuilder();
            sb.Append($"response_type={response_type}&");
            sb.Append($"client_id={client_id}&");
            if (redirect_uri != null) { sb.Append($"redirect_uri={redirect_uri}&"); }

            if (scope != null) { sb.Append($"scope={scope}&"); }

            if (state != null) { sb.Append($"state={state}&"); }

            return sb.ToString().Trim('&');
        }
    }
}
