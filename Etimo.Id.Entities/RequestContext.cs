using Etimo.Id.Entities.Abstractions;
using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class RequestContext : IRequestContext
    {
        public Guid? ClientId { get; set; }
        public List<string> Scopes { get; set; } = new List<string>();
        public Guid? UserId { get; set; }
        public string Username { get; set; }
        public string ActiveScope { get; set; }
    }
}
