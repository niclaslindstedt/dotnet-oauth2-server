using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IRefreshTokenGenerator
    {
        Task<JwtToken> GenerateTokenAsync(IRefreshTokenRequest request);
        RefreshToken GenerateRefreshToken(int applicationId, string redirectUri, Guid userId);
    }
}
