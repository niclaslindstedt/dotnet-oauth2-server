using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IAuthenticationRequest
    {
        string Username { get; }
        string Password { get; }
        string State { get; }
    }
}
