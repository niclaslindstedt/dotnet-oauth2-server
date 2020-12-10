using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AuthenticateUserService : IAuthenticateUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticateUserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            if (user == null)
            {
                throw new InvalidGrantException("Invalid user credentials.");
            }

            if (!_passwordHasher.Verify(password, user.Password))
            {
                throw new InvalidGrantException("Invalid user credentials.");
            }

            return user;
        }
    }
}
