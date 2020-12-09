using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
using Etimo.Id.Service.Scopes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Applications
{
    [ApiController]
    public class ApplicationController : Controller
    {
        private readonly SiteSettings _siteSettings;
        private readonly IApplicationService _applicationService;

        public ApplicationController(
            SiteSettings siteSettings,
            IApplicationService applicationService)
        {
            _siteSettings = siteSettings;
            _applicationService = applicationService;
        }

        [HttpGet]
        [Route("/applications")]
        [Authorize(Policy = ApplicationScopes.Read)]
        public async Task<IActionResult> GetAsync()
        {
            List<Application> applications;
            if (this.UserHasScope(ApplicationScopes.Admin))
            {
                applications = await _applicationService.GetAllAsync();
            }
            else
            {
                applications = await _applicationService.GetByUserIdAsync(this.GetUserId());
            }

            var found = applications.Select(ApplicationResponseDto.FromApplication);

            return Ok(found);
        }

        [HttpGet]
        [Route("/applications/{applicationId:int}")]
        [Authorize(Policy = ApplicationScopes.Read)]
        public async Task<IActionResult> FindAsync([FromRoute] int applicationId)
        {
            Application application;
            if (this.UserHasScope(ApplicationScopes.Admin))
            {
                application = await _applicationService.FindAsync(applicationId);
            }
            else
            {
                application = await _applicationService.FindAsync(applicationId, this.GetUserId());
            }

            var found = ApplicationResponseDto.FromApplication(application);

            return Ok(found);
        }

        [HttpPost]
        [Route("/applications")]
        [ValidateModel]
        [Authorize(Policy = ApplicationScopes.Write)]
        public async Task<IActionResult> CreateAsync([FromBody] ApplicationRequestDto dto)
        {
            Application application;
            if (this.UserHasScope(ApplicationScopes.Admin))
            {
                application = await _applicationService.AddAsync(dto.ToApplication(), dto.user_id ?? this.GetUserId());
            }
            else
            {
                application = await _applicationService.AddAsync(dto.ToApplication(), this.GetUserId());
            }

            var created = ApplicationResponseDto.FromApplication(application);

            return Created($"{_siteSettings.ListenUri}/applications/{application.ApplicationId}", created);
        }

        [HttpPost]
        [Route("/applications/{applicationId:int}/secret")]
        [Authorize(Policy = ApplicationScopes.Write)]
        [NoCache]
        public async Task<IActionResult> GenerateSecretAsync(int applicationId)
        {
            Application application;
            if (this.UserHasScope(ApplicationScopes.Admin))
            {
                application = await _applicationService.GenerateSecretAsync(applicationId);
            }
            else
            {
                application = await _applicationService.GenerateSecretAsync(applicationId, this.GetUserId());
            }

            var updated = ApplicationSecretResponseDto.FromApplication(application);

            return Ok(updated);
        }

        [HttpPut]
        [Route("/applications/{applicationId:int}")]
        [ValidateModel]
        [Authorize(Policy = ApplicationScopes.Write)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int applicationId, [FromBody] ApplicationRequestDto dto)
        {
            Application application;
            if (this.UserHasScope(ApplicationScopes.Admin))
            {
                application = await _applicationService.UpdateAsync(dto.ToApplication(applicationId), dto.user_id ?? this.GetUserId());
            }
            else
            {
                application = await _applicationService.UpdateAsync(dto.ToApplication(applicationId), this.GetUserId());
            }

            var updated = ApplicationResponseDto.FromApplication(application);

            return Ok(updated);
        }

        [HttpDelete]
        [Route("/applications/{applicationId:int}")]
        [Authorize(Policy = ApplicationScopes.Write)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int applicationId)
        {
            if (this.UserHasScope(ApplicationScopes.Admin))
            {
                await _applicationService.DeleteAsync(applicationId);
            }
            else
            {
                await _applicationService.DeleteAsync(applicationId, this.GetUserId());
            }

            return NoContent();
        }
    }
}
