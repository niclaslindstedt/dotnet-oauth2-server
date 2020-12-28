using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IRequestContext
    {
        Guid?        ClientId { get; set; }
        List<string> Scopes   { get; set; }
        Guid?        UserId   { get; set; }
        string       Username { get; set; }
    }
}
