using System.Collections.Generic;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IJwtTokenRequest
    {
        string Subject { get; }
        List<string> Audience { get; }
    }
}
