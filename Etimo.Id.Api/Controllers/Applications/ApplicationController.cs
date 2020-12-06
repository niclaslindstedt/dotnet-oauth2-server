using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            // If the user calling is not an admin, revert to the GetByUserId method.
            if (!this.UserHasScope(ApplicationScopes.Admin))
            {
                var apps = await _applicationService.GetByUserIdAsync(this.GetUserId());
                return Ok(apps);
            }

            var allApps = await _applicationService.GetAllAsync();

            return Ok(allApps.Select(ApplicationResponseDto.FromApplication));
        }

        [HttpGet]
        [Route("/applications/{applicationId:int}")]
        [Authorize(Policy = ApplicationScopes.Read)]
        public async Task<IActionResult> FindAsync([FromRoute] int applicationId)
        {
            Application app;
            if (this.UserHasScope(ApplicationScopes.Admin))
            {
                app = await _applicationService.FindAsync(applicationId);
            }
            else
            {
                app = await _applicationService.FindAsync(applicationId, this.GetUserId());
            }

            return Ok(ApplicationResponseDto.FromApplication(app));
        }

        [HttpPost]
        [Route("/applications")]
        [ValidateModel]
        [Authorize(Policy = ApplicationScopes.Write)]
        public async Task<IActionResult> CreateAsync([FromBody] ApplicationRequestDto dto)
        {
            var app = await _applicationService.AddAsync(dto.ToApplication(), this.GetUserId());
            var created = ApplicationResponseDto.FromApplication(app);

            return Created($"{_siteSettings.ListenUri}/applications/{app.ApplicationId}", created);
        }

        [HttpPost]
        [Route("/applications/{applicationId:int}/secret")]
        [Authorize(Policy = ApplicationScopes.Write)]
        [NoCache]
        public async Task<IActionResult> GenerateSecretAsync(int applicationId)
        {
            var application = await _applicationService.GenerateSecretAsync(applicationId, this.GetUserId());

            return Ok(ApplicationSecretResponseDto.FromApplication(application));
        }

        [HttpPut]
        [Route("/applications/{applicationId:int}")]
        [ValidateModel]
        [Authorize(Policy = ApplicationScopes.Write)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int applicationId, [FromBody] ApplicationRequestDto dto)
        {
            var app = await _applicationService.UpdateAsync(dto.ToApplication(applicationId), this.GetUserId());
            var created = ApplicationResponseDto.FromApplication(app);

            return Ok(created);
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

            // If the caller doesn't have the admin:applications scope, it needs to own the application.
            await _applicationService.DeleteAsync(applicationId, this.GetUserId());

            return NoContent();
        }
    }
}
