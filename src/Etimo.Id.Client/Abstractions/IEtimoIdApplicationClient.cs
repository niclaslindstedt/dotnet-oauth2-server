using Etimo.Id.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public interface IEtimoIdApplicationClient
    {
        Task<ResponseDto<List<ApplicationResponseDto>>> GetApplicationsAsync();
        Task<ResponseDto<ApplicationResponseDto>> GetApplicationAsync(int applicationId);
        Task<ResponseDto<RoleResponseDto>> GetApplicationRolesAsync(int applicationId);
        Task<ResponseDto<ApplicationResponseDto>> AddApplicationAsync(ApplicationRequestDto application);
        Task<ResponseDto<ApplicationSecretResponseDto>> RefreshApplicationSecretAsync(int applicationId);
        Task<ResponseDto<ApplicationResponseDto>> UpdateApplicationAsync(int applicationId, ApplicationRequestDto application);
        Task<ResponseDto<ApplicationResponseDto>> DeleteApplicationAsync(int applicationId);
    }
}
