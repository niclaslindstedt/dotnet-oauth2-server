using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IClientsRepository
    {
        Task<List<Client>> GetAllAsync();
        Task<List<Client>> GetByUserIdAsync(Guid userId);
        ValueTask<Client> FindAsync(Guid clientId);
        Task<Client> FindAsync(Guid clientId, Guid userId);
        Client Add(Client client);
        Task<int> SaveAsync();
        void Delete(Client client);
    }
}
