using Etimo.Id.Entities.Abstractions;
using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class RequestContext : IRequestContext
    {
        public string       ActiveScope { get; set; }
        public Guid?        ClientId    { get; set; }
        public List<string> Scopes      { get; set; } = new();
        public Guid?        UserId      { get; set; }
        public string       Username    { get; set; }
        public string       IpAddress   { get; set; }

        public Guid GetClientId()
            => ClientId.GetValueOrDefault();

        public Guid GetUserId()
            => UserId.GetValueOrDefault();
    }
}
