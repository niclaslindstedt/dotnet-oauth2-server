using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AuthenticateUserService : IAuthenticateUserService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        public AuthenticateUserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public Task<User> AuthenticateAsync(IAuthenticationRequest request)
            => AuthenticateAsync(request.Username, request.Password, request.State);

        public async Task<User> AuthenticateAsync(
            string username,
            string password,
            string state = null)
        {
            User user = await _userRepository.FindByUsernameAsync(username);
            if (user == null) { throw new InvalidGrantException("Invalid user credentials.", state); }

            if (!_passwordHasher.Verify(password, user.Password)) { throw new InvalidGrantException("Invalid user credentials.", state); }

            return user;
        }
    }
}
