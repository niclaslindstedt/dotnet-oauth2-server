using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AddUserService : IAddUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AddUserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> AddAsync(User user)
        {
            if (await _userRepository.AnyByUsernameAsync(user.Username))
            {
                throw new ConflictException("Username already exists.");
            }

            // We assume the password is not encrypted at this point.
            user.Password = _passwordHasher.Hash(user.Password);

            _userRepository.Add(user);
            await _userRepository.SaveAsync();

            return user;
        }
    }
}
