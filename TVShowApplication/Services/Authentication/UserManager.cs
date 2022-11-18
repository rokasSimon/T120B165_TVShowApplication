using TVShowApplication.Models;
using TVShowApplication.Data.DTO.User;
using TVShowApplication.Services.Interfaces;
using System.Security.Claims;
using TVShowApplication.Extensions;
using TVShowApplication.Data.Options;
using TVShowApplication.Exceptions;
using Microsoft.Extensions.Options;

namespace TVShowApplication.Services.Authentication
{
    public class UserManager : IUserManager
    {
        private const string BasicUserRoleSecret = "basic-user";
        private const string PosterRoleSecret = "poster-user";
        private const string AdminRoleSecret = "admin-user";

        private readonly IUserDataProvider _userDataProvider;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly JwtOptions _jwtOptions;

        public UserManager(
            IUserDataProvider userDataProvider,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtGenerator jwtGenerator,
            IOptions<JwtOptions> jwtOptions)
        {
            _userDataProvider = userDataProvider;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtGenerator = jwtGenerator;
            _jwtOptions = jwtOptions.Value;
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
                _ => new User { Email = request.Email, HashedPassword = passwordHash, Salt = salt },
            };

            var insertedUser = await _userRepository.InsertUserAsync(newUser);

            return insertedUser != null;
        }

        public async Task<AuthenticatedResponse?> GetTokenForUser(SignInRequest request)
        {
            var user = await _userRepository.FindUserAsync(request.Email);
            if (user == null) return null;

            var validPassword = _passwordHasher.VerifyPassword(user.HashedPassword, request.Password, user.Salt);
            if (!validPassword) return null;

            var accessToken = _jwtGenerator.GenerateToken(DefaultClaims(user));
            var refreshToken = _jwtGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_jwtOptions.RefreshTokenExpirationDays!.Value);

            await _userRepository.UpdateUserAsync(user.Id, user);

            return new AuthenticatedResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        private static Dictionary<string, string> DefaultClaims(User user)
        {
            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.Role, user.GetRole().ToString() },
                { ClaimTypes.NameIdentifier, user.Id.ToString() },
            };

            return claims;
        }

        public async Task<AuthenticatedResponse?> RefreshToken(RefreshTokenRequest request)
        {
            var principal = _jwtGenerator.GetPrincipalFromExpiredToken(request.AccessToken);
            var idClaim = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
            var id = int.Parse(idClaim.Value);

            var user = await _userRepository.GetUserAsync<User>(id);

            if (user == null
                || user.RefreshToken != request.RefreshToken
                || user.RefreshTokenExpiryTime < DateTime.Now)
            {
                return null;
            }

            var newAccessToken = _jwtGenerator.GenerateToken(DefaultClaims(user));
            var newRefreshToken = _jwtGenerator.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            await _userRepository.UpdateUserAsync(id, user);

            return new AuthenticatedResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }

        public async Task Revoke(int? userId = null)
        {
            if (userId == null)
            {
                var user = await _userRepository.GetUserAsync<User>(_userDataProvider.UserId);

                if (user == null) throw new UnauthenticatedException("User is not logged in and cannot revoke token.");

                user.RefreshToken = null;
                await _userRepository.UpdateUserAsync(_userDataProvider.UserId, user);
            }
            else
            {
                var user = await _userRepository.GetUserAsync<User>(userId.Value);

                if (user == null) throw new ResourceNotFoundException("Unknown user id.");

                user.RefreshToken = null;
                await _userRepository.UpdateUserAsync(userId.Value, user);
            }
        }
    }
}
