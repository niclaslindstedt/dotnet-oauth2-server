using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IOAuthService
    {
        Task<JwtToken> GenerateTokenAsync(TokenRequest request);
        Task<AuthorizationResponse> StartAuthorizationCodeFlowAsync(AuthorizationRequest request);
        Task<Uri> FinishAuthorizationCodeAsync(AuthorizationRequest request);
    }
}
