using Etimo.Id.Dtos;
using Etimo.Id.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public class EtimoIdRoleClient
        : EtimoIdBaseClient,
            IEtimoIdRoleClient
    {
        public EtimoIdRoleClient(EtimoIdSettings settings, HttpClient httpClient)
            : base(settings, httpClient) { }

        public Task<ResponseDto<List<RoleResponseDto>>> GetRolesAsync()
            => GetAsync<List<RoleResponseDto>>("/roles");

        public Task<ResponseDto<RoleResponseDto>> GetRoleAsync(Guid roleId)
            => GetAsync<RoleResponseDto>($"/roles/{roleId}");

        public Task<ResponseDto<List<RoleResponseDto>>> GetRoleScopesAsync(Guid roleId)
            => GetAsync<List<RoleResponseDto>>($"/roles/{roleId}/scopes");

        public Task<ResponseDto<RoleResponseDto>> AddRoleAsync(RoleRequestDto role)
            => PostAsync<RoleResponseDto>("/roles", role);

        public Task<ResponseDto<RoleResponseDto>> UpdateRoleAsync(Guid roleId, RoleRequestDto role)
            => PutAsync<RoleResponseDto>($"/roles/{roleId}", role);

        public Task<ResponseDto<RoleResponseDto>> DeleteRoleAsync(Guid roleId)
            => DeleteAsync<RoleResponseDto>($"/roles/{roleId}");

        public Task<ResponseDto<RoleResponseDto>> AddScopeToRoleAsync(Guid scopeId, Guid roleId)
            => PutAsync<RoleResponseDto>($"/roles/{roleId}/scopes/{scopeId}");

        public Task<ResponseDto<RoleResponseDto>> DeleteScopeFromRoleAsync(Guid scopeId, Guid roleId)
            => DeleteAsync<RoleResponseDto>($"/roles/{roleId}/scopes/{scopeId}");
    }
}
