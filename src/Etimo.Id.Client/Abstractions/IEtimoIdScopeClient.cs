using Etimo.Id.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public interface IEtimoIdScopeClient
    {
        Task<ResponseDto<List<ScopeResponseDto>>> GetScopesAsync();
        Task<ResponseDto<ScopeResponseDto>> GetScopeAsync(Guid scopeId);
        Task<ResponseDto<List<ScopeResponseDto>>> GetScopeRolesAsync(Guid scopeId);
        Task<ResponseDto<ScopeResponseDto>> AddScopeAsync(ScopeRequestDto scope);
        Task<ResponseDto<ScopeResponseDto>> UpdateScopeAsync(Guid scopeId, ScopeRequestDto scope);
        Task<ResponseDto<ScopeResponseDto>> DeleteScopeAsync(Guid scopeId);
    }
}
