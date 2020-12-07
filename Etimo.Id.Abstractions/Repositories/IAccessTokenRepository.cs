using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAccessTokenRepository
    {
        Task<AccessToken> FindAsync(Guid accessTokenId);
        void Add(AccessToken token);
        void Delete(AccessToken token);
        Task<int> SaveAsync();
    }
}
