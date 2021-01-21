using Etimo.Id.Dtos;
using Etimo.Id.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public class EtimoIdUserClient
        : EtimoIdBaseClient,
            IEtimoIdUserClient
    {
        public EtimoIdUserClient(EtimoIdSettings settings, HttpClient httpClient)
            : base(settings, httpClient) { }

        public Task<ResponseDto<List<UserResponseDto>>> GetUsersAsync()
            => GetAsync<List<UserResponseDto>>("/users");

        public Task<ResponseDto<UserResponseDto>> GetUserAsync(Guid userId)
            => GetAsync<UserResponseDto>($"/users/{userId}");

        public Task<ResponseDto<List<RoleResponseDto>>> GetUserRolesAsync(Guid userId)
            => GetAsync<List<RoleResponseDto>>($"/users/{userId}/roles");

        public Task<ResponseDto<UserResponseDto>> AddUserAsync(UserRequestDto user)
            => PostAsync<UserResponseDto>("/users", user);

        public Task<ResponseDto<UserResponseDto>> UnlockUserAsync(Guid userId)
            => PutAsync<UserResponseDto>($"/users/{userId}/unlock");

        public Task<ResponseDto<UserResponseDto>> UpdateUserAsync(Guid userId, UserRequestDto user)
            => PutAsync<UserResponseDto>($"/users/{userId}", user);

        public Task<ResponseDto<UserResponseDto>> DeleteUserAsync(Guid userId)
            => DeleteAsync<UserResponseDto>($"/users/{userId}");

        public Task<ResponseDto<UserResponseDto>> AddRoleToUserAsync(Guid roleId, Guid userId)
            => PutAsync<UserResponseDto>($"/users/{userId}/roles/{roleId}");

        public Task<ResponseDto<UserResponseDto>> DeleteRoleFromUserAsync(Guid roleId, Guid userId)
            => DeleteAsync<UserResponseDto>($"/users/{userId}/roles/{roleId}");
    }
}
