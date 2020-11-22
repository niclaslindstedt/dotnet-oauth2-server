using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class ClientsRepository : IClientsRepository
    {
        private readonly EtimoIdDbContext _dbContext;

        public ClientsRepository(EtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Client>> GetAllAsync()
        {
            return _dbContext.Clients.ToListAsync();
        }

        public Task<List<Client>> GetByUserIdAsync(Guid userId)
        {
            return _dbContext.Clients.Where(c => c.UserId == userId).ToListAsync();
        }

        public ValueTask<Client> FindAsync(Guid clientId)
        {
            return _dbContext.Clients.FindAsync(clientId);
        }

        public Task<Client> FindAsync(Guid clientId, Guid userId)
        {
            return _dbContext.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId && c.UserId == userId);
        }

        public Client Add(Client client)
        {
            return _dbContext.Clients.Add(client).Entity;
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Delete(Client client)
        {
            if (client != null)
            {
                _dbContext.Clients.Remove(client);
            }
        }
    }
}
