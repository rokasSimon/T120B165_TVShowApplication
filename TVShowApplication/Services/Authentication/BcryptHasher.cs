using BC = BCrypt.Net.BCrypt;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Services.Authentication
{
    public class BcryptHasher : IPasswordHasher
    {
        public string CreateSalt()
        {
            return BC.GenerateSalt();
        }

        public string HashPassword(string password, string salt)
        {
            return BC.HashPassword(password, salt);
        }

        public bool VerifyPassword(string hashedPassword, string password, string salt)
        {
            return BC.Verify(password, hashedPassword);
        }
    }
}
