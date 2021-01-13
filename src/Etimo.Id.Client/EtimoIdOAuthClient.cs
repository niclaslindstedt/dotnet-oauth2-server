using Etimo.Id.Dtos;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public abstract class EtimoIdOAuthClient
        : EtimoIdBaseClient,
            IEtimoIdOAuthClient
    {
        private AccessTokenResponseDto _unvalidatedAccessToken;
        private AccessTokenResponseDto _validatedAccessToken;

        protected EtimoIdOAuthClient(EtimoIdSettings settings, IHttpClientFactory clientFactory)
            : base(settings, clientFactory) { }

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

            HttpResponseMessage response = await HttpClient.PostAsync("/oauth2/token", form);
            if (response.IsSuccessStatusCode)
            {
                string jsonData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AccessTokenResponseDto>(jsonData);
            }

            return null;
        }

        private async Task<AccessTokenResponseDto> PostClientCredentialsTokenRequestAsync(
            string clientId,
            string clientSecret,
            string scope = null)
        {
            var form = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "scope", scope },
                });

            SetAuthenticationHeaders(clientId, clientSecret);
            HttpResponseMessage response = await HttpClient.PostAsync("/oauth2/token", form);
            if (response.IsSuccessStatusCode)
            {
                string jsonData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AccessTokenResponseDto>(jsonData);
            }

            return null;
        }
    }
}
