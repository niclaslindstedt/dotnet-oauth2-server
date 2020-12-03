using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAccessTokensRepository
    {
        Task<AccessToken> FindAsync(Guid accessTokenId);
        AccessToken Add(AccessToken token);
        Task<int> SaveAsync();
        void Delete(AccessToken token);
    }
}
