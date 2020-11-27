using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IRefreshTokenGenerator
    {
        Task<JwtToken> GenerateTokenAsync(TokenRequest request);
        RefreshToken GenerateRefreshToken(int applicationId, string redirectUri, Guid userId);
    }
}
