using TVShowApplication.Data.DTO.User;
using TVShowApplication.Models;

namespace TVShowApplication.Services.Interfaces
{
    public interface IUserManager
    {
        Task<bool> CreateUser(SignUpRequest request);
        Task<string?> GetTokenForUser(SignInRequest request);
    }
}
