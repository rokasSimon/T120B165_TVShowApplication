using TVShowApplication.Models;
using TVShowApplication.Data.DTO.User;
using TVShowApplication.Services.Interfaces;
using System.Security.Claims;
using TVShowApplication.Extensions;

namespace TVShowApplication.Services.Authentication
{
    public class UserManager : IUserManager
    {
        private const string BasicUserRoleSecret = "basic-user";
        private const string PosterRoleSecret = "poster-user";
        private const string AdminRoleSecret = "admin-user";

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtGenerator _jwtGenerator;

        public UserManager(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtGenerator jwtGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<bool> CreateUser(SignUpRequest request)
        {
            var user = await _userRepository.FindUserAsync(request.Email);

            if (user != null) return false;

            var salt = _passwordHasher.CreateSalt();
            var passwordHash = _passwordHasher.HashPassword(request.Password, salt);

            var newUser = request.RoleSecret switch
            {
                BasicUserRoleSecret => new User { Email = request.Email, HashedPassword = passwordHash, Salt = salt },
                PosterRoleSecret => new Poster { Email = request.Email, HashedPassword = passwordHash, Salt = salt, },
                AdminRoleSecret => new Administrator { Email = request.Email, HashedPassword = passwordHash, Salt = salt, },
                _ => null,
            };
            if (newUser == null) return false;

            var insertedUser = await _userRepository.InsertUserAsync(newUser);

            return insertedUser != null;
        }

        public async Task<string?> GetTokenForUser(SignInRequest request)
        {
            var user = await _userRepository.FindUserAsync(request.Email);
            if (user == null) return null;

            var validPassword = _passwordHasher.VerifyPassword(user.HashedPassword, request.Password, user.Salt);
            if (!validPassword) return null;

            return _jwtGenerator.GenerateToken(DefaultClaims(user));
        }

        private static Dictionary<string, string> DefaultClaims(User user)
        {
            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.Role, user.GetRole().ToString() }
            };

            return claims;
        }
    }
}
