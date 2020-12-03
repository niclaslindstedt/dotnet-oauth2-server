using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IRefreshTokensRepository
    {
        Task<RefreshToken> FindAsync(string refreshToken);
        void Add(RefreshToken refreshToken);
        Task<int> SaveAsync();
        Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);
        void RemoveRange(List<RefreshToken> refreshTokens);
    }
}
