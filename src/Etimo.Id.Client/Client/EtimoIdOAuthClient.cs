using Etimo.Id.Constants;
using Etimo.Id.Dtos;
using Etimo.Id.Settings;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public class EtimoIdOAuthClient
        : EtimoIdBaseClient,
            IEtimoIdOAuthClient
    {
        private AccessTokenResponseDto _unvalidatedAccessToken;
        private AccessTokenResponseDto _validatedAccessToken;

        public EtimoIdOAuthClient(EtimoIdSettings settings, HttpClient httpClient)
            : base(settings, httpClient) { }

        public AccessTokenResponseDto GetAccessToken()
            => _validatedAccessToken;

        public async Task<bool> AuthorizeAsync(string code, string state)
        {
            try
            {
                AccessTokenResponseDto accessTokenResponse = await PostAuthorizationCodeTokenRequestAsync(code, state);
                if (accessTokenResponse != null)
                {
                    UseAccessToken(accessTokenResponse.access_token);
                    _unvalidatedAccessToken = accessTokenResponse;
                    return true;
                }
            }
            catch { return false; }

            return false;
        }

        public async Task<bool> ClientCredentialsAsync(string clientId, string clientSecret)
        {
            try
            {
                AccessTokenResponseDto accessTokenResponse = await PostClientCredentialsTokenRequestAsync(clientId, clientSecret);
                if (accessTokenResponse != null)
                {
                    UseAccessToken(accessTokenResponse.access_token);
                    _unvalidatedAccessToken = accessTokenResponse;
                    return true;
                }
            }
            catch { return false; }

            return false;
        }

        public async Task<AccessTokenResponseDto> ResourceOwnerPasswordCredentialsAsync(string username, string password)
        {
            try { return await PostResourceOwnerPasswordCredentialsAsync(username, password); }
            catch { return null; }
        }

        public async Task<bool> ValidateAccessTokenAsync()
        {
            HttpResponseMessage verifyResponse = await HttpClient.GetAsync("/oauth2/validate");
            if (verifyResponse.IsSuccessStatusCode)
            {
                _validatedAccessToken = _unvalidatedAccessToken;
                return true;
            }

            return false;
        }

        private async Task<AccessTokenResponseDto> PostAuthorizationCodeTokenRequestAsync(string code, string state)
        {
            var form = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "state", state },
                    { "redirect_uri", Settings.RedirectUri },
                });

            ResponseDto<AccessTokenResponseDto> response = await PostFormUrlEncodedAsync<AccessTokenResponseDto>("/oauth2/token", form);

            return response.Success ? response.Data : null;
        }

        private async Task<AccessTokenResponseDto> PostClientCredentialsTokenRequestAsync(
            string clientId,
            string clientSecret,
            string scope = null)
        {
            var form = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "grant_type", GrantTypes.ClientCredentials },
                    { "scope", scope },
                });

            SetAuthenticationHeaders(clientId, clientSecret);

            ResponseDto<AccessTokenResponseDto> response = await PostFormUrlEncodedAsync<AccessTokenResponseDto>("/oauth2/token", form);

            return response.Success ? response.Data : null;
        }

        private async Task<AccessTokenResponseDto> PostResourceOwnerPasswordCredentialsAsync(string username, string password)
        {
            var form = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "grant_type", GrantTypes.Password },
                    { "username", username },
                    { "password", password },
                    { "scope", null },
                });

            SetAuthenticationHeaders(Settings.ClientId, Settings.ClientSecret);

            ResponseDto<AccessTokenResponseDto> response = await PostFormUrlEncodedAsync<AccessTokenResponseDto>("/oauth2/token", form);

            return response.Success ? response.Data : null;
        }
    }
}
