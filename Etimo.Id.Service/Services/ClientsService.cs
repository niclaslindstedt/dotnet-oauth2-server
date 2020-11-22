using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Etimo.Id.Service.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Etimo.Id.Service.Services
{
    public class ClientsService : IClientsService
    {
        private readonly IClientsRepository _clientsRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ClientsService(
            IClientsRepository clientsRepository,
            IPasswordHasher passwordHasher)
        {
            _clientsRepository = clientsRepository;
            _passwordHasher = passwordHasher;
        }

        public Task<List<Client>> GetByUserIdAsync(Guid userId)
        {
            return _clientsRepository.GetByUserIdAsync(userId);
        }

        public Task<List<Client>> GetAllAsync()
        {
            return _clientsRepository.GetAllAsync();
        }

        public async Task<Client> AddAsync(Client client, Guid userId)
        {
            // Create a clear-text secret here.
            var secret = Guid.NewGuid().ToString();

            client.UserId = userId;
            client.ClientSecret = _passwordHasher.Hash(secret);

            _clientsRepository.Add(client);
            await _clientsRepository.SaveAsync();

            // The owner needs to see this at least once.
            client.ClientSecret = secret;

            return client;
        }

        public ValueTask<Client> FindAsync(Guid clientId)
        {
            return _clientsRepository.FindAsync(clientId);
        }

        public Task<Client> FindAsync(Guid clientId, Guid userId)
        {
            return _clientsRepository.FindAsync(clientId, userId);
        }

        public async Task DeleteAsync(Guid clientId)
        {
            var client = await _clientsRepository.FindAsync(clientId);
            await DeleteAsync(client);
        }

        public async Task DeleteAsync(Guid clientId, Guid userId)
        {
            var client = await _clientsRepository.FindAsync(clientId, userId);
            await DeleteAsync(client);
        }

        private async Task DeleteAsync(Client client)
        {
            if (client != null)
            {
                _clientsRepository.Delete(client);
                await _clientsRepository.SaveAsync();
            }
        }
    }
}
