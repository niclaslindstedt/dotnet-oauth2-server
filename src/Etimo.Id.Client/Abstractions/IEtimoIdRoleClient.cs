using Etimo.Id.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public interface IEtimoIdRoleClient
    {
        Task<ResponseDto<List<RoleResponseDto>>> GetRolesAsync();
        Task<ResponseDto<RoleResponseDto>> GetRoleAsync(Guid roleId);
        Task<ResponseDto<List<RoleResponseDto>>> GetRoleScopesAsync(Guid roleId);
        Task<ResponseDto<RoleResponseDto>> AddRoleAsync(RoleRequestDto role);
        Task<ResponseDto<RoleResponseDto>> UpdateRoleAsync(Guid roleId, RoleRequestDto role);
        Task<ResponseDto<RoleResponseDto>> DeleteRoleAsync(Guid roleId);
        Task<ResponseDto<RoleResponseDto>> AddScopeToRoleAsync(Guid scopeId, Guid roleId);
        Task<ResponseDto<RoleResponseDto>> DeleteScopeFromRoleAsync(Guid scopeId, Guid roleId);
    }
}
