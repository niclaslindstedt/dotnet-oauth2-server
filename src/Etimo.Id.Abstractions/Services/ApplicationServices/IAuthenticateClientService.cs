using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthenticateClientService
    {
        Task<Application> AuthenticateAsync(Guid clientId, string clientSecret);
    }
}
