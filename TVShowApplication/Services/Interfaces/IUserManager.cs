using TVShowApplication.Data.DTO.User;
using TVShowApplication.Models;

namespace TVShowApplication.Services.Interfaces
{
    public interface IUserManager
    {
        Task<bool> CreateUser(SignUpRequest request);
        Task<AuthenticatedResponse?> GetTokenForUser(SignInRequest request);
        Task<AuthenticatedResponse?> RefreshToken(RefreshTokenRequest request);
        Task Revoke(int? userId = null);
    }
}
