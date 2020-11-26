// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;
using System;

namespace Etimo.Id.Api.OAuth
{
    public class AuthorizationCodeQuery
    {
        public string response_type { get; set; }
        public string client_id { get; set; }
        public string redirect_uri { get; set; }
        public string scope { get; set; }
        public string state { get; set; }
        public string code_id { get; set; }

        public AuthorizationRequest ToAuthorizeRequest(string username = null, string password = null)
        {
            var request = new AuthorizationRequest(response_type, new Guid(client_id), state, scope, redirect_uri);

            if (code_id != null)
            {
                request.AuthorizationCodeId = new Guid(code_id);
            }

            if (username != null && password != null)
            {
                request.Username = username;
                request.Password = password;
            }

            return request;
        }
    }
}
