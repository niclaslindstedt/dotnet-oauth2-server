using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class GetUsersService : IGetUsersService
    {
        private readonly IUserRepository _userRepository;

        public GetUsersService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<List<User>> GetAllAsync()
        {
            return _userRepository.GetAllAsync();
        }
    }
}
