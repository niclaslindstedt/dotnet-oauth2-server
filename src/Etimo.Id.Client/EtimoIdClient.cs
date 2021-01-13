using Etimo.Id.Dtos;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public class EtimoIdClient : IEtimoIdClient
    {
        private readonly IEtimoIdOAuthClient _oauthClient;

        public EtimoIdClient(IEtimoIdOAuthClient oauthClient)
        {
            _oauthClient = oauthClient;
        }

        public AccessTokenResponseDto GetAccessToken()
            => _oauthClient.GetAccessToken();

        public Task<bool> AuthorizeAsync(string code, string state)
            => _oauthClient.AuthorizeAsync(code, state);

        public Task<bool> ValidateAccessTokenAsync()
            => _oauthClient.ValidateAccessTokenAsync();

        public Task<bool> ClientCredentialsAsync(string clientId, string clientSecret)
            => _oauthClient.AuthorizeAsync(clientId, clientSecret);
    }
}
