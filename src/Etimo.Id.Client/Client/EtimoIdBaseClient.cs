using Etimo.Id.Dtos;
using Etimo.Id.Exceptions;
using Etimo.Id.Settings;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public abstract class EtimoIdBaseClient
    {
        protected readonly HttpClient      HttpClient;
        protected readonly EtimoIdSettings Settings;

        public EtimoIdBaseClient(EtimoIdSettings settings, HttpClient httpClient)
        {
            Settings   = settings;
            HttpClient = httpClient;
            InitializeHttpClient();
        }

        private void InitializeHttpClient()
        {
            HttpClient.BaseAddress                         = new Uri(Settings.ServerUri.TrimEnd('/'));
            HttpClient.DefaultRequestHeaders.Authorization = GetBasicAuthenticationHeaders();
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

        protected async Task<ResponseDto<T>> PostFormUrlEncodedAsync<T>(string endpoint, FormUrlEncodedContent content) where T : class
            => await ParseResponseAsync<T>(await HttpClient.PostAsync(endpoint, content));

        protected Task<ResponseDto<T>> GetAsync<T>(string endpoint) where T : class
            => CallAsync<T>(HttpMethod.Get, endpoint);

        protected Task<ResponseDto<T>> PostAsync<T>(string endpoint, object payload = null) where T : class
            => CallAsync<T>(HttpMethod.Post, endpoint, payload);

        protected Task<ResponseDto<T>> PutAsync<T>(string endpoint, object payload = null) where T : class
            => CallAsync<T>(HttpMethod.Put, endpoint, payload);

        protected Task<ResponseDto<T>> DeleteAsync<T>(string endpoint) where T : class
            => CallAsync<T>(HttpMethod.Delete, endpoint);

        private async Task<ResponseDto<T>> CallAsync<T>(
            HttpMethod method,
            string endpoint,
            object payload = null) where T : class
        {
            var request = new HttpRequestMessage
            {
                Method     = method,
                RequestUri = new Uri(HttpClient.BaseAddress + endpoint.TrimStart('/')),
            };

            if (payload != null)
            {
                string jsonPayload = JsonSerializer.Serialize(payload);
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response = await HttpClient.SendAsync(request);

            return await ParseResponseAsync<T>(response);
        }

        private async Task<ResponseDto<T>> ParseResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized) { throw new UnauthorizedException(); }

                ErrorResponseDto error = await GetErrorAsync(response);

                return new ResponseDto<T>(error) { StatusCode = (int)response.StatusCode };
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(jsonResponse))
            {
                T data = JsonSerializer.Deserialize<T>(jsonResponse);
                return new ResponseDto<T>(data) { StatusCode = (int)response.StatusCode };
            }

            return new ResponseDto<T> { StatusCode = (int)response.StatusCode };
        }

        private async Task<ErrorResponseDto> GetErrorAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) { return null; }

            string jsonData = await response.Content.ReadAsStringAsync();
            try { return JsonSerializer.Deserialize<ErrorResponseDto>(jsonData); }
            catch { return null; }
        }
    }
}
