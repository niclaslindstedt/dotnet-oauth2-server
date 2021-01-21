using Etimo.Id.Dtos;
using Etimo.Id.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public class EtimoIdScopeClient
        : EtimoIdBaseClient,
            IEtimoIdScopeClient
    {
        public EtimoIdScopeClient(EtimoIdSettings settings, HttpClient httpClient)
            : base(settings, httpClient) { }

        public Task<ResponseDto<List<ScopeResponseDto>>> GetScopesAsync()
            => GetAsync<List<ScopeResponseDto>>("/scopes");

        public Task<ResponseDto<ScopeResponseDto>> GetScopeAsync(Guid scopeId)
            => GetAsync<ScopeResponseDto>($"/scopes/{scopeId}");

        public Task<ResponseDto<List<ScopeResponseDto>>> GetScopeRolesAsync(Guid scopeId)
            => GetAsync<List<ScopeResponseDto>>($"/scopes/{scopeId}/roles");

        public Task<ResponseDto<ScopeResponseDto>> AddScopeAsync(ScopeRequestDto scope)
            => PostAsync<ScopeResponseDto>("/scopes", scope);

        public Task<ResponseDto<ScopeResponseDto>> UpdateScopeAsync(Guid scopeId, ScopeRequestDto scope)
            => PutAsync<ScopeResponseDto>($"/scopes/{scopeId}", scope);

        public Task<ResponseDto<ScopeResponseDto>> DeleteScopeAsync(Guid scopeId)
            => DeleteAsync<ScopeResponseDto>($"/scopes/{scopeId}");
    }
}
