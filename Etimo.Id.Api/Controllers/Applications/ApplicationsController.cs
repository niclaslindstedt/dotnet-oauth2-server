using Etimo.Id.Abstractions;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Security;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Applications
{
    [ApiController]
    [Route("applications")]
    public class ApplicationsController : Controller
    {
        private readonly SiteSettings _siteSettings;
        private readonly IApplicationsService _applicationsService;

        public ApplicationsController(
            SiteSettings siteSettings,
            IApplicationsService applicationsService)
        {
            _siteSettings = siteSettings;
            _applicationsService = applicationsService;
        }

        [Authorize(Policy = Policies.User)]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            // If the user calling is not an admin, revert to the GetByUserId method.
            if (!this.UserHasRole(Roles.Admin))
            {
                var apps = await _applicationsService.GetByUserIdAsync(this.GetUserId());
                return Ok(apps);
            }

            var allApps = await _applicationsService.GetAllAsync();

            return Ok(allApps.Select(ApplicationResponseDto.FromApplication));
        }

        [Authorize(Policy = Policies.User)]
        [HttpGet]
        [Route("{applicationId:int}")]
        public async Task<IActionResult> FindAsync([FromRoute] int applicationId)
        {
            Application app;
            if (this.UserHasRole(Roles.Admin))
            {
                app = await _applicationsService.FindAsync(applicationId);
            }
            else
            {
                app = await _applicationsService.FindAsync(applicationId, this.GetUserId());
            }

            return Ok(ApplicationResponseDto.FromApplication(app));
        }

        [Authorize(Policy = Policies.User)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ApplicationRequestDto dto)
        {
            var app = await _applicationsService.AddAsync(dto.ToApplication(), this.GetUserId());
            var created = ApplicationSecretResponseDto.FromApplication(app);

            return Created($"{_siteSettings.ListenUri}/applications/{app.ApplicationId}", created);
        }

        [Authorize(Policy = Policies.User)]
        [HttpPost]
        [Route("{applicationId:int}/secret")]
        public async Task<IActionResult> GenerateSecretAsync(int applicationId)
        {
            var application = await _applicationsService.GenerateSecretAsync(applicationId, this.GetUserId());

            return Ok(ApplicationSecretResponseDto.FromApplication(application));
        }

        [Authorize(Policy = Policies.User)]
        [HttpPut]
        [Route("{applicationId:int}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int applicationId, [FromBody] ApplicationRequestDto dto)
        {
            var app = await _applicationsService.UpdateAsync(dto.ToApplication(applicationId), this.GetUserId());
            var created = ApplicationSecretResponseDto.FromApplication(app);

            return Created($"{_siteSettings.ListenUri}/applications/{app.ApplicationId}", created);
        }

        [Authorize(Policy = Policies.User)]
        [HttpDelete]
        [Route("{applicationId:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int applicationId)
        {
            if (this.UserHasRole(Roles.Admin))
            {
                await _applicationsService.DeleteAsync(applicationId);
            }

            await _applicationsService.DeleteAsync(applicationId, this.GetUserId());

            return NoContent();
        }
    }
}