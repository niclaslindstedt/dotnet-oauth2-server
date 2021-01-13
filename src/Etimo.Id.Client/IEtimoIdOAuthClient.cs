using Etimo.Id.Dtos;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public interface IEtimoIdOAuthClient
    {
        AccessTokenResponseDto GetAccessToken();
        Task<bool> AuthorizeAsync(string code, string state);
        Task<bool> ClientCredentialsAsync(string clientId, string clientSecret);
        Task<bool> ValidateAccessTokenAsync();
    }
}
