using Etimo.Id.Abstractions;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Models;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
using Etimo.Id.Security;
using Etimo.Id.Service.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Controllers
{
    [ApiController]
    [Route("clients")]
    public class ClientsController : Controller
    {
        private readonly SiteSettings _siteSettings;
        private readonly IClientsService _clientsService;

        public ClientsController(
            SiteSettings siteSettings,
            IClientsService clientsService)
        {
            _siteSettings = siteSettings;
            _clientsService = clientsService;
        }

        [Authorize(Policy = Policies.User)]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            // If the user calling is not an admin, revert to the GetByUserId method.
            if (!this.UserHasRole(Roles.Admin))
            {
                var userClients = await _clientsService.GetByUserIdAsync(this.GetUserId());
                return Ok(userClients);
            }

            var clients = await _clientsService.GetAllAsync();
            var userDtos = clients.Select(ClientResponseDto.FromClient);

            return Ok(userDtos);
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(NewClientRequestDto dto)
        {
            var client = await _clientsService.AddAsync(dto.ToClient(), this.GetUserId());

            return Created($"{_siteSettings.ListenUri}/clients/{client.ClientId}", NewClientResponseDto.FromClient(client));
        }

        [Authorize(Policy = Policies.User)]
        [HttpGet]
        [Route("{clientId:guid}")]
        public async Task<IActionResult> FindAsync(Guid clientId)
        {
            Client client;
            if (this.UserHasRole(Roles.Admin))
            {
                client = await _clientsService.FindAsync(clientId);
            }
            else
            {
                client = await _clientsService.FindAsync(clientId, this.GetUserId());
            }

            return Ok(ClientResponseDto.FromClient(client));
        }

        [Authorize(Policy = Policies.User)]
        [HttpDelete]
        [Route("{clientId:guid}")]
        public Task DeleteAsync(Guid clientId)
        {
            if (this.UserHasRole(Roles.Admin))
            {
                return _clientsService.DeleteAsync(clientId);
            }

            return _clientsService.DeleteAsync(clientId, this.GetUserId());
        }
    }
}
