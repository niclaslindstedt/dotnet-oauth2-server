using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Etimo.Id.Client
{
    public abstract class EtimoIdBaseClient
    {
        private readonly   IHttpClientFactory _clientFactory;
        protected readonly HttpClient         HttpClient;
        protected readonly EtimoIdSettings    Settings;

        public EtimoIdBaseClient(EtimoIdSettings settings, IHttpClientFactory clientFactory)
        {
            Settings       = settings;
            _clientFactory = clientFactory;
            HttpClient     = InitializeHttpClient();
        }

        private HttpClient InitializeHttpClient()
        {
            HttpClient client = _clientFactory.CreateClient("etimo-client");
            client.BaseAddress                         = new Uri($"{Settings.Issuer.TrimEnd('/')}");
            client.DefaultRequestHeaders.Authorization = GetBasicAuthenticationHeaders();

            return client;
        }

        protected void UseAccessToken(string jwtToken)
            => HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        protected void SetAuthenticationHeaders(string username, string password)
            => HttpClient.DefaultRequestHeaders.Authorization = GetBasicAuthenticationHeaders(username, password);

        protected AuthenticationHeaderValue GetBasicAuthenticationHeaders(string username = null, string password = null)
        {
            byte[] bytes = Encoding.ASCII.GetBytes($"{username ?? Settings.ClientId}:{password ?? Settings.ClientSecret}");

            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
        }
    }
}
