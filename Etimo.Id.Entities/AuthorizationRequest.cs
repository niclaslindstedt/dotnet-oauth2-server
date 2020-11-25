using System;

namespace Etimo.Id.Entities
{
    /// <summary>
    /// This data object is used in the first step of authorization code grant flow.
    /// </summary>
    public class AuthorizationRequest
    {
        public string ResponseType { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
    }
}
