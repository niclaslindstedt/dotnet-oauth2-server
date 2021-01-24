using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Dtos;
using Etimo.Id.Entities;
using Etimo.Id.Security;
using Etimo.Id.Settings;
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
        private readonly IAddApplicationService       _addApplicationService;
        private readonly IAuthenticateClientService   _authenticateClientService;
        private readonly IDeleteApplicationService    _deleteApplicationService;
        private readonly IFindApplicationService      _findApplicationService;
        private readonly IGenerateClientSecretService _generateClientSecretService;
        private readonly IGetApplicationsService      _getApplicationsService;
        private readonly IGetRolesService             _getRolesService;
        private readonly SiteSettings                 _siteSettings;
        private readonly IUpdateApplicationService    _updateApplicationService;

        public ApplicationController(
            SiteSettings siteSettings,
            IAddApplicationService addApplicationService,
            IAuthenticateClientService authenticateClientService,
            IDeleteApplicationService deleteApplicationService,
            IFindApplicationService findApplicationService,
            IGenerateClientSecretService generateClientSecretService,
            IGetApplicationsService getApplicationsService,
            IGetRolesService getRolesService,
            IUpdateApplicationService updateApplicationService)
        {
            _siteSettings                = siteSettings;
            _addApplicationService       = addApplicationService;
            _authenticateClientService   = authenticateClientService;
            _deleteApplicationService    = deleteApplicationService;
            _findApplicationService      = findApplicationService;
            _generateClientSecretService = generateClientSecretService;
            _getApplicationsService      = getApplicationsService;
            _getRolesService             = getRolesService;
            _updateApplicationService    = updateApplicationService;
        }

        [HttpGet]
        [Route("/applications")]
        [Authorize(Policy = ApplicationScopes.Read)]
        public async Task<IActionResult> GetAsync()
        {
            List<Application> applications;
            if (this.UserHasScope(ApplicationScopes.Admin)) { applications = await _getApplicationsService.GetAllAsync(); }
            else { applications = await _getApplicationsService.GetByUserIdAsync(this.GetUserId()); }

            IEnumerable<ApplicationResponseDto> found = applications.Select(app => ApplicationResponseDto.FromApplication(app, false));

            return Ok(found);
        }

        [HttpGet]
        [Route("/applications/{applicationId:int}")]
        [Authorize(Policy = ApplicationScopes.Read)]
        public async Task<IActionResult> FindAsync([FromRoute] int applicationId)
        {
            Application application;
            if (this.UserHasScope(ApplicationScopes.Admin)) { application = await _findApplicationService.FindAsync(applicationId); }
            else { application = await _findApplicationService.FindAsync(applicationId, this.GetUserId()); }

            var found = ApplicationResponseDto.FromApplication(application);

            return Ok(found);
        }

        [HttpGet]
        [Route("/applications/{applicationId:int}/roles")]
        [Authorize(Policy = CombinedScopes.ReadApplicationRole)]
        public async Task<IActionResult> GetRolesAsync([FromRoute] int applicationId)
        {
            List<Role> roles;
            if (this.UserHasScope(ApplicationScopes.Admin)) { roles = await _getRolesService.GetByApplicationId(applicationId); }
            else { roles = await _getRolesService.GetByApplicationId(applicationId, this.GetUserId()); }

            IEnumerable<RoleResponseDto> found = roles.Select(r => RoleResponseDto.FromRole(r, false));

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
                application = await _addApplicationService.AddAsync(dto.ToApplication(), dto.user_id ?? this.GetUserId());
            }
            else { application = await _addApplicationService.AddAsync(dto.ToApplication(), this.GetUserId()); }

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
                application = await _generateClientSecretService.GenerateSecretAsync(applicationId);
            }
            else { application = await _generateClientSecretService.GenerateSecretAsync(applicationId, this.GetUserId()); }

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
                application = await _updateApplicationService.UpdateAsync(
                    dto.ToApplication(applicationId),
                    dto.user_id ?? this.GetUserId());
            }
            else { application = await _updateApplicationService.UpdateAsync(dto.ToApplication(applicationId), this.GetUserId()); }

            var updated = ApplicationResponseDto.FromApplication(application);

            return Ok(updated);
        }

        [HttpDelete]
        [Route("/applications/{applicationId:int}")]
        [Authorize(Policy = ApplicationScopes.Write)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int applicationId)
        {
            if (this.UserHasScope(ApplicationScopes.Admin)) { await _deleteApplicationService.DeleteAsync(applicationId); }
            else { await _deleteApplicationService.DeleteAsync(applicationId, this.GetUserId()); }

            return NoContent();
        }
    }
}
