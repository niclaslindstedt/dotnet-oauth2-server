using Etimo.Id.Dtos;
using Etimo.Id.Settings;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public class EtimoIdApplicationClient
        : EtimoIdBaseClient,
            IEtimoIdApplicationClient
    {
        public EtimoIdApplicationClient(EtimoIdSettings settings, HttpClient httpClient)
            : base(settings, httpClient) { }

        public Task<ResponseDto<List<ApplicationResponseDto>>> GetApplicationsAsync()
            => GetAsync<List<ApplicationResponseDto>>("/applications");

        public Task<ResponseDto<ApplicationResponseDto>> GetApplicationAsync(int applicationId)
            => GetAsync<ApplicationResponseDto>($"/applications/{applicationId}");

        public Task<ResponseDto<RoleResponseDto>> GetApplicationRolesAsync(int applicationId)
            => GetAsync<RoleResponseDto>($"/applications/{applicationId}/roles");

        public Task<ResponseDto<ApplicationResponseDto>> AddApplicationAsync(ApplicationRequestDto application)
            => PostAsync<ApplicationResponseDto>("/applications", application);

        public Task<ResponseDto<ApplicationSecretResponseDto>> RefreshApplicationSecretAsync(int applicationId)
            => GetAsync<ApplicationSecretResponseDto>($"/applications/{applicationId}/secret");

        public Task<ResponseDto<ApplicationResponseDto>> UpdateApplicationAsync(int applicationId, ApplicationRequestDto application)
            => PutAsync<ApplicationResponseDto>($"/applications/{applicationId}", application);

        public Task<ResponseDto<ApplicationResponseDto>> DeleteApplicationAsync(int applicationId)
            => GetAsync<ApplicationResponseDto>($"/applications/{applicationId}");
    }
}
