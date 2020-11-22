using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IClientsService
    {
        Task<List<Client>> GetByUserIdAsync(Guid userId);
        Task<List<Client>> GetAllAsync();
        Task<Client> AddAsync(Client client, Guid userId);
        ValueTask<Client> FindAsync(Guid clientId);
        Task<Client> FindAsync(Guid clientId, Guid userId);
        Task DeleteAsync(Guid clientId);
        Task DeleteAsync(Guid clientId, Guid userId);
    }
}
