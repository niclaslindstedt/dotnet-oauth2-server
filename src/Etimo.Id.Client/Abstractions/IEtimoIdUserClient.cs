using Etimo.Id.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public interface IEtimoIdUserClient
    {
        Task<ResponseDto<List<UserResponseDto>>> GetUsersAsync();
        Task<ResponseDto<UserResponseDto>> GetUserAsync(Guid userId);
        Task<ResponseDto<List<RoleResponseDto>>> GetUserRolesAsync(Guid userId);
        Task<ResponseDto<UserResponseDto>> AddUserAsync(UserRequestDto user);
        Task<ResponseDto<UserResponseDto>> UnlockUserAsync(Guid userId);
        Task<ResponseDto<UserResponseDto>> UpdateUserAsync(Guid userId, UserRequestDto user);
        Task<ResponseDto<UserResponseDto>> DeleteUserAsync(Guid userId);
        Task<ResponseDto<UserResponseDto>> AddRoleToUserAsync(Guid roleId, Guid userId);
        Task<ResponseDto<UserResponseDto>> DeleteRoleFromUserAsync(Guid roleId, Guid userId);
    }
}
