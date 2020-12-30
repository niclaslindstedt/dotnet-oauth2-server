using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IRefreshTokenGenerator
    {
        Task<JwtToken> GenerateTokenAsync(IRefreshTokenRequest request);

        Task<RefreshToken> GenerateRefreshTokenAsync(
            int applicationId,
            string redirectUri,
            Guid userId,
            string scope = null);
    }
}
